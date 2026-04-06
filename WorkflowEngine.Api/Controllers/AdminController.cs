using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkflowEngine.Application.Interfaces;
using WorkflowEngine.Domain.Entities;

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

        [HttpGet("applications")]
        public async Task<ActionResult<List<WorkflowApplication>>> GetApplications()
        {
            return await _service.GetApplications();
        }

        [HttpPost("applications")]
        public async Task<ActionResult<int>> CreateApplication(WorkflowApplication app)
        {
            var id = await _service.CreateApplication(app);
            return Ok(id);
        }

        [HttpGet("workflows/{applicationId}")]
        public async Task<ActionResult<List<Workflow>>> GetWorkflows(int applicationId)
        {
            return await _service.GetWorkflows(applicationId);
        }

        [HttpPost("workflows")]
        public async Task<ActionResult<int>> CreateWorkflow(Workflow workflow)
        {
            var id = await _service.CreateWorkflow(workflow);
            return Ok(id);
        }

        [HttpPost("levels")]
        public async Task<IActionResult> AddLevel(WorkflowLevel level)
        {
            await _service.AddLevel(level);
            return Ok();
        }

        [HttpPost("rules")]
        public async Task<IActionResult> AddRule(WorkflowRule rule)
        {
            await _service.AddRule(rule);
            return Ok();
        }

        [HttpPost("approvers")]
        public async Task<IActionResult> AddApprover(WorkflowRuleApprover approver)
        {
            await _service.AddApprover(approver);
            return Ok();
        }
    }
}
