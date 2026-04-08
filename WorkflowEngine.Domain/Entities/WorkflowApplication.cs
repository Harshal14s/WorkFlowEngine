using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WorkflowEngine.Domain.Entities
{
    public class WorkflowApplication
    {
        [Key]
        public int ApplicationId { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string ApplicationCode { get; set; }
        
        [MaxLength(200)]
        public string ApplicationName { get; set; }
        
        public bool IsActive { get; set; }
        public ICollection<Workflow>? Workflows { get; set; }
    }
}
