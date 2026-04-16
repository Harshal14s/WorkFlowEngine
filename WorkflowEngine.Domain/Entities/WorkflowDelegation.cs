using System;
using System.ComponentModel.DataAnnotations;

using WorkflowEngine.Domain.Base;

namespace WorkflowEngine.Domain.Entities
{
    public class WorkflowDelegation : MetaFields
    {
        [Key]
        public int DelegationId { get; set; }
        
        [MaxLength(100)]
        public string FromUserId { get; set; }
        
        [MaxLength(100)]
        public string ToUserId { get; set; }
        
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        


    }
}
