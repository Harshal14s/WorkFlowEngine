using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using WorkflowEngine.Domain.Base;

namespace WorkflowEngine.Domain.Entities
{
    public class Workflow : MetaFields
    {
        [Key]
        public int WorkflowId { get; set; }
        
        public int ApplicationId { get; set; }
        
        [MaxLength(200)]
        public string WorkflowName { get; set; }
        public int Version { get; set; }


        [ForeignKey("ApplicationId")]
        public WorkflowApplication? Application { get; set; }
        public ICollection<WorkflowLevel>? Levels { get; set; }
    }
}
