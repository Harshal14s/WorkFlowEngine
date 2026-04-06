using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkflowEngine.Domain.Entities
{
    public class WorkflowInstanceParameter
    {
        [Key]
        public int Id { get; set; }
        public int InstanceId { get; set; }
        
        [MaxLength(100)]
        public string ParameterName { get; set; }
        public string ParameterValue { get; set; }

        [ForeignKey("InstanceId")]
        public WorkflowInstance Instance { get; set; }
    }
}
