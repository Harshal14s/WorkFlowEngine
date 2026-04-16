using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using WorkflowEngine.Domain.Base;

using WorkflowEngine.Domain.Enums;

namespace WorkflowEngine.Domain.Entities
{
    public class WorkflowHistory : MetaFields
    {
        [Key]
        public int HistoryId { get; set; }
        public int InstanceId { get; set; }
        public int LevelNumber { get; set; }
        
        [MaxLength(100)]
        public string UserId { get; set; }
        
        public WorkflowAction Action { get; set; }
        public string Remarks { get; set; }


        [ForeignKey("InstanceId")]
        public WorkflowInstance Instance { get; set; }
    }
}
