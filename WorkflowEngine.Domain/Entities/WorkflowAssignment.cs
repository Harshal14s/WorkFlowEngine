using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using WorkflowEngine.Domain.Base;

using WorkflowEngine.Domain.Enums;

namespace WorkflowEngine.Domain.Entities
{
    public class WorkflowAssignment : MetaFields
    {
        [Key]
        public int AssignmentId { get; set; }
        public int InstanceId { get; set; }
        public int LevelNumber { get; set; }
        
        [MaxLength(100)]
        public string ApproverUserId { get; set; }
        
        public WorkflowStatus AssignmentStatus { get; set; }

        public DateTime? ActionDate { get; set; }

        [ForeignKey("InstanceId")]
        public WorkflowInstance Instance { get; set; }
    }
}
