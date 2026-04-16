using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using WorkflowEngine.Domain.Base;

namespace WorkflowEngine.Domain.Entities
{
    public class WorkflowDesignerNode : MetaFields
    {
        [Key]
        [MaxLength(100)]
        public string NodeId { get; set; }
        
        public int? WorkflowId { get; set; }
        
        [MaxLength(50)]
        public string NodeType { get; set; }
        
        [MaxLength(100)]
        public string Label { get; set; }
        
        public double? PosX { get; set; }
        public double? PosY { get; set; }

        [ForeignKey("WorkflowId")]
        public virtual Workflow Workflow { get; set; }
    }
}
