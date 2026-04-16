using System;
using System.Collections.Generic;

namespace WorkflowEngine.Application.DTOs
{
    // ═══════════════════════════════════════════════════════════════
    // DASHBOARD STATS
    // ═══════════════════════════════════════════════════════════════

    public class DashboardStatsResponse
    {
        public int TotalRequests { get; set; }
        public int PendingRequests { get; set; }
        public int ApprovedRequests { get; set; }
        public int RejectedRequests { get; set; }
        public List<RecentInstanceResponse> RecentInstances { get; set; }
    }

    public class RecentInstanceResponse
    {
        public int InstanceId { get; set; }
        public string RequestId { get; set; }
        public string ApplicationCode { get; set; }
        public string WorkflowName { get; set; }
        public string CreatedBy { get; set; }
        public int CurrentLevel { get; set; }
        public string WorkflowState { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    // ═══════════════════════════════════════════════════════════════
    // 1. APPLICATION DTOs
    // ═══════════════════════════════════════════════════════════════
    
    public class ApplicationRequest
    {
        public string ApplicationCode { get; set; }
        public string ApplicationName { get; set; }
        public bool IsActive { get; set; }
    }

    public class ApplicationResponse
    {
        public int ApplicationId { get; set; }
        public string ApplicationCode { get; set; }
        public string ApplicationName { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    // ═══════════════════════════════════════════════════════════════
    // 2. WORKFLOW DTOs
    // ═══════════════════════════════════════════════════════════════
    
    public class WorkflowRequest
    {
        public int ApplicationId { get; set; }
        public string WorkflowName { get; set; }
        public int Version { get; set; }
        public bool IsActive { get; set; }
    }

    public class WorkflowResponse
    {
        public int WorkflowId { get; set; }
        public int ApplicationId { get; set; }
        public string WorkflowName { get; set; }
        public int Version { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    // ═══════════════════════════════════════════════════════════════
    // 3. LEVEL DTOs
    // ═══════════════════════════════════════════════════════════════
    
    public class LevelRequest
    {
        public int WorkflowId { get; set; }
        public int LevelNumber { get; set; }
        public string LevelName { get; set; }
        public string ApprovalStrategy { get; set; }
        public int MinApprovalsRequired { get; set; }
    }

    public class LevelResponse
    {
        public int LevelId { get; set; }
        public int WorkflowId { get; set; }
        public string WorkflowName { get; set; }
        public int LevelNumber { get; set; }
        public string LevelName { get; set; }
        public string ApprovalStrategy { get; set; }
        public int MinApprovalsRequired { get; set; }
    }

    // ═══════════════════════════════════════════════════════════════
    // 4. RULE DTOs
    // ═══════════════════════════════════════════════════════════════
    
    public class RuleRequest
    {
        public int LevelId { get; set; }
        public string RuleExpression { get; set; }
        public int Priority { get; set; }
        public bool IsActive { get; set; }
    }

    public class RuleResponse
    {
        public int RuleId { get; set; }
        public int LevelId { get; set; }
        public string RuleExpression { get; set; }
        public int Priority { get; set; }
        public bool IsActive { get; set; }
    }

    // ═══════════════════════════════════════════════════════════════
    // 5. APPROVER DTOs
    // ═══════════════════════════════════════════════════════════════
    
    public class ApproverRequest
    {
        public int RuleId { get; set; }
        public string ApproverType { get; set; }
        public string ApproverValue { get; set; }
    }

    public class ApproverResponse
    {
        public int Id { get; set; }
        public int RuleId { get; set; }
        public string RuleExpression { get; set; }
        public int LevelId { get; set; }
        public string LevelName { get; set; }
        public int LevelNumber { get; set; }
        public int WorkflowId { get; set; }
        public string WorkflowName { get; set; }
        public string ApproverType { get; set; }
        public string ApproverValue { get; set; }
    }

    // ═══════════════════════════════════════════════════════════════
    // 6. TRANSITION DTOs
    // ═══════════════════════════════════════════════════════════════
    
    public class TransitionRequest
    {
        public int WorkflowId { get; set; }
        public int FromLevel { get; set; }
        public int ToLevel { get; set; }
        public string Action { get; set; }
        public string TransitionType { get; set; }
    }

    public class TransitionResponse
    {
        public int TransitionId { get; set; }
        public int WorkflowId { get; set; }
        public int FromLevel { get; set; }
        public int ToLevel { get; set; }
        public string Action { get; set; }
        public string TransitionType { get; set; }
    }

    // ═══════════════════════════════════════════════════════════════
    // 7. INSTANCE DTOs
    // ═══════════════════════════════════════════════════════════════
    
    public class InstanceRequest
    {
        public int WorkflowId { get; set; }
        public string ApplicationCode { get; set; }
        public string RequestId { get; set; }
        public int CurrentLevel { get; set; }
        public string WorkflowState { get; set; }
        public string CreatedBy { get; set; }
    }

    public class InstanceResponse
    {
        public int InstanceId { get; set; }
        public int WorkflowId { get; set; }
        public string ApplicationCode { get; set; }
        public string RequestId { get; set; }
        public int CurrentLevel { get; set; }
        public string WorkflowState { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
    }

    // ═══════════════════════════════════════════════════════════════
    // 8. PARAMETER DTOs
    // ═══════════════════════════════════════════════════════════════
    
    public class ParameterRequest
    {
        public int InstanceId { get; set; }
        public string ParameterName { get; set; }
        public string ParameterValue { get; set; }
    }

    public class ParameterResponse
    {
        public int Id { get; set; }
        public int InstanceId { get; set; }
        public string ParameterName { get; set; }
        public string ParameterValue { get; set; }
    }

    // ═══════════════════════════════════════════════════════════════
    // 9. ASSIGNMENT DTOs
    // ═══════════════════════════════════════════════════════════════
    
    public class AssignmentRequest
    {
        public int InstanceId { get; set; }
        public int LevelNumber { get; set; }
        public string ApproverUserId { get; set; }
        public string AssignmentStatus { get; set; }
    }

    public class AssignmentResponse
    {
        public int AssignmentId { get; set; }
        public int InstanceId { get; set; }
        public int LevelNumber { get; set; }
        public string ApproverUserId { get; set; }
        public string AssignmentStatus { get; set; }
        public DateTime AssignedDate { get; set; }
        public DateTime? ActionDate { get; set; }
    }

    // ═══════════════════════════════════════════════════════════════
    // 10. HISTORY DTOs
    // ═══════════════════════════════════════════════════════════════
    
    public class HistoryResponse
    {
        public int HistoryId { get; set; }
        public int InstanceId { get; set; }
        public int LevelNumber { get; set; }
        public string UserId { get; set; }
        public string Action { get; set; }
        public string Remarks { get; set; }
        public DateTime ActionDate { get; set; }
    }

    // ═══════════════════════════════════════════════════════════════
    // 11. AUDIT LOG DTOs
    // ═══════════════════════════════════════════════════════════════
    
    public class AuditLogResponse
    {
        public int LogId { get; set; }
        public int? InstanceId { get; set; }
        public string ActionBy { get; set; }
        public string LogDetails { get; set; }
        public DateTime Timestamp { get; set; }
    }

    // ═══════════════════════════════════════════════════════════════
    // 12. DELEGATION DTOs
    // ═══════════════════════════════════════════════════════════════
    
    public class DelegationRequest
    {
        public string FromUserId { get; set; }
        public string ToUserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }

    public class DelegationResponse
    {
        public int DelegationId { get; set; }
        public string FromUserId { get; set; }
        public string ToUserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    // ═══════════════════════════════════════════════════════════════
    // 13. DESIGNER NODE DTOs
    // ═══════════════════════════════════════════════════════════════
    
    public class DesignerNodeRequest
    {
        public string NodeId { get; set; }
        public int? WorkflowId { get; set; }
        public string NodeType { get; set; }
        public string Label { get; set; }
        public double? PosX { get; set; }
        public double? PosY { get; set; }
    }

    public class DesignerNodeResponse
    {
        public string NodeId { get; set; }
        public int? WorkflowId { get; set; }
        public string NodeType { get; set; }
        public string Label { get; set; }
        public double? PosX { get; set; }
        public double? PosY { get; set; }
    }

    // ═══════════════════════════════════════════════════════════════
    // 14. DESIGNER EDGE DTOs
    // ═══════════════════════════════════════════════════════════════
    
    public class DesignerEdgeRequest
    {
        public string EdgeId { get; set; }
        public int? WorkflowId { get; set; }
        public string SourceNodeId { get; set; }
        public string TargetNodeId { get; set; }
        public string Label { get; set; }
    }

    public class DesignerEdgeResponse
    {
        public string EdgeId { get; set; }
        public int? WorkflowId { get; set; }
        public string SourceNodeId { get; set; }
        public string TargetNodeId { get; set; }
        public string Label { get; set; }
    }
}
