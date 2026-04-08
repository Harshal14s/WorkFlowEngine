using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkflowEngine.Application.DTOs;
using WorkflowEngine.Application.Interfaces;

namespace WorkflowEngine.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _service;

        public AdminController(IAdminService service)
        {
            _service = service;
        }

        // ── Dashboard Stats ──────────────────────────────────────────

        [HttpGet("stats")]
        public async Task<ActionResult<DashboardStatsResponse>> GetStats()
            => Ok(await _service.GetDashboardStats());

        // ── 1. Applications ──────────────────────────────────────────

        [HttpGet("applications")]
        public async Task<ActionResult<List<ApplicationResponse>>> GetApplications()
            => Ok(await _service.GetApplications());

        [HttpGet("applications/{id}")]
        public async Task<ActionResult<ApplicationResponse>> GetApplication(int id)
            => Ok(await _service.GetApplication(id));

        [HttpPost("applications")]
        public async Task<ActionResult<int>> CreateApplication(ApplicationRequest request)
            => Ok(await _service.CreateApplication(request));

        [HttpPut("applications/{id}")]
        public async Task<IActionResult> UpdateApplication(int id, ApplicationRequest request)
        {
            await _service.UpdateApplication(id, request);
            return Ok();
        }

        [HttpDelete("applications/{id}")]
        public async Task<IActionResult> DeleteApplication(int id)
        {
            await _service.DeleteApplication(id);
            return Ok();
        }

        // ── 2. Workflows ─────────────────────────────────────────────

        [HttpGet("workflows")]
        public async Task<ActionResult<List<WorkflowResponse>>> GetAllWorkflows()
            => Ok(await _service.GetAllWorkflows());

        [HttpGet("workflows/app/{applicationId}")]
        public async Task<ActionResult<List<WorkflowResponse>>> GetWorkflows(int applicationId)
            => Ok(await _service.GetWorkflows(applicationId));

        [HttpGet("workflows/{id}")]
        public async Task<ActionResult<WorkflowResponse>> GetWorkflow(int id)
            => Ok(await _service.GetWorkflow(id));

        [HttpPost("workflows")]
        public async Task<ActionResult<int>> CreateWorkflow(WorkflowRequest request)
            => Ok(await _service.CreateWorkflow(request));

        [HttpPut("workflows/{id}")]
        public async Task<IActionResult> UpdateWorkflow(int id, WorkflowRequest request)
        {
            await _service.UpdateWorkflow(id, request);
            return Ok();
        }

        [HttpDelete("workflows/{id}")]
        public async Task<IActionResult> DeleteWorkflow(int id)
        {
            await _service.DeleteWorkflow(id);
            return Ok();
        }

        // ── 3. Levels ────────────────────────────────────────────────

        [HttpGet("levels")]
        public async Task<ActionResult<List<LevelResponse>>> GetAllLevels()
            => Ok(await _service.GetAllLevels());

        [HttpGet("levels/workflow/{workflowId}")]
        public async Task<ActionResult<List<LevelResponse>>> GetLevels(int workflowId)
            => Ok(await _service.GetWorkflowLevels(workflowId));

        [HttpGet("levels/{id}")]
        public async Task<ActionResult<LevelResponse>> GetLevel(int id)
            => Ok(await _service.GetLevel(id));

        [HttpPost("levels")]
        public async Task<IActionResult> AddLevel(LevelRequest request)
        {
            await _service.AddLevel(request);
            return Ok();
        }

        [HttpPut("levels/{id}")]
        public async Task<IActionResult> UpdateLevel(int id, LevelRequest request)
        {
            await _service.UpdateLevel(id, request);
            return Ok();
        }

        [HttpDelete("levels/{id}")]
        public async Task<IActionResult> DeleteLevel(int id)
        {
            await _service.DeleteLevel(id);
            return Ok();
        }

        // ── 4. Rules ─────────────────────────────────────────────────

        [HttpGet("rules")]
        public async Task<ActionResult<List<RuleResponse>>> GetAllRules()
            => Ok(await _service.GetAllRules());

        [HttpGet("rules/level/{levelId}")]
        public async Task<ActionResult<List<RuleResponse>>> GetRules(int levelId)
            => Ok(await _service.GetWorkflowRules(levelId));

        [HttpGet("rules/{id}")]
        public async Task<ActionResult<RuleResponse>> GetRule(int id)
            => Ok(await _service.GetRule(id));

        [HttpPost("rules")]
        public async Task<IActionResult> AddRule(RuleRequest request)
        {
            await _service.AddRule(request);
            return Ok();
        }

        [HttpPut("rules/{id}")]
        public async Task<IActionResult> UpdateRule(int id, RuleRequest request)
        {
            await _service.UpdateRule(id, request);
            return Ok();
        }

        [HttpDelete("rules/{id}")]
        public async Task<IActionResult> DeleteRule(int id)
        {
            await _service.DeleteRule(id);
            return Ok();
        }

        // ── 5. Approvers ─────────────────────────────────────────────

        [HttpGet("approvers")]
        public async Task<ActionResult<List<ApproverResponse>>> GetAllApprovers()
            => Ok(await _service.GetAllApprovers());

        [HttpGet("approvers/rule/{ruleId}")]
        public async Task<ActionResult<List<ApproverResponse>>> GetApprovers(int ruleId)
            => Ok(await _service.GetWorkflowRuleApprovers(ruleId));

        [HttpGet("approvers/{id}")]
        public async Task<ActionResult<ApproverResponse>> GetApprover(int id)
            => Ok(await _service.GetApprover(id));

        [HttpPost("approvers")]
        public async Task<IActionResult> AddApprover(ApproverRequest request)
        {
            await _service.AddApprover(request);
            return Ok();
        }

        [HttpPut("approvers/{id}")]
        public async Task<IActionResult> UpdateApprover(int id, ApproverRequest request)
        {
            await _service.UpdateApprover(id, request);
            return Ok();
        }

        [HttpDelete("approvers/{id}")]
        public async Task<IActionResult> DeleteApprover(int id)
        {
            await _service.DeleteApprover(id);
            return Ok();
        }

        // ── 6. Transitions ───────────────────────────────────────────

        [HttpGet("transitions")]
        public async Task<ActionResult<List<TransitionResponse>>> GetAllTransitions()
            => Ok(await _service.GetAllTransitions());

        [HttpGet("transitions/workflow/{workflowId}")]
        public async Task<ActionResult<List<TransitionResponse>>> GetTransitions(int workflowId)
            => Ok(await _service.GetWorkflowTransitions(workflowId));

        [HttpGet("transitions/{id}")]
        public async Task<ActionResult<TransitionResponse>> GetTransition(int id)
            => Ok(await _service.GetTransition(id));

        [HttpPost("transitions")]
        public async Task<IActionResult> AddTransition(TransitionRequest request)
        {
            await _service.AddTransition(request);
            return Ok();
        }

        [HttpPut("transitions/{id}")]
        public async Task<IActionResult> UpdateTransition(int id, TransitionRequest request)
        {
            await _service.UpdateTransition(id, request);
            return Ok();
        }

        [HttpDelete("transitions/{id}")]
        public async Task<IActionResult> DeleteTransition(int id)
        {
            await _service.DeleteTransition(id);
            return Ok();
        }

        // ── 7. Instances ─────────────────────────────────────────────

        [HttpGet("instances")]
        public async Task<ActionResult<List<InstanceResponse>>> GetInstances()
            => Ok(await _service.GetInstances());

        [HttpGet("instances/{id}")]
        public async Task<ActionResult<InstanceResponse>> GetInstance(int id)
            => Ok(await _service.GetInstance(id));

        [HttpPut("instances/{id}")]
        public async Task<IActionResult> UpdateInstance(int id, InstanceRequest request)
        {
            await _service.UpdateInstance(id, request);
            return Ok();
        }

        [HttpDelete("instances/{id}")]
        public async Task<IActionResult> DeleteInstance(int id)
        {
            await _service.DeleteInstance(id);
            return Ok();
        }

        // ── 8. Parameters ────────────────────────────────────────────

        [HttpGet("parameters")]
        public async Task<ActionResult<List<ParameterResponse>>> GetAllParams()
            => Ok(await _service.GetAllInstanceParameters());

        [HttpGet("instances/{instanceId}/parameters")]
        public async Task<ActionResult<List<ParameterResponse>>> GetParams(int instanceId)
            => Ok(await _service.GetInstanceParameters(instanceId));

        [HttpPost("parameters")]
        public async Task<IActionResult> AddParameter(ParameterRequest request)
        {
            await _service.AddInstanceParameter(request);
            return Ok();
        }

        [HttpDelete("parameters/{id}")]
        public async Task<IActionResult> DeleteParameter(int id)
        {
            await _service.DeleteInstanceParameter(id);
            return Ok();
        }

        // ── 9. Assignments ───────────────────────────────────────────

        [HttpGet("assignments")]
        public async Task<ActionResult<List<AssignmentResponse>>> GetAllAssignments()
            => Ok(await _service.GetAllAssignments());

        [HttpGet("instances/{instanceId}/assignments")]
        public async Task<ActionResult<List<AssignmentResponse>>> GetAssignments(int instanceId)
            => Ok(await _service.GetAssignments(instanceId));

        [HttpPost("assignments")]
        public async Task<IActionResult> AddAssignment(AssignmentRequest request)
        {
            await _service.AddAssignment(request);
            return Ok();
        }

        [HttpPut("assignments/{id}")]
        public async Task<IActionResult> UpdateAssignment(int id, AssignmentRequest request)
        {
            await _service.UpdateAssignment(id, request);
            return Ok();
        }

        [HttpDelete("assignments/{id}")]
        public async Task<IActionResult> DeleteAssignment(int id)
        {
            await _service.DeleteAssignment(id);
            return Ok();
        }

        // ── 10. History ──────────────────────────────────────────────

        [HttpGet("history")]
        public async Task<ActionResult<List<HistoryResponse>>> GetAllHistory()
            => Ok(await _service.GetAllHistory());

        [HttpGet("instances/{instanceId}/history")]
        public async Task<ActionResult<List<HistoryResponse>>> GetHistory(int instanceId)
            => Ok(await _service.GetHistory(instanceId));

        [HttpDelete("history/{id}")]
        public async Task<IActionResult> DeleteHistory(int id)
        {
            await _service.DeleteHistory(id);
            return Ok();
        }

        // ── 11. Audit Logs ───────────────────────────────────────────

        [HttpGet("audit")]
        public async Task<ActionResult<List<AuditLogResponse>>> GetAllAudit()
            => Ok(await _service.GetAllAuditLogs());

        [HttpGet("instances/{instanceId}/audit")]
        public async Task<ActionResult<List<AuditLogResponse>>> GetAudit(int instanceId)
            => Ok(await _service.GetAuditLogs(instanceId));

        // ── 12. Delegations ──────────────────────────────────────────

        [HttpGet("delegations")]
        public async Task<ActionResult<List<DelegationResponse>>> GetDelegations()
            => Ok(await _service.GetDelegations());

        [HttpGet("delegations/{id}")]
        public async Task<ActionResult<DelegationResponse>> GetDelegation(int id)
            => Ok(await _service.GetDelegation(id));

        [HttpPost("delegations")]
        public async Task<ActionResult<int>> CreateDelegation(DelegationRequest request)
            => Ok(await _service.CreateDelegation(request));

        [HttpPut("delegations/{id}")]
        public async Task<IActionResult> UpdateDelegation(int id, DelegationRequest request)
        {
            await _service.UpdateDelegation(id, request);
            return Ok();
        }

        [HttpDelete("delegations/{id}")]
        public async Task<IActionResult> DeleteDelegation(int id)
        {
            await _service.DeleteDelegation(id);
            return Ok();
        }

        // ── 13. Designer Nodes ───────────────────────────────────────

        [HttpGet("designer/nodes")]
        public async Task<ActionResult<List<DesignerNodeResponse>>> GetAllNodes()
            => Ok(await _service.GetAllDesignerNodes());

        [HttpGet("designer/workflow/{workflowId}/nodes")]
        public async Task<ActionResult<List<DesignerNodeResponse>>> GetNodes(int workflowId)
            => Ok(await _service.GetDesignerNodes(workflowId));

        [HttpPost("designer/nodes")]
        public async Task<IActionResult> AddNode(DesignerNodeRequest request)
        {
            await _service.AddDesignerNode(request);
            return Ok();
        }

        [HttpPut("designer/nodes/{nodeId}")]
        public async Task<IActionResult> UpdateNode(string nodeId, DesignerNodeRequest request)
        {
            await _service.UpdateDesignerNode(nodeId, request);
            return Ok();
        }

        [HttpDelete("designer/nodes/{nodeId}")]
        public async Task<IActionResult> DeleteNode(string nodeId)
        {
            await _service.DeleteDesignerNode(nodeId);
            return Ok();
        }

        // ── 14. Designer Edges ───────────────────────────────────────

        [HttpGet("designer/edges")]
        public async Task<ActionResult<List<DesignerEdgeResponse>>> GetAllEdges()
            => Ok(await _service.GetAllDesignerEdges());

        [HttpGet("designer/workflow/{workflowId}/edges")]
        public async Task<ActionResult<List<DesignerEdgeResponse>>> GetEdges(int workflowId)
            => Ok(await _service.GetDesignerEdges(workflowId));

        [HttpPost("designer/edges")]
        public async Task<IActionResult> AddEdge(DesignerEdgeRequest request)
        {
            await _service.AddDesignerEdge(request);
            return Ok();
        }

        [HttpPut("designer/edges/{edgeId}")]
        public async Task<IActionResult> UpdateEdge(string edgeId, DesignerEdgeRequest request)
        {
            await _service.UpdateDesignerEdge(edgeId, request);
            return Ok();
        }

        [HttpDelete("designer/edges/{edgeId}")]
        public async Task<IActionResult> DeleteEdge(string edgeId)
        {
            await _service.DeleteDesignerEdge(edgeId);
            return Ok();
        }
    }
}
