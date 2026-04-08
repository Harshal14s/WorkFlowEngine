using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WorkflowEngine.Application.DTOs;
using WorkflowEngine.Application.Interfaces;
using WorkflowEngine.Domain.Entities;
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
                where /*a.ApproverUserId == userId &&*/ a.Status == "Pending" && i.Status == "Pending"
                orderby a.AssignedDate descending
                select new
                {
                    a.AssignmentId,
                    a.InstanceId,
                    a.LevelNumber,
                    a.Status,
                    a.AssignedDate,
                    i.RequestId,
                    i.ApplicationCode,
                    i.CreatedBy,
                    i.CreatedDate,
                    i.CurrentLevel,
                    InstanceStatus = i.Status,
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
                AssignmentStatus = q.Status,
                AssignedDate  = q.AssignedDate,
                Parameters    = allParams
                    .Where(p => p.InstanceId == q.InstanceId)
                    .ToDictionary(p => p.ParameterName, p => p.ParameterValue)
            }).ToList();
        }

        public async Task<int> StartWorkflow(StartWorkflowRequest request)
        {
            var workflow = await _context.Workflows.FirstOrDefaultAsync(w => w.WorkflowName == request.WorkflowName && w.IsActive);
            if (workflow == null) throw new Exception("Workflow not found.");

            var instance = new WorkflowInstance
            {
                WorkflowId = workflow.WorkflowId,
                ApplicationCode = request.ApplicationCode,
                RequestId = request.RequestId,
                CurrentLevel = 1,
                Status = "Pending",
                CreatedBy = request.SubmittedBy,
                CreatedDate = DateTime.UtcNow
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
                .Where(d => d.IsActive && d.StartDate <= DateTime.UtcNow && d.EndDate >= DateTime.UtcNow)
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
                                Action = "Delegated", Remarks = $"Automatically delegated to {delegation.ToUserId}", ActionDate = DateTime.UtcNow
                            });
                        }

                        // Prevent duplicate assignments for the same person at the same level
                        if (!await _context.WorkflowAssignments.AnyAsync(w => w.InstanceId == instanceId && w.LevelNumber == levelNumer && w.ApproverUserId == finalApprover))
                        {
                            _context.WorkflowAssignments.Add(new WorkflowAssignment {
                                InstanceId = instanceId, LevelNumber = levelNumer, ApproverUserId = finalApprover, Status = "Pending", AssignedDate = DateTime.UtcNow
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
                .FirstOrDefaultAsync(x => x.InstanceId == request.InstanceId && x.ApproverUserId == request.UserId && x.Status == "Pending");

            if (assignment == null) throw new Exception("Pending assignment not found.");

            assignment.Status = "Approved";
            assignment.ActionDate = DateTime.UtcNow;

            _context.WorkflowHistory.Add(new WorkflowHistory {
                InstanceId = request.InstanceId, LevelNumber = assignment.LevelNumber, 
                UserId = request.UserId, Action = "Approved", Remarks = request.Remarks, ActionDate = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            await ProcessTransition(request.InstanceId, assignment.LevelNumber, "Approve");
        }

        public async Task Reject(ApproveRequest request) // Reusing approve request for DTO
        {
            var assignment = await _context.WorkflowAssignments
                .FirstOrDefaultAsync(x => x.InstanceId == request.InstanceId && x.ApproverUserId == request.UserId && x.Status == "Pending");

            if (assignment == null) throw new Exception("Pending assignment not found.");

            assignment.Status = "Rejected";
            assignment.ActionDate = DateTime.UtcNow;

            _context.WorkflowHistory.Add(new WorkflowHistory {
                InstanceId = request.InstanceId, LevelNumber = assignment.LevelNumber, 
                UserId = request.UserId, Action = "Rejected", Remarks = request.Remarks, ActionDate = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            await ProcessTransition(request.InstanceId, assignment.LevelNumber, "Reject");
        }

        private async Task ProcessTransition(int instanceId, int currentLevel, string action)
        {
            var instance = await _context.WorkflowInstances.FindAsync(instanceId);
            var levelConfig = await _context.WorkflowLevels.FirstOrDefaultAsync(l => l.WorkflowId == instance.WorkflowId && l.LevelNumber == currentLevel);

            bool levelActionCompleted = false;

            if (action == "Reject")
            {
                // Typically rejection by one person is rejection for the whole level
                levelActionCompleted = true;
            }
            else // Approve
            {
                var assignments = await _context.WorkflowAssignments.Where(a => a.InstanceId == instanceId && a.LevelNumber == currentLevel).ToListAsync();
                var approvedCount = assignments.Count(a => a.Status == "Approved");
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
                        instance.Status = action == "Approve" ? "Completed" : "Rejected";
                        instance.CompletedDate = DateTime.UtcNow;
                    }
                    else
                    {
                        instance.CurrentLevel = transition.ToLevel;
                        instance.Status = "Pending";
                        
                        // Re-trigger assignment for the new level
                        var parameters = await _context.WorkflowInstanceParameters
                            .Where(p => p.InstanceId == instanceId).ToDictionaryAsync(p => p.ParameterName, p => (object)p.ParameterValue);
                        await AssignApprovers(instanceId, transition.ToLevel, parameters);
                    }
                }
                else if (action == "Approve")
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
                        instance.Status = "Completed";
                        instance.CompletedDate = DateTime.UtcNow;
                    }
                }
                else if (action == "Reject")
                {
                    // Fallback: If no custom transition exists, a rejection at any level rejects the entire workflow.
                    instance.Status = "Rejected";
                    instance.CompletedDate = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}
