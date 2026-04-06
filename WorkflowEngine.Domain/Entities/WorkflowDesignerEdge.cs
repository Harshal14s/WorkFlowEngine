using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkflowEngine.Domain.Entities
{
    public class WorkflowDesignerEdge
    {
        [Key]
        [MaxLength(100)]
        public string EdgeId { get; set; }
        
        public int? WorkflowId { get; set; }
        
        [MaxLength(100)]
        public string SourceNodeId { get; set; }
        
        [MaxLength(100)]
        public string TargetNodeId { get; set; }
        
        [MaxLength(100)]
        public string Label { get; set; }

        [ForeignKey("WorkflowId")]
        public virtual Workflow Workflow { get; set; }

        [ForeignKey("SourceNodeId")]
        public virtual WorkflowDesignerNode SourceNode { get; set; }

        [ForeignKey("TargetNodeId")]
        public virtual WorkflowDesignerNode TargetNode { get; set; }
    }
}
