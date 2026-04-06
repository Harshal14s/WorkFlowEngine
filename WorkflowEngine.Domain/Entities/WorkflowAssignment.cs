using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkflowEngine.Domain.Entities
{
    public class WorkflowAssignment
    {
        [Key]
        public int AssignmentId { get; set; }
        public int InstanceId { get; set; }
        public int LevelNumber { get; set; }
        
        [MaxLength(100)]
        public string ApproverUserId { get; set; }
        
        [MaxLength(20)]
        public string Status { get; set; }
        public DateTime AssignedDate { get; set; }
        public DateTime? ActionDate { get; set; }

        [ForeignKey("InstanceId")]
        public WorkflowInstance Instance { get; set; }
    }
}
