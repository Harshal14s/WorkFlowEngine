using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkflowEngine.Domain.Entities
{
    public class WorkflowRuleApprover
    {
        [Key]
        public int Id { get; set; }
        public int RuleId { get; set; }
        
        [MaxLength(50)]
        public string ApproverType { get; set; }
        
        [MaxLength(200)]
        public string ApproverValue { get; set; }

        [ForeignKey("RuleId")]
        public WorkflowRule Rule { get; set; }
    }
}
