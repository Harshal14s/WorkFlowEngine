-- =============================================
-- DELETE / CLEANUP SCRIPT FOR WORKFLOW ENGINE
-- =============================================
-- This script safely deletes all data from your 14 Workflow Engine tables.
-- It executes in reverse-dependency order to respect all Foreign Key Constraints.

USE [WorkflowEngine]; -- Change this if your DB name is different!

-- 1. Delete all Runtime Data (Child tables first)
PRINT 'Deleting runtime data...';
DELETE FROM AuditLogs;
DELETE FROM WorkflowHistory;
DELETE FROM WorkflowAssignments;
DELETE FROM WorkflowInstanceParameters;
DELETE FROM WorkflowInstances;

-- 2. Delete all Workflow Configuration Logic
PRINT 'Deleting workflow logic...';
DELETE FROM WorkflowRuleApprovers;
DELETE FROM WorkflowRules;
DELETE FROM WorkflowTransitions;
DELETE FROM WorkflowLevels;

-- 3. Delete Designer UI Nodes & Edges
PRINT 'Deleting designer metadata...';
DELETE FROM WorkflowDesignerEdges;
DELETE FROM WorkflowDesignerNodes;

-- 4. Delete Core Workflows and Applications
PRINT 'Deleting core applications...';
DELETE FROM Workflows;
DELETE FROM Applications;

-- 5. Delete Enterprise settings
PRINT 'Deleting delegations...';
DELETE FROM WorkflowDelegations;

PRINT 'All Workflow Data successfully deleted!';

-- OPTIONAL: If you want to reset the auto-increment Identity columns back to 1, uncomment the lines below:
-- DBCC CHECKIDENT ('WorkflowApplications', RESEED, 0);
-- DBCC CHECKIDENT ('Workflows', RESEED, 0);
-- DBCC CHECKIDENT ('WorkflowLevels', RESEED, 0);
-- DBCC CHECKIDENT ('WorkflowRules', RESEED, 0);
-- DBCC CHECKIDENT ('WorkflowInstances', RESEED, 0);
-- DBCC CHECKIDENT ('WorkflowRuleApprovers', RESEED, 0);
