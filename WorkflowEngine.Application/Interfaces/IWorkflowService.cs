using System.Collections.Generic;
using System.Threading.Tasks;
using WorkflowEngine.Application.DTOs;

namespace WorkflowEngine.Application.Interfaces
{
    public interface IWorkflowService
    {
        Task<int> StartWorkflow(StartWorkflowRequest request);
        Task Approve(ApproveRequest request);
        Task Reject(ApproveRequest request);
        Task<List<PendingApprovalResponse>> GetPendingApprovals(string userId);
    }
}
