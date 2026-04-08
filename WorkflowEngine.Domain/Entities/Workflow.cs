using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkflowEngine.Domain.Entities
{
    public class Workflow
    {
        [Key]
        public int WorkflowId { get; set; }
        
        public int ApplicationId { get; set; }
        
        [MaxLength(200)]
        public string WorkflowName { get; set; }
        public int Version { get; set; }
        public bool IsActive { get; set; }

        [ForeignKey("ApplicationId")]
        public WorkflowApplication? Application { get; set; }
        public ICollection<WorkflowLevel>? Levels { get; set; }
    }
}
