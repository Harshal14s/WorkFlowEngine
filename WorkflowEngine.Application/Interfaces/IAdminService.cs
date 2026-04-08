using System.Collections.Generic;
using System.Threading.Tasks;
using WorkflowEngine.Application.DTOs;

namespace WorkflowEngine.Application.Interfaces
{
    public interface IAdminService
    {
        // Dashboard
        Task<DashboardStatsResponse> GetDashboardStats();

        // 1. Applications
        Task<List<ApplicationResponse>> GetApplications();
        Task<ApplicationResponse> GetApplication(int id);
        Task<int> CreateApplication(ApplicationRequest request);
        Task UpdateApplication(int id, ApplicationRequest request);
        Task DeleteApplication(int id);

        // 2. Workflows
        Task<List<WorkflowResponse>> GetAllWorkflows();
        Task<List<WorkflowResponse>> GetWorkflows(int applicationId);
        Task<WorkflowResponse> GetWorkflow(int id);
        Task<int> CreateWorkflow(WorkflowRequest request);
        Task UpdateWorkflow(int id, WorkflowRequest request);
        Task DeleteWorkflow(int id);

        // 3. Levels
        Task<List<LevelResponse>> GetAllLevels();
        Task<List<LevelResponse>> GetWorkflowLevels(int workflowId);
        Task<LevelResponse> GetLevel(int id);
        Task AddLevel(LevelRequest request);
        Task UpdateLevel(int id, LevelRequest request);
        Task DeleteLevel(int id);

        // 4. Rules
        Task<List<RuleResponse>> GetAllRules();
        Task<List<RuleResponse>> GetWorkflowRules(int levelId);
        Task<RuleResponse> GetRule(int id);
        Task AddRule(RuleRequest request);
        Task UpdateRule(int id, RuleRequest request);
        Task DeleteRule(int id);

        // 5. Rule Approvers
        Task<List<ApproverResponse>> GetAllApprovers();
        Task<List<ApproverResponse>> GetWorkflowRuleApprovers(int ruleId);
        Task<ApproverResponse> GetApprover(int id);
        Task AddApprover(ApproverRequest request);
        Task UpdateApprover(int id, ApproverRequest request);
        Task DeleteApprover(int id);

        // 6. Transitions
        Task<List<TransitionResponse>> GetAllTransitions();
        Task<List<TransitionResponse>> GetWorkflowTransitions(int workflowId);
        Task<TransitionResponse> GetTransition(int id);
        Task AddTransition(TransitionRequest request);
        Task UpdateTransition(int id, TransitionRequest request);
        Task DeleteTransition(int id);

        // 7. Instances
        Task<List<InstanceResponse>> GetInstances();
        Task<InstanceResponse> GetInstance(int id);
        Task UpdateInstance(int id, InstanceRequest request);
        Task DeleteInstance(int id);

        // 8. Instance Parameters
        Task<List<ParameterResponse>> GetAllInstanceParameters();
        Task<List<ParameterResponse>> GetInstanceParameters(int instanceId);
        Task AddInstanceParameter(ParameterRequest request);
        Task DeleteInstanceParameter(int id);

        // 9. Assignments
        Task<List<AssignmentResponse>> GetAllAssignments();
        Task<List<AssignmentResponse>> GetAssignments(int instanceId);
        Task AddAssignment(AssignmentRequest request);
        Task UpdateAssignment(int id, AssignmentRequest request);
        Task DeleteAssignment(int id);

        // 10. History
        Task<List<HistoryResponse>> GetAllHistory();
        Task<List<HistoryResponse>> GetHistory(int instanceId);
        Task DeleteHistory(int id);

        // 11. Audit Logs
        Task<List<AuditLogResponse>> GetAllAuditLogs();
        Task<List<AuditLogResponse>> GetAuditLogs(int instanceId);

        // 12. Delegations
        Task<List<DelegationResponse>> GetDelegations();
        Task<DelegationResponse> GetDelegation(int id);
        Task<int> CreateDelegation(DelegationRequest request);
        Task UpdateDelegation(int id, DelegationRequest request);
        Task DeleteDelegation(int id);

        // 13. Designer Nodes
        Task<List<DesignerNodeResponse>> GetAllDesignerNodes();
        Task<List<DesignerNodeResponse>> GetDesignerNodes(int workflowId);
        Task AddDesignerNode(DesignerNodeRequest request);
        Task UpdateDesignerNode(string nodeId, DesignerNodeRequest request);
        Task DeleteDesignerNode(string nodeId);

        // 14. Designer Edges
        Task<List<DesignerEdgeResponse>> GetAllDesignerEdges();
        Task<List<DesignerEdgeResponse>> GetDesignerEdges(int workflowId);
        Task AddDesignerEdge(DesignerEdgeRequest request);
        Task UpdateDesignerEdge(string edgeId, DesignerEdgeRequest request);
        Task DeleteDesignerEdge(string edgeId);
    }
}
