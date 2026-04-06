using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkflowEngine.Domain.Entities
{
    public class WorkflowHistory
    {
        [Key]
        public int HistoryId { get; set; }
        public int InstanceId { get; set; }
        public int LevelNumber { get; set; }
        
        [MaxLength(100)]
        public string UserId { get; set; }
        
        [MaxLength(20)]
        public string Action { get; set; }
        public string Remarks { get; set; }
        public DateTime ActionDate { get; set; }

        [ForeignKey("InstanceId")]
        public WorkflowInstance Instance { get; set; }
    }
}
