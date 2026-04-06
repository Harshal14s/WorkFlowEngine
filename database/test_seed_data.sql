-- =============================================
-- TEST SEED DATA FOR ENTERPRISE WORKFLOW ENGINE
-- =============================================
-- This script populates your 14-table schema with realistic Enterprise Data.
-- It creates 1 Application ("Procurement"), 1 Workflow ("Purchase Order Approval"),
-- 2 Approval Levels, and 3 Rules with complex Microsoft RulesEngine expressions.

USE [WorkflowEngine]; -- Change this if your DB name is different!

-- 1. Create the Application (The Portal)
INSERT INTO Applications (ApplicationCode, ApplicationName, IsActive)
VALUES ('PROCURE', 'Global Procurement Portal', 1);

-- Get the ID of the inserted application
DECLARE @AppId INT = SCOPE_IDENTITY();

-- 2. Create the Workflow
INSERT INTO Workflows (ApplicationId, WorkflowName, Version, IsActive)
VALUES (@AppId, 'Purchase Order Approval Flow', 1, 1);

DECLARE @WorkflowId INT = SCOPE_IDENTITY();

-- 3. Create the Levels (Level 1: Manager, Level 2: VP)
INSERT INTO WorkflowLevels (WorkflowId, LevelNumber, LevelName, ApprovalStrategy, MinApprovalsRequired)
VALUES 
(@WorkflowId, 1, 'L1 - Manager Review', 'AnyOne', 1),
(@WorkflowId, 2, 'L2 - Executive Sign-Off', 'All', 2);

DECLARE @Level1Id INT = (SELECT LevelId FROM WorkflowLevels WHERE WorkflowId = @WorkflowId AND LevelNumber = 1);
DECLARE @Level2Id INT = (SELECT LevelId FROM WorkflowLevels WHERE WorkflowId = @WorkflowId AND LevelNumber = 2);

-- 4. Create Rules for Level 1 (Manager checks the amount)
-- Rule A: Low amount, Auto-approve or simple check
INSERT INTO WorkflowRules (LevelId, RuleExpression, Priority, IsActive)
VALUES (@Level1Id, 'Amount <= 5000', 1, 1);
DECLARE @Rule1A_Id INT = SCOPE_IDENTITY();

-- Rule B: High amount, requires Senior Manager
INSERT INTO WorkflowRules (LevelId, RuleExpression, Priority, IsActive)
VALUES (@Level1Id, 'Amount > 5000 AND Amount <= 50000', 2, 1);
DECLARE @Rule1B_Id INT = SCOPE_IDENTITY();

-- 5. Create Rules for Level 2 (VP checks massive orders and specifically cross-border)
INSERT INTO WorkflowRules (LevelId, RuleExpression, Priority, IsActive)
VALUES (@Level2Id, 'Amount > 50000 OR IsCrossBorder == true', 1, 1);
DECLARE @Rule2A_Id INT = SCOPE_IDENTITY();

-- 6. Add the Approvers for the specific rules
-- For Rule A (Low Amount) -> Bob can approve
INSERT INTO WorkflowRuleApprovers (RuleId, ApproverType, ApproverValue)
VALUES (@Rule1A_Id, 'User', 'Bob_Junior_Manager');

-- For Rule B (Mid Amount) -> Sarah can approve
INSERT INTO WorkflowRuleApprovers (RuleId, ApproverType, ApproverValue)
VALUES (@Rule1B_Id, 'User', 'Sarah_Senior_Manager');

-- For Level 2 (Massive/CrossBorder) -> Both VP of Finance AND VP of Ops must approve! (Strategy is "All")
INSERT INTO WorkflowRuleApprovers (RuleId, ApproverType, ApproverValue)
VALUES 
(@Rule2A_Id, 'User', 'VP_Finance_Alice'),
(@Rule2A_Id, 'User', 'VP_Operations_Mike');

-- 7. Add a Rejection Transition Rule
-- If Level 2 Rejects, send it all the way back to Level 1!
INSERT INTO WorkflowTransitions (WorkflowId, FromLevel, ToLevel, Action, TransitionType)
VALUES (@WorkflowId, 2, 1, 'Reject', 'System');

-- 8. Add an Active Delegation (Out of Office)
-- VP_Finance_Alice is on vacation, automatically assign to Director_Dave
INSERT INTO WorkflowDelegations (FromUserId, ToUserId, StartDate, EndDate, IsActive, CreatedDate)
VALUES ('VP_Finance_Alice', 'Director_Dave', GETUTCDATE(), DATEADD(day, 10, GETUTCDATE()), 1, GETUTCDATE());

PRINT 'Test Data Successfully Seeded!';
