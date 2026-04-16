using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using WorkflowEngine.Domain.Base;

namespace WorkflowEngine.Domain.Entities
{
    public class AuditLog : MetaFields
    {
        [Key]
        public int LogId { get; set; }
        public int? InstanceId { get; set; }
        
        [MaxLength(100)]
        public string ActionBy { get; set; }
        
        public string LogDetails { get; set; }
        public DateTime Timestamp { get; set; }

        [ForeignKey("InstanceId")]
        public WorkflowInstance Instance { get; set; }
    }
}
