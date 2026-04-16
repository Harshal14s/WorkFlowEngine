using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using WorkflowEngine.Domain.Base;

using WorkflowEngine.Domain.Enums;

namespace WorkflowEngine.Domain.Entities
{
    public class WorkflowTransition : MetaFields
    {
        [Key]
        public int TransitionId { get; set; }
        public int WorkflowId { get; set; }
        public int FromLevel { get; set; }
        public int ToLevel { get; set; }
        
        public WorkflowAction Action { get; set; }
        
        [MaxLength(20)]
        public string TransitionType { get; set; } // 'Next', 'Previous', 'Specific', 'Dynamic'

        [ForeignKey("WorkflowId")]
        public Workflow? Workflow { get; set; }
    }
}
