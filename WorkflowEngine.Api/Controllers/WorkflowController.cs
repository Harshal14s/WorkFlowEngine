using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WorkflowEngine.Application.DTOs;
using WorkflowEngine.Application.Interfaces;

namespace WorkflowEngine.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkflowController : ControllerBase
    {
        private readonly IWorkflowService _service;

        public WorkflowController(IWorkflowService service)
        {
            _service = service;
        }   

        [HttpGet("pending/{userId}")]
        public async Task<IActionResult> GetPendingApprovals(string userId)
        {
            var result = await _service.GetPendingApprovals(userId);
            return Ok(result);
        }

        [HttpPost("start")]
        public async Task<IActionResult> Start(StartWorkflowRequest request)
        {
            try
            {
                var id = await _service.StartWorkflow(request);
                return Ok(new { InstanceId = id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("approve")]
        public async Task<IActionResult> Approve(ApproveRequest request)
        {
            try
            {
                await _service.Approve(request);
                return Ok(new { Message = "Approved successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("reject")]
        public async Task<IActionResult> Reject(ApproveRequest request)
        {
            try
            {
                await _service.Reject(request);
                return Ok(new { Message = "Rejected successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
