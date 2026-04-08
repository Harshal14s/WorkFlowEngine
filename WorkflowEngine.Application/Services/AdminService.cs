using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WorkflowEngine.Application.DTOs;
using WorkflowEngine.Application.Interfaces;
using WorkflowEngine.Domain.Entities;
using WorkflowEngine.Infrastructure.Data;

namespace WorkflowEngine.Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly WorkflowDbContext _context;

        public AdminService(WorkflowDbContext context)
        {
            _context = context;
        }

        // ── Dashboard Stats ──────────────────────────────────────────

        public async Task<DashboardStatsResponse> GetDashboardStats()
        {
            var instances = await _context.WorkflowInstances.ToListAsync();

            var recent = await (
                from i in _context.WorkflowInstances
                join w in _context.Workflows on i.WorkflowId equals w.WorkflowId
                orderby i.CreatedDate descending
                select new RecentInstanceResponse
                {
                    InstanceId    = i.InstanceId,
                    RequestId     = i.RequestId,
                    ApplicationCode = i.ApplicationCode,
                    WorkflowName  = w.WorkflowName,
                    CreatedBy     = i.CreatedBy,
                    CurrentLevel  = i.CurrentLevel,
                    Status        = i.Status,
                    CreatedDate   = i.CreatedDate
                }
            ).Take(20).ToListAsync();

            return new DashboardStatsResponse
            {
                TotalRequests    = instances.Count,
                PendingRequests  = instances.Count(i => i.Status == "Pending"),
                ApprovedRequests = instances.Count(i => i.Status == "Completed"),
                RejectedRequests = instances.Count(i => i.Status == "Rejected"),
                RecentInstances  = recent
            };
        }

        // ── 1. Applications ──────────────────────────────────────────

        public async Task<List<ApplicationResponse>> GetApplications()
            => await _context.Applications
                .Select(a => MapApplication(a))
                .ToListAsync();

        public async Task<ApplicationResponse> GetApplication(int id)
        {
            var entity = await _context.Applications.FindAsync(id);
            return entity == null ? null : MapApplication(entity);
        }

        public async Task<int> CreateApplication(ApplicationRequest request)
        {
            var entity = new WorkflowApplication
            {
                ApplicationCode = request.ApplicationCode,
                ApplicationName = request.ApplicationName,
                IsActive = request.IsActive
            };
            _context.Applications.Add(entity);
            await _context.SaveChangesAsync();
            return entity.ApplicationId;
        }

        public async Task UpdateApplication(int id, ApplicationRequest request)
        {
            var entity = await _context.Applications.FindAsync(id);
            if (entity == null) return;
            entity.ApplicationCode = request.ApplicationCode;
            entity.ApplicationName = request.ApplicationName;
            entity.IsActive = request.IsActive;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteApplication(int id)
        {
            var entity = await _context.Applications.FindAsync(id);
            if (entity != null) { _context.Applications.Remove(entity); await _context.SaveChangesAsync(); }
        }

        // ── 2. Workflows ─────────────────────────────────────────────

        public async Task<List<WorkflowResponse>> GetAllWorkflows()
            => await _context.Workflows
                .Select(w => MapWorkflow(w))
                .ToListAsync();

        public async Task<List<WorkflowResponse>> GetWorkflows(int applicationId)
            => await _context.Workflows
                .Where(w => w.ApplicationId == applicationId)
                .Select(w => MapWorkflow(w))
                .ToListAsync();

        public async Task<WorkflowResponse> GetWorkflow(int id)
        {
            var entity = await _context.Workflows.FindAsync(id);
            return entity == null ? null : MapWorkflow(entity);
        }

        public async Task<int> CreateWorkflow(WorkflowRequest request)
        {
            var entity = new Workflow
            {
                ApplicationId = request.ApplicationId,
                WorkflowName = request.WorkflowName,
                Version = request.Version,
                IsActive = request.IsActive
            };
            _context.Workflows.Add(entity);
            await _context.SaveChangesAsync();
            return entity.WorkflowId;
        }

        public async Task UpdateWorkflow(int id, WorkflowRequest request)
        {
            var entity = await _context.Workflows.FindAsync(id);
            if (entity == null) return;
            entity.ApplicationId = request.ApplicationId;
            entity.WorkflowName = request.WorkflowName;
            entity.Version = request.Version;
            entity.IsActive = request.IsActive;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteWorkflow(int id)
        {
            var entity = await _context.Workflows.FindAsync(id);
            if (entity != null) { _context.Workflows.Remove(entity); await _context.SaveChangesAsync(); }
        }

        // ── 3. Levels ────────────────────────────────────────────────

        public async Task<List<LevelResponse>> GetAllLevels()
            => await _context.WorkflowLevels
                .Join(_context.Workflows,
                    l => l.WorkflowId,
                    w => w.WorkflowId,
                    (l, w) => new LevelResponse
                    {
                        LevelId = l.LevelId,
                        WorkflowId = l.WorkflowId,
                        WorkflowName = w.WorkflowName,
                        LevelNumber = l.LevelNumber,
                        LevelName = l.LevelName,
                        ApprovalStrategy = l.ApprovalStrategy,
                        MinApprovalsRequired = l.MinApprovalsRequired
                    })
                .ToListAsync();

        public async Task<List<LevelResponse>> GetWorkflowLevels(int workflowId)
            => await _context.WorkflowLevels
                .Where(l => l.WorkflowId == workflowId)
                .Join(_context.Workflows,
                    l => l.WorkflowId,
                    w => w.WorkflowId,
                    (l, w) => new LevelResponse
                    {
                        LevelId = l.LevelId,
                        WorkflowId = l.WorkflowId,
                        WorkflowName = w.WorkflowName,
                        LevelNumber = l.LevelNumber,
                        LevelName = l.LevelName,
                        ApprovalStrategy = l.ApprovalStrategy,
                        MinApprovalsRequired = l.MinApprovalsRequired
                    })
                .OrderBy(l => l.LevelNumber)
                .ToListAsync();

        public async Task<LevelResponse> GetLevel(int id)
        {
            var entity = await _context.WorkflowLevels.FindAsync(id);
            if (entity == null) return null;
            var workflow = await _context.Workflows.FindAsync(entity.WorkflowId);
            return MapLevel(entity, workflow?.WorkflowName);
        }

        public async Task AddLevel(LevelRequest request)
        {
            var entity = new WorkflowLevel
            {
                WorkflowId = request.WorkflowId,
                LevelNumber = request.LevelNumber,
                LevelName = request.LevelName,
                ApprovalStrategy = request.ApprovalStrategy,
                MinApprovalsRequired = request.MinApprovalsRequired
            };
            _context.WorkflowLevels.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateLevel(int id, LevelRequest request)
        {
            var entity = await _context.WorkflowLevels.FindAsync(id);
            if (entity == null) return;
            entity.WorkflowId = request.WorkflowId;
            entity.LevelNumber = request.LevelNumber;
            entity.LevelName = request.LevelName;
            entity.ApprovalStrategy = request.ApprovalStrategy;
            entity.MinApprovalsRequired = request.MinApprovalsRequired;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteLevel(int id)
        {
            var entity = await _context.WorkflowLevels.FindAsync(id);
            if (entity != null) { _context.WorkflowLevels.Remove(entity); await _context.SaveChangesAsync(); }
        }

        // ── 4. Rules ─────────────────────────────────────────────────

        public async Task<List<RuleResponse>> GetAllRules()
            => await _context.WorkflowRules
                .Select(r => MapRule(r))
                .ToListAsync();

        public async Task<List<RuleResponse>> GetWorkflowRules(int levelId)
            => await _context.WorkflowRules
                .Where(r => r.LevelId == levelId)
                .Select(r => MapRule(r))
                .ToListAsync();

        public async Task<RuleResponse> GetRule(int id)
        {
            var entity = await _context.WorkflowRules.FindAsync(id);
            return entity == null ? null : MapRule(entity);
        }

        public async Task AddRule(RuleRequest request)
        {
            var entity = new WorkflowRule
            {
                LevelId = request.LevelId,
                RuleExpression = request.RuleExpression,
                Priority = request.Priority,
                IsActive = request.IsActive
            };
            _context.WorkflowRules.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRule(int id, RuleRequest request)
        {
            var entity = await _context.WorkflowRules.FindAsync(id);
            if (entity == null) return;
            entity.LevelId = request.LevelId;
            entity.RuleExpression = request.RuleExpression;
            entity.Priority = request.Priority;
            entity.IsActive = request.IsActive;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRule(int id)
        {
            var entity = await _context.WorkflowRules.FindAsync(id);
            if (entity != null) { _context.WorkflowRules.Remove(entity); await _context.SaveChangesAsync(); }
        }

        // ── 5. Approvers ─────────────────────────────────────────────

        public async Task<List<ApproverResponse>> GetAllApprovers()
            => await _context.WorkflowRuleApprovers
                .Join(_context.WorkflowRules,
                    a => a.RuleId, r => r.RuleId,
                    (a, r) => new { a, r })
                .Join(_context.WorkflowLevels,
                    ar => ar.r.LevelId, l => l.LevelId,
                    (ar, l) => new { ar.a, ar.r, l })
                .Join(_context.Workflows,
                    arl => arl.l.WorkflowId, w => w.WorkflowId,
                    (arl, w) => new ApproverResponse
                    {
                        Id = arl.a.Id,
                        RuleId = arl.a.RuleId,
                        RuleExpression = arl.r.RuleExpression,
                        LevelId = arl.l.LevelId,
                        LevelName = arl.l.LevelName,
                        LevelNumber = arl.l.LevelNumber,
                        WorkflowId = w.WorkflowId,
                        WorkflowName = w.WorkflowName,
                        ApproverType = arl.a.ApproverType,
                        ApproverValue = arl.a.ApproverValue
                    })
                .ToListAsync();

        public async Task<List<ApproverResponse>> GetWorkflowRuleApprovers(int ruleId)
            => await _context.WorkflowRuleApprovers
                .Where(a => a.RuleId == ruleId)
                .Join(_context.WorkflowRules,
                    a => a.RuleId, r => r.RuleId,
                    (a, r) => new { a, r })
                .Join(_context.WorkflowLevels,
                    ar => ar.r.LevelId, l => l.LevelId,
                    (ar, l) => new { ar.a, ar.r, l })
                .Join(_context.Workflows,
                    arl => arl.l.WorkflowId, w => w.WorkflowId,
                    (arl, w) => new ApproverResponse
                    {
                        Id = arl.a.Id,
                        RuleId = arl.a.RuleId,
                        RuleExpression = arl.r.RuleExpression,
                        LevelId = arl.l.LevelId,
                        LevelName = arl.l.LevelName,
                        LevelNumber = arl.l.LevelNumber,
                        WorkflowId = w.WorkflowId,
                        WorkflowName = w.WorkflowName,
                        ApproverType = arl.a.ApproverType,
                        ApproverValue = arl.a.ApproverValue
                    })
                .ToListAsync();

        public async Task<ApproverResponse> GetApprover(int id)
        {
            var a = await _context.WorkflowRuleApprovers.FindAsync(id);
            if (a == null) return null;
            var r = await _context.WorkflowRules.FindAsync(a.RuleId);
            var l = r != null ? await _context.WorkflowLevels.FindAsync(r.LevelId) : null;
            var w = l != null ? await _context.Workflows.FindAsync(l.WorkflowId) : null;
            return new ApproverResponse
            {
                Id = a.Id,
                RuleId = a.RuleId,
                RuleExpression = r?.RuleExpression,
                LevelId = l?.LevelId ?? 0,
                LevelName = l?.LevelName,
                LevelNumber = l?.LevelNumber ?? 0,
                WorkflowId = w?.WorkflowId ?? 0,
                WorkflowName = w?.WorkflowName,
                ApproverType = a.ApproverType,
                ApproverValue = a.ApproverValue
            };
        }

        public async Task AddApprover(ApproverRequest request)
        {
            var entity = new WorkflowRuleApprover
            {
                RuleId = request.RuleId,
                ApproverType = request.ApproverType,
                ApproverValue = request.ApproverValue
            };
            _context.WorkflowRuleApprovers.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateApprover(int id, ApproverRequest request)
        {
            var entity = await _context.WorkflowRuleApprovers.FindAsync(id);
            if (entity == null) return;
            entity.RuleId = request.RuleId;
            entity.ApproverType = request.ApproverType;
            entity.ApproverValue = request.ApproverValue;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteApprover(int id)
        {
            var entity = await _context.WorkflowRuleApprovers.FindAsync(id);
            if (entity != null) { _context.WorkflowRuleApprovers.Remove(entity); await _context.SaveChangesAsync(); }
        }

        // ── 6. Transitions ───────────────────────────────────────────

        public async Task<List<TransitionResponse>> GetAllTransitions()
            => await _context.WorkflowTransitions
                .Select(t => MapTransition(t))
                .ToListAsync();

        public async Task<List<TransitionResponse>> GetWorkflowTransitions(int workflowId)
            => await _context.WorkflowTransitions
                .Where(t => t.WorkflowId == workflowId)
                .Select(t => MapTransition(t))
                .ToListAsync();

        public async Task<TransitionResponse> GetTransition(int id)
        {
            var entity = await _context.WorkflowTransitions.FindAsync(id);
            return entity == null ? null : MapTransition(entity);
        }

        public async Task AddTransition(TransitionRequest request)
        {
            var entity = new WorkflowTransition
            {
                WorkflowId = request.WorkflowId,
                FromLevel = request.FromLevel,
                ToLevel = request.ToLevel,
                Action = request.Action,
                TransitionType = request.TransitionType
            };
            _context.WorkflowTransitions.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTransition(int id, TransitionRequest request)
        {
            var entity = await _context.WorkflowTransitions.FindAsync(id);
            if (entity == null) return;
            entity.WorkflowId = request.WorkflowId;
            entity.FromLevel = request.FromLevel;
            entity.ToLevel = request.ToLevel;
            entity.Action = request.Action;
            entity.TransitionType = request.TransitionType;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTransition(int id)
        {
            var entity = await _context.WorkflowTransitions.FindAsync(id);
            if (entity != null) { _context.WorkflowTransitions.Remove(entity); await _context.SaveChangesAsync(); }
        }

        // ── 7. Instances ─────────────────────────────────────────────

        public async Task<List<InstanceResponse>> GetInstances()
            => await _context.WorkflowInstances
                .Select(i => MapInstance(i))
                .ToListAsync();

        public async Task<InstanceResponse> GetInstance(int id)
        {
            var entity = await _context.WorkflowInstances.FindAsync(id);
            return entity == null ? null : MapInstance(entity);
        }

        public async Task UpdateInstance(int id, InstanceRequest request)
        {
            var entity = await _context.WorkflowInstances.FindAsync(id);
            if (entity == null) return;
            entity.WorkflowId = request.WorkflowId;
            entity.ApplicationCode = request.ApplicationCode;
            entity.RequestId = request.RequestId;
            entity.CurrentLevel = request.CurrentLevel;
            entity.Status = request.Status;
            entity.CreatedBy = request.CreatedBy;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteInstance(int id)
        {
            var entity = await _context.WorkflowInstances.FindAsync(id);
            if (entity != null) { _context.WorkflowInstances.Remove(entity); await _context.SaveChangesAsync(); }
        }

        // ── 8. Parameters ────────────────────────────────────────────

        public async Task<List<ParameterResponse>> GetAllInstanceParameters()
            => await _context.WorkflowInstanceParameters
                .Select(p => MapParameter(p))
                .ToListAsync();

        public async Task<List<ParameterResponse>> GetInstanceParameters(int instanceId)
            => await _context.WorkflowInstanceParameters
                .Where(p => p.InstanceId == instanceId)
                .Select(p => MapParameter(p))
                .ToListAsync();

        public async Task AddInstanceParameter(ParameterRequest request)
        {
            var entity = new WorkflowInstanceParameter
            {
                InstanceId = request.InstanceId,
                ParameterName = request.ParameterName,
                ParameterValue = request.ParameterValue
            };
            _context.WorkflowInstanceParameters.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteInstanceParameter(int id)
        {
            var entity = await _context.WorkflowInstanceParameters.FindAsync(id);
            if (entity != null) { _context.WorkflowInstanceParameters.Remove(entity); await _context.SaveChangesAsync(); }
        }

        // ── 9. Assignments ───────────────────────────────────────────

        public async Task<List<AssignmentResponse>> GetAllAssignments()
            => await _context.WorkflowAssignments
                .Select(a => MapAssignment(a))
                .ToListAsync();

        public async Task<List<AssignmentResponse>> GetAssignments(int instanceId)
            => await _context.WorkflowAssignments
                .Where(a => a.InstanceId == instanceId)
                .Select(a => MapAssignment(a))
                .ToListAsync();

        public async Task AddAssignment(AssignmentRequest request)
        {
            var entity = new WorkflowAssignment
            {
                InstanceId = request.InstanceId,
                LevelNumber = request.LevelNumber,
                ApproverUserId = request.ApproverUserId,
                Status = request.Status,
                AssignedDate = DateTime.UtcNow
            };
            _context.WorkflowAssignments.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAssignment(int id, AssignmentRequest request)
        {
            var entity = await _context.WorkflowAssignments.FindAsync(id);
            if (entity == null) return;
            entity.InstanceId = request.InstanceId;
            entity.LevelNumber = request.LevelNumber;
            entity.ApproverUserId = request.ApproverUserId;
            entity.Status = request.Status;
            entity.ActionDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAssignment(int id)
        {
            var entity = await _context.WorkflowAssignments.FindAsync(id);
            if (entity != null) { _context.WorkflowAssignments.Remove(entity); await _context.SaveChangesAsync(); }
        }

        // ── 10. History ──────────────────────────────────────────────

        public async Task<List<HistoryResponse>> GetAllHistory()
            => await _context.WorkflowHistory
                .Select(h => MapHistory(h))
                .ToListAsync();

        public async Task<List<HistoryResponse>> GetHistory(int instanceId)
            => await _context.WorkflowHistory
                .Where(h => h.InstanceId == instanceId)
                .Select(h => MapHistory(h))
                .ToListAsync();

        public async Task DeleteHistory(int id)
        {
            var entity = await _context.WorkflowHistory.FindAsync(id);
            if (entity != null) { _context.WorkflowHistory.Remove(entity); await _context.SaveChangesAsync(); }
        }

        // ── 11. Delegations ──────────────────────────────────────────

        public async Task<List<DelegationResponse>> GetDelegations()
            => await _context.WorkflowDelegations
                .Select(d => MapDelegation(d))
                .ToListAsync();

        public async Task<DelegationResponse> GetDelegation(int id)
        {
            var entity = await _context.WorkflowDelegations.FindAsync(id);
            return entity == null ? null : MapDelegation(entity);
        }

        public async Task<int> CreateDelegation(DelegationRequest request)
        {
            var entity = new WorkflowDelegation
            {
                FromUserId = request.FromUserId,
                ToUserId = request.ToUserId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                IsActive = request.IsActive,
                CreatedDate = DateTime.UtcNow
            };
            _context.WorkflowDelegations.Add(entity);
            await _context.SaveChangesAsync();
            return entity.DelegationId;
        }

        public async Task UpdateDelegation(int id, DelegationRequest request)
        {
            var entity = await _context.WorkflowDelegations.FindAsync(id);
            if (entity == null) return;
            entity.FromUserId = request.FromUserId;
            entity.ToUserId = request.ToUserId;
            entity.StartDate = request.StartDate;
            entity.EndDate = request.EndDate;
            entity.IsActive = request.IsActive;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDelegation(int id)
        {
            var entity = await _context.WorkflowDelegations.FindAsync(id);
            if (entity != null) { _context.WorkflowDelegations.Remove(entity); await _context.SaveChangesAsync(); }
        }

        // ── 12. Audit Logs ───────────────────────────────────────────

        public async Task<List<AuditLogResponse>> GetAllAuditLogs()
            => await _context.AuditLogs
                .Select(l => MapAuditLog(l))
                .ToListAsync();

        public async Task<List<AuditLogResponse>> GetAuditLogs(int instanceId)
            => await _context.AuditLogs
                .Where(l => l.InstanceId == instanceId)
                .Select(l => MapAuditLog(l))
                .ToListAsync();

        // ── 13. Designer Nodes ───────────────────────────────────────

        public async Task<List<DesignerNodeResponse>> GetAllDesignerNodes()
            => await _context.WorkflowDesignerNodes
                .Select(n => MapNode(n))
                .ToListAsync();

        public async Task<List<DesignerNodeResponse>> GetDesignerNodes(int workflowId)
            => await _context.WorkflowDesignerNodes
                .Where(n => n.WorkflowId == workflowId)
                .Select(n => MapNode(n))
                .ToListAsync();

        public async Task AddDesignerNode(DesignerNodeRequest request)
        {
            var entity = new WorkflowDesignerNode
            {
                NodeId = request.NodeId,
                WorkflowId = request.WorkflowId,
                NodeType = request.NodeType,
                Label = request.Label,
                PosX = request.PosX,
                PosY = request.PosY
            };
            _context.WorkflowDesignerNodes.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDesignerNode(string nodeId, DesignerNodeRequest request)
        {
            var entity = await _context.WorkflowDesignerNodes.FindAsync(nodeId);
            if (entity == null) return;
            entity.WorkflowId = request.WorkflowId;
            entity.NodeType = request.NodeType;
            entity.Label = request.Label;
            entity.PosX = request.PosX;
            entity.PosY = request.PosY;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDesignerNode(string nodeId)
        {
            var entity = await _context.WorkflowDesignerNodes.FindAsync(nodeId);
            if (entity != null) { _context.WorkflowDesignerNodes.Remove(entity); await _context.SaveChangesAsync(); }
        }

        // ── 14. Designer Edges ───────────────────────────────────────

        public async Task<List<DesignerEdgeResponse>> GetAllDesignerEdges()
            => await _context.WorkflowDesignerEdges
                .Select(e => MapEdge(e))
                .ToListAsync();

        public async Task<List<DesignerEdgeResponse>> GetDesignerEdges(int workflowId)
            => await _context.WorkflowDesignerEdges
                .Where(e => e.WorkflowId == workflowId)
                .Select(e => MapEdge(e))
                .ToListAsync();

        public async Task AddDesignerEdge(DesignerEdgeRequest request)
        {
            var entity = new WorkflowDesignerEdge
            {
                EdgeId = request.EdgeId,
                WorkflowId = request.WorkflowId,
                SourceNodeId = request.SourceNodeId,
                TargetNodeId = request.TargetNodeId,
                Label = request.Label
            };
            _context.WorkflowDesignerEdges.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDesignerEdge(string edgeId, DesignerEdgeRequest request)
        {
            var entity = await _context.WorkflowDesignerEdges.FindAsync(edgeId);
            if (entity == null) return;
            entity.WorkflowId = request.WorkflowId;
            entity.SourceNodeId = request.SourceNodeId;
            entity.TargetNodeId = request.TargetNodeId;
            entity.Label = request.Label;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDesignerEdge(string edgeId)
        {
            var entity = await _context.WorkflowDesignerEdges.FindAsync(edgeId);
            if (entity != null) { _context.WorkflowDesignerEdges.Remove(entity); await _context.SaveChangesAsync(); }
        }

        // ── Private mappers ──────────────────────────────────────────

        private static ApplicationResponse MapApplication(WorkflowApplication e) => new()
        {
            ApplicationId = e.ApplicationId,
            ApplicationCode = e.ApplicationCode,
            ApplicationName = e.ApplicationName,
            IsActive = e.IsActive
        };

        private static WorkflowResponse MapWorkflow(Workflow e) => new()
        {
            WorkflowId = e.WorkflowId,
            ApplicationId = e.ApplicationId,
            WorkflowName = e.WorkflowName,
            Version = e.Version,
            IsActive = e.IsActive
        };

        private static LevelResponse MapLevel(WorkflowLevel e, string workflowName = null) => new()
        {
            LevelId = e.LevelId,
            WorkflowId = e.WorkflowId,
            WorkflowName = workflowName,
            LevelNumber = e.LevelNumber,
            LevelName = e.LevelName,
            ApprovalStrategy = e.ApprovalStrategy,
            MinApprovalsRequired = e.MinApprovalsRequired
        };

        private static RuleResponse MapRule(WorkflowRule e) => new()
        {
            RuleId = e.RuleId,
            LevelId = e.LevelId,
            RuleExpression = e.RuleExpression,
            Priority = e.Priority,
            IsActive = e.IsActive
        };

        private static TransitionResponse MapTransition(WorkflowTransition e) => new()
        {
            TransitionId = e.TransitionId,
            WorkflowId = e.WorkflowId,
            FromLevel = e.FromLevel,
            ToLevel = e.ToLevel,
            Action = e.Action,
            TransitionType = e.TransitionType
        };

        private static InstanceResponse MapInstance(WorkflowInstance e) => new()
        {
            InstanceId = e.InstanceId,
            WorkflowId = e.WorkflowId,
            ApplicationCode = e.ApplicationCode,
            RequestId = e.RequestId,
            CurrentLevel = e.CurrentLevel,
            Status = e.Status,
            CreatedBy = e.CreatedBy,
            CreatedDate = e.CreatedDate,
            CompletedDate = e.CompletedDate
        };

        private static ParameterResponse MapParameter(WorkflowInstanceParameter e) => new()
        {
            Id = e.Id,
            InstanceId = e.InstanceId,
            ParameterName = e.ParameterName,
            ParameterValue = e.ParameterValue
        };

        private static AssignmentResponse MapAssignment(WorkflowAssignment e) => new()
        {
            AssignmentId = e.AssignmentId,
            InstanceId = e.InstanceId,
            LevelNumber = e.LevelNumber,
            ApproverUserId = e.ApproverUserId,
            Status = e.Status,
            AssignedDate = e.AssignedDate,
            ActionDate = e.ActionDate
        };

        private static HistoryResponse MapHistory(WorkflowHistory e) => new()
        {
            HistoryId = e.HistoryId,
            InstanceId = e.InstanceId,
            LevelNumber = e.LevelNumber,
            UserId = e.UserId,
            Action = e.Action,
            Remarks = e.Remarks,
            ActionDate = e.ActionDate
        };

        private static AuditLogResponse MapAuditLog(AuditLog e) => new()
        {
            LogId = e.LogId,
            InstanceId = e.InstanceId,
            ActionBy = e.ActionBy,
            LogDetails = e.LogDetails,
            Timestamp = e.Timestamp
        };

        private static DelegationResponse MapDelegation(WorkflowDelegation e) => new()
        {
            DelegationId = e.DelegationId,
            FromUserId = e.FromUserId,
            ToUserId = e.ToUserId,
            StartDate = e.StartDate,
            EndDate = e.EndDate,
            IsActive = e.IsActive,
            CreatedDate = e.CreatedDate
        };

        private static DesignerNodeResponse MapNode(WorkflowDesignerNode e) => new()
        {
            NodeId = e.NodeId,
            WorkflowId = e.WorkflowId,
            NodeType = e.NodeType,
            Label = e.Label,
            PosX = e.PosX,
            PosY = e.PosY
        };

        private static DesignerEdgeResponse MapEdge(WorkflowDesignerEdge e) => new()
        {
            EdgeId = e.EdgeId,
            WorkflowId = e.WorkflowId,
            SourceNodeId = e.SourceNodeId,
            TargetNodeId = e.TargetNodeId,
            Label = e.Label
        };
    }
}
