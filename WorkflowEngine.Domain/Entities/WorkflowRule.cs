using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using WorkflowEngine.Domain.Base;

namespace WorkflowEngine.Domain.Entities
{
    public class WorkflowRule : MetaFields
    {
        [Key]
        public int RuleId { get; set; }
        public int LevelId { get; set; }
        
        [Required]
        public string RuleExpression { get; set; }
        public int Priority { get; set; }


        [ForeignKey("LevelId")]
        public WorkflowLevel? Level { get; set; }
        public ICollection<WorkflowRuleApprover>? Approvers { get; set; }
    }
}
