using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkflowEngine.Domain.Entities
{
    public class WorkflowRule
    {
        [Key]
        public int RuleId { get; set; }
        public int LevelId { get; set; }
        
        [Required]
        public string RuleExpression { get; set; }
        public int Priority { get; set; }
        public bool IsActive { get; set; }

        [ForeignKey("LevelId")]
        public WorkflowLevel Level { get; set; }
        public ICollection<WorkflowRuleApprover> Approvers { get; set; }
    }
}
