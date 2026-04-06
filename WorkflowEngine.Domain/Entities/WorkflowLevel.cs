using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkflowEngine.Domain.Entities
{
    public class WorkflowLevel
    {
        [Key]
        public int LevelId { get; set; }
        public int WorkflowId { get; set; }
        public int LevelNumber { get; set; }
        
        [MaxLength(200)]
        public string LevelName { get; set; }
        
        [MaxLength(20)]
        public string ApprovalStrategy { get; set; } // 'Single', 'AnyOne', 'All', 'Majority'
        
        public int MinApprovalsRequired { get; set; }

        [ForeignKey("WorkflowId")]
        public Workflow Workflow { get; set; }
        public ICollection<WorkflowRule> Rules { get; set; }
    }
}
