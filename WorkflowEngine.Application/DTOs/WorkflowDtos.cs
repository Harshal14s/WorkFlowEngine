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

    // ═══════════════════════════════════════════════════════════════
    // PENDING APPROVAL RESPONSE
    // ═══════════════════════════════════════════════════════════════

    public class PendingApprovalResponse
    {
        public int AssignmentId { get; set; }
        public int InstanceId { get; set; }
        public string RequestId { get; set; }
        public string ApplicationCode { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CurrentLevel { get; set; }
        public string InstanceStatus { get; set; }

        // From Workflow
        public int WorkflowId { get; set; }
        public string WorkflowName { get; set; }

        // From Assignment
        public int LevelNumber { get; set; }
        public string AssignmentStatus { get; set; }
        public DateTime AssignedDate { get; set; }

        // Instance parameters as key-value pairs
        public Dictionary<string, string> Parameters { get; set; }
    }
}
