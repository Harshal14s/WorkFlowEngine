using System;
using System.ComponentModel.DataAnnotations;

namespace WorkflowEngine.Domain.Entities
{
    public class WorkflowDelegation
    {
        [Key]
        public int DelegationId { get; set; }
        
        [MaxLength(100)]
        public string FromUserId { get; set; }
        
        [MaxLength(100)]
        public string ToUserId { get; set; }
        
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
