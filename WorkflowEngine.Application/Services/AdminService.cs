using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

        public async Task<List<WorkflowApplication>> GetApplications()
        {
            return await _context.Applications.ToListAsync();
        }

        public async Task<int> CreateApplication(WorkflowApplication app)
        {
            _context.Applications.Add(app);
            await _context.SaveChangesAsync();
            return app.ApplicationId;
        }

        public async Task<List<Workflow>> GetWorkflows(int applicationId)
        {
            return await _context.Workflows
                .Where(w => w.ApplicationId == applicationId)
                .Include(w => w.Levels)
                    .ThenInclude(l => l.Rules)
                        .ThenInclude(r => r.Approvers)
                .ToListAsync();
        }

        public async Task<int> CreateWorkflow(Workflow workflow)
        {
            _context.Workflows.Add(workflow);
            await _context.SaveChangesAsync();
            return workflow.WorkflowId;
        }

        public async Task AddLevel(WorkflowLevel level)
        {
            _context.WorkflowLevels.Add(level);
            await _context.SaveChangesAsync();
        }

        public async Task AddRule(WorkflowRule rule)
        {
            _context.WorkflowRules.Add(rule);
            await _context.SaveChangesAsync();
        }

        public async Task AddApprover(WorkflowRuleApprover approver)
        {
            _context.WorkflowRuleApprovers.Add(approver);
            await _context.SaveChangesAsync();
        }
    }
}
