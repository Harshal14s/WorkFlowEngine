using System.Collections.Generic;
using System.Threading.Tasks;
using WorkflowEngine.Domain.Entities;

namespace WorkflowEngine.Application.Interfaces
{
    public interface IAdminService
    {
        // Application Management
        Task<List<WorkflowApplication>> GetApplications();
        Task<int> CreateApplication(WorkflowApplication app);

        // Workflow Management
        Task<List<Workflow>> GetWorkflows(int applicationId);
        Task<int> CreateWorkflow(Workflow workflow);

        // Configuration Management
        Task AddLevel(WorkflowLevel level);
        Task AddRule(WorkflowRule rule);
        Task AddApprover(WorkflowRuleApprover approver);
    }
}
