using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WorkflowEngine.Application.DTOs;
using WorkflowEngine.Application.Interfaces;
using WorkflowEngine.Domain.Entities;
using WorkflowEngine.Domain.Base;
using WorkflowEngine.Domain.Enums;
using WorkflowEngine.Infrastructure.Data;
using WorkflowEngine.RuleEngine.Engines;

namespace WorkflowEngine.Application.Services
{
    public class WorkflowService : IWorkflowService
    {
        private readonly WorkflowDbContext _context;
        private readonly RuleEvaluator _ruleEngine;

        public WorkflowService(WorkflowDbContext context, RuleEvaluator ruleEngine)
        {
            _context = context;
            _ruleEngine = ruleEngine;
        }

        public async Task<List<PendingApprovalResponse>> GetPendingApprovals(string userId)
        {
            // Join: Assignments → Instances → Workflows
            //CHANGE: We have Commented Code here for UserId 
            var query = await (
                from a in _context.WorkflowAssignments
                join i in _context.WorkflowInstances on a.InstanceId equals i.InstanceId
                join w in _context.Workflows on i.WorkflowId equals w.WorkflowId
                where /*a.ApproverUserId == userId &&*/ a.AssignmentStatus == WorkflowStatus.Pending && i.WorkflowState == WorkflowStatus.Pending
                orderby a.created_date descending
                select new
                {
                    a.AssignmentId,
                    a.InstanceId,
                    a.LevelNumber,
                    Status = a.AssignmentStatus,
                    AssignedDate = a.created_date,
                    i.RequestId,
                    i.ApplicationCode,
                    CreatedBy = i.created_by,
                    CreatedDate = i.created_date ?? DateTime.MinValue,
                    i.CurrentLevel,
                    InstanceStatus = i.WorkflowState.ToString(),
                    i.WorkflowId,
                    w.WorkflowName
                }
            ).ToListAsync();

            // Load parameters for each instance
            var instanceIds = query.Select(q => q.InstanceId).Distinct().ToList();
            var allParams = await _context.WorkflowInstanceParameters
                .Where(p => instanceIds.Contains(p.InstanceId))
                .ToListAsync();

            return query.Select(q => new PendingApprovalResponse
            {
                AssignmentId  = q.AssignmentId,
                InstanceId    = q.InstanceId,
                RequestId     = q.RequestId,
                ApplicationCode = q.ApplicationCode,
                CreatedBy     = q.CreatedBy,
                CreatedDate   = q.CreatedDate,
                CurrentLevel  = q.CurrentLevel,
                InstanceStatus = q.InstanceStatus,
                WorkflowId    = q.WorkflowId,
                WorkflowName  = q.WorkflowName,
                LevelNumber   = q.LevelNumber,
                AssignmentStatus = q.Status.ToString(),
                AssignedDate  = q.AssignedDate ?? DateTime.MinValue,
                Parameters    = allParams
                    .Where(p => p.InstanceId == q.InstanceId)
                    .ToDictionary(p => p.ParameterName, p => p.ParameterValue)
            }).ToList();
        }

        public async Task<int> StartWorkflow(StartWorkflowRequest request)
        {
            var workflow = await _context.Workflows.FirstOrDefaultAsync(w => w.WorkflowName == request.WorkflowName && w.status == EntityStatus.Active);
            if (workflow == null) throw new Exception("Workflow not found.");

            var instance = new WorkflowInstance
            {
                WorkflowId = workflow.WorkflowId,
                ApplicationCode = request.ApplicationCode,
                RequestId = request.RequestId,
                CurrentLevel = 1,
                WorkflowState = WorkflowStatus.Pending,
                created_by = request.SubmittedBy
            };

            _context.WorkflowInstances.Add(instance);
            await _context.SaveChangesAsync();

            if (request.Parameters != null)
            {
                foreach (var param in request.Parameters)
                {
                    _context.WorkflowInstanceParameters.Add(new WorkflowInstanceParameter
                    {
                        InstanceId = instance.InstanceId,
                        ParameterName = param.Key,
                        ParameterValue = param.Value?.ToString()
                    });
                }
                await _context.SaveChangesAsync();
            }

            await AssignApprovers(instance.InstanceId, 1, request.Parameters);
            return instance.InstanceId;
        }

