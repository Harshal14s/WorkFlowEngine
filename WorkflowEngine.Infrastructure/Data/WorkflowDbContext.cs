using Microsoft.EntityFrameworkCore;
using WorkflowEngine.Domain.Entities;

namespace WorkflowEngine.Infrastructure.Data
{
    public class WorkflowDbContext : DbContext
    {
        public WorkflowDbContext(DbContextOptions<WorkflowDbContext> options)
            : base(options)
        {
        }

        public DbSet<WorkflowApplication> Applications { get; set; }
        public DbSet<Workflow> Workflows { get; set; }
        public DbSet<WorkflowLevel> WorkflowLevels { get; set; }
        public DbSet<WorkflowRule> WorkflowRules { get; set; }
        public DbSet<WorkflowRuleApprover> WorkflowRuleApprovers { get; set; }
        public DbSet<WorkflowTransition> WorkflowTransitions { get; set; }
        public DbSet<WorkflowInstance> WorkflowInstances { get; set; }
        public DbSet<WorkflowInstanceParameter> WorkflowInstanceParameters { get; set; }
        public DbSet<WorkflowAssignment> WorkflowAssignments { get; set; }
        public DbSet<WorkflowHistory> WorkflowHistory { get; set; }
        public DbSet<WorkflowDelegation> WorkflowDelegations { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<WorkflowDesignerNode> WorkflowDesignerNodes { get; set; }
        public DbSet<WorkflowDesignerEdge> WorkflowDesignerEdges { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // WorkflowApplication -> Workflows
            modelBuilder.Entity<Workflow>()
                .HasOne(w => w.Application)
                .WithMany(a => a.Workflows)
                .HasForeignKey(w => w.ApplicationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Workflow -> WorkflowLevels
            modelBuilder.Entity<WorkflowLevel>()
                .HasOne(l => l.Workflow)
                .WithMany(w => w.Levels)
                .HasForeignKey(l => l.WorkflowId)
                .OnDelete(DeleteBehavior.Cascade);

            // Workflow -> WorkflowTransitions
            modelBuilder.Entity<WorkflowTransition>()
                .HasOne(t => t.Workflow)
                .WithMany()
                .HasForeignKey(t => t.WorkflowId)
                .OnDelete(DeleteBehavior.Cascade);

            // WorkflowLevel -> WorkflowRules
            modelBuilder.Entity<WorkflowRule>()
                .HasOne(r => r.Level)
                .WithMany(l => l.Rules)
                .HasForeignKey(r => r.LevelId)
                .OnDelete(DeleteBehavior.Cascade);

            // WorkflowRule -> WorkflowRuleApprovers
            modelBuilder.Entity<WorkflowRuleApprover>()
                .HasOne(a => a.Rule)
                .WithMany(r => r.Approvers)
                .HasForeignKey(a => a.RuleId)
                .OnDelete(DeleteBehavior.Cascade);

            // WorkflowInstance -> WorkflowInstanceParameters
            modelBuilder.Entity<WorkflowInstanceParameter>()
                .HasOne(p => p.Instance)
                .WithMany(i => i.Parameters)
                .HasForeignKey(p => p.InstanceId)
                .OnDelete(DeleteBehavior.Cascade);

            // WorkflowInstance -> WorkflowAssignments
            modelBuilder.Entity<WorkflowAssignment>()
                .HasOne(a => a.Instance)
                .WithMany(i => i.Assignments)
                .HasForeignKey(a => a.InstanceId)
                .OnDelete(DeleteBehavior.Cascade);

            // WorkflowInstance -> WorkflowHistory
            modelBuilder.Entity<WorkflowHistory>()
                .HasOne(h => h.Instance)
                .WithMany(i => i.History)
                .HasForeignKey(h => h.InstanceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Instance -> AuditLogs
            modelBuilder.Entity<AuditLog>()
                .HasOne(l => l.Instance)
                .WithMany()
                .HasForeignKey(l => l.InstanceId)
                .OnDelete(DeleteBehavior.SetNull);

            // Workflow -> WorkflowDesignerNodes
            modelBuilder.Entity<WorkflowDesignerNode>()
                .HasOne(n => n.Workflow)
                .WithMany()
                .HasForeignKey(n => n.WorkflowId)
                .OnDelete(DeleteBehavior.Cascade);

            // Workflow -> WorkflowDesignerEdges
            modelBuilder.Entity<WorkflowDesignerEdge>()
                .HasOne(e => e.Workflow)
                .WithMany()
                .HasForeignKey(e => e.WorkflowId)
                .OnDelete(DeleteBehavior.Cascade);

            // Designer Nodes -> Edges (Prevent Multiple Cascade Path Error)
            modelBuilder.Entity<WorkflowDesignerEdge>()
                .HasOne(e => e.SourceNode)
                .WithMany()
                .HasForeignKey(e => e.SourceNodeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<WorkflowDesignerEdge>()
                .HasOne(e => e.TargetNode)
                .WithMany()
                .HasForeignKey(e => e.TargetNodeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
