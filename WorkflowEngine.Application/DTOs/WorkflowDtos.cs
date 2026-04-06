using System.Collections.Generic;

namespace WorkflowEngine.Application.DTOs
{
    public class StartWorkflowRequest
    {
        public string ApplicationCode { get; set; }
        public string WorkflowName { get; set; }
        public string RequestId { get; set; }
        public string SubmittedBy { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
    }

    public class ApproveRequest
    {
        public int InstanceId { get; set; }
        public string UserId { get; set; }
        public string Remarks { get; set; }
    }
}