        private async Task AssignApprovers(int instanceId, int levelNumer, Dictionary<string, object> parameters)
        {
            var instance = await _context.WorkflowInstances.FindAsync(instanceId);
            if (instance == null) return;

            // Delegation check
            var activeDelegations = await _context.WorkflowDelegations
                .Where(d => d.status == EntityStatus.Active && d.StartDate <= DateTime.UtcNow && d.EndDate >= DateTime.UtcNow)
                .ToListAsync();

            var levelConfig = await _context.WorkflowLevels
                .Include(l => l.Rules)
                    .ThenInclude(r => r.Approvers)
                .FirstOrDefaultAsync(l => l.WorkflowId == instance.WorkflowId && l.LevelNumber == levelNumer);

            if (levelConfig == null) return;

            foreach (var rule in levelConfig.Rules.OrderBy(r => r.Priority))
            {
                if (await _ruleEngine.EvaluateAsync(rule.RuleExpression, parameters))
                {
                    foreach (var a in rule.Approvers)
                    {
                        var finalApprover = a.ApproverValue;
                        
                        // Check if the approver has delegated their authority
                        var delegation = activeDelegations.FirstOrDefault(d => d.FromUserId == finalApprover);
                        if (delegation != null)
                        {
                            finalApprover = delegation.ToUserId;
                            await _context.WorkflowHistory.AddAsync(new WorkflowHistory {
                                InstanceId = instanceId, LevelNumber = levelNumer, UserId = a.ApproverValue,
                                Action = WorkflowAction.Approve, Remarks = $"Automatically delegated to {delegation.ToUserId}"
                            });
                        }

                        if (!await _context.WorkflowAssignments.AnyAsync(w => w.InstanceId == instanceId && w.LevelNumber == levelNumer && w.ApproverUserId == finalApprover))
                        {
                            _context.WorkflowAssignments.Add(new WorkflowAssignment {
                                InstanceId = instanceId, LevelNumber = levelNumer, ApproverUserId = finalApprover, AssignmentStatus = WorkflowStatus.Pending
                            });
                        }
                    }
                    await _context.SaveChangesAsync();
                    // No break here - allows multiple rules to execute for the same level
                }
            }
        }

        public async Task Approve(ApproveRequest request)
        {
            var assignment = await _context.WorkflowAssignments
                .FirstOrDefaultAsync(x => x.InstanceId == request.InstanceId && x.ApproverUserId == request.UserId && x.AssignmentStatus == WorkflowStatus.Pending);

            if (assignment == null) throw new Exception("Pending assignment not found.");

            assignment.AssignmentStatus = WorkflowStatus.Approved;
            assignment.ActionDate = DateTime.UtcNow;

            _context.WorkflowHistory.Add(new WorkflowHistory {
                InstanceId = request.InstanceId, LevelNumber = assignment.LevelNumber, 
                UserId = request.UserId, Action = WorkflowAction.Approve, Remarks = request.Remarks
            });

            await _context.SaveChangesAsync();
            await ProcessTransition(request.InstanceId, assignment.LevelNumber, WorkflowAction.Approve);
        }

