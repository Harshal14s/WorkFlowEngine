using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkflowEngine.Domain.Entities
{
    public class WorkflowInstance
    {
        [Key]
        public int InstanceId { get; set; }
        public int WorkflowId { get; set; }
        
        [MaxLength(50)]
        public string ApplicationCode { get; set; }
        
        [MaxLength(100)]
        public string RequestId { get; set; }
        public int CurrentLevel { get; set; }
        
        [MaxLength(20)]
        public string Status { get; set; }
        
        [MaxLength(100)]
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? CompletedDate { get; set; }

        [ForeignKey("WorkflowId")]
        public Workflow Workflow { get; set; }
        public ICollection<WorkflowAssignment> Assignments { get; set; }
        public ICollection<WorkflowInstanceParameter> Parameters { get; set; }
        public ICollection<WorkflowHistory> History { get; set; }
    }
}