        public async Task Reject(ApproveRequest request) // Reusing approve request for DTO
        {
            var assignment = await _context.WorkflowAssignments
                .FirstOrDefaultAsync(x => x.InstanceId == request.InstanceId && x.ApproverUserId == request.UserId && x.AssignmentStatus == WorkflowStatus.Pending);

            if (assignment == null) throw new Exception("Pending assignment not found.");

            assignment.AssignmentStatus = WorkflowStatus.Rejected;
            assignment.ActionDate = DateTime.UtcNow;

            _context.WorkflowHistory.Add(new WorkflowHistory {
                InstanceId = request.InstanceId, LevelNumber = assignment.LevelNumber, 
                UserId = request.UserId, Action = WorkflowAction.Reject, Remarks = request.Remarks
            });

            await _context.SaveChangesAsync();
            await ProcessTransition(request.InstanceId, assignment.LevelNumber, WorkflowAction.Reject);
        }

        private async Task ProcessTransition(int instanceId, int currentLevel, WorkflowAction action)
        {
            var instance = await _context.WorkflowInstances.FindAsync(instanceId);
            var levelConfig = await _context.WorkflowLevels.FirstOrDefaultAsync(l => l.WorkflowId == instance.WorkflowId && l.LevelNumber == currentLevel);

            bool levelActionCompleted = false;

            if (action == WorkflowAction.Reject)
            {
                // Typically rejection by one person is rejection for the whole level
                levelActionCompleted = true;
            }
            else // Approve
            {
                var assignments = await _context.WorkflowAssignments.Where(a => a.InstanceId == instanceId && a.LevelNumber == currentLevel).ToListAsync();
                var approvedCount = assignments.Count(a => a.AssignmentStatus == WorkflowStatus.Approved);
                var totalCount = assignments.Count;

                levelActionCompleted = levelConfig.ApprovalStrategy switch
                {
                    "Single" or "AnyOne" => approvedCount >= 1,
                    "All" => approvedCount == totalCount,
                    "Majority" => approvedCount > (totalCount / 2),
                    _ => approvedCount >= levelConfig.MinApprovalsRequired
                };
            }

            if (levelActionCompleted)
            {
                var transition = await _context.WorkflowTransitions
                    .FirstOrDefaultAsync(t => t.WorkflowId == instance.WorkflowId && t.FromLevel == currentLevel && t.Action == action);

                if (transition != null)
                {
                    if (transition.ToLevel == 0) // Logical end for 'Approve'
                    {
                        instance.WorkflowState = action == WorkflowAction.Approve ? WorkflowStatus.Completed : WorkflowStatus.Rejected;
                        instance.CompletedDate = DateTime.UtcNow;
                    }
                    else
                    {
                        instance.CurrentLevel = transition.ToLevel;
                        instance.WorkflowState = WorkflowStatus.Pending;
                        
                        // Re-trigger assignment for the new level
                        var parameters = await _context.WorkflowInstanceParameters
                            .Where(p => p.InstanceId == instanceId).ToDictionaryAsync(p => p.ParameterName, p => (object)p.ParameterValue);
                        await AssignApprovers(instanceId, transition.ToLevel, parameters);
                    }
                }
                else if (action == WorkflowAction.Approve)
                {
                    // Fallback: move to next increment if no specific transition exists
                    var nextLevel = currentLevel + 1;
                    var hasNext = await _context.WorkflowLevels.AnyAsync(l => l.WorkflowId == instance.WorkflowId && l.LevelNumber == nextLevel);
                    if (hasNext)
                    {
                        instance.CurrentLevel = nextLevel;
                        var parameters = await _context.WorkflowInstanceParameters
                            .Where(p => p.InstanceId == instanceId).ToDictionaryAsync(p => p.ParameterName, p => (object)p.ParameterValue);
                        await AssignApprovers(instanceId, nextLevel, parameters);
                    }
                    else
                    {
                        instance.WorkflowState = WorkflowStatus.Completed;
                        instance.CompletedDate = DateTime.UtcNow;
                    }
                }
                else if (action == WorkflowAction.Reject)
                {
                    // Fallback: If no custom transition exists, a rejection at any level rejects the entire workflow.
                    instance.WorkflowState = WorkflowStatus.Rejected;
                    instance.CompletedDate = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}
