using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkflowEngine.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Applications",
                columns: table => new
                {
                    ApplicationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ApplicationName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.ApplicationId);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowDelegations",
                columns: table => new
                {
                    DelegationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromUserId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ToUserId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowDelegations", x => x.DelegationId);
                });

            migrationBuilder.CreateTable(
                name: "Workflows",
                columns: table => new
                {
                    WorkflowId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    WorkflowName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workflows", x => x.WorkflowId);
                    table.ForeignKey(
                        name: "FK_Workflows_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "ApplicationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowDesignerNodes",
                columns: table => new
                {
                    NodeId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    WorkflowId = table.Column<int>(type: "int", nullable: true),
                    NodeType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Label = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PosX = table.Column<double>(type: "float", nullable: true),
                    PosY = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowDesignerNodes", x => x.NodeId);
                    table.ForeignKey(
                        name: "FK_WorkflowDesignerNodes_Workflows_WorkflowId",
                        column: x => x.WorkflowId,
                        principalTable: "Workflows",
                        principalColumn: "WorkflowId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowInstances",
                columns: table => new
                {
                    InstanceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkflowId = table.Column<int>(type: "int", nullable: false),
                    ApplicationCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RequestId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CurrentLevel = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowInstances", x => x.InstanceId);
                    table.ForeignKey(
                        name: "FK_WorkflowInstances_Workflows_WorkflowId",
                        column: x => x.WorkflowId,
                        principalTable: "Workflows",
                        principalColumn: "WorkflowId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowLevels",
                columns: table => new
                {
                    LevelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkflowId = table.Column<int>(type: "int", nullable: false),
                    LevelNumber = table.Column<int>(type: "int", nullable: false),
                    LevelName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ApprovalStrategy = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MinApprovalsRequired = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowLevels", x => x.LevelId);
                    table.ForeignKey(
                        name: "FK_WorkflowLevels_Workflows_WorkflowId",
                        column: x => x.WorkflowId,
                        principalTable: "Workflows",
                        principalColumn: "WorkflowId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowTransitions",
                columns: table => new
                {
                    TransitionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkflowId = table.Column<int>(type: "int", nullable: false),
                    FromLevel = table.Column<int>(type: "int", nullable: false),
                    ToLevel = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TransitionType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowTransitions", x => x.TransitionId);
                    table.ForeignKey(
                        name: "FK_WorkflowTransitions_Workflows_WorkflowId",
                        column: x => x.WorkflowId,
                        principalTable: "Workflows",
                        principalColumn: "WorkflowId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowDesignerEdges",
                columns: table => new
                {
                    EdgeId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    WorkflowId = table.Column<int>(type: "int", nullable: true),
                    SourceNodeId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TargetNodeId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Label = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowDesignerEdges", x => x.EdgeId);
                    table.ForeignKey(
                        name: "FK_WorkflowDesignerEdges_WorkflowDesignerNodes_SourceNodeId",
                        column: x => x.SourceNodeId,
                        principalTable: "WorkflowDesignerNodes",
                        principalColumn: "NodeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkflowDesignerEdges_WorkflowDesignerNodes_TargetNodeId",
                        column: x => x.TargetNodeId,
                        principalTable: "WorkflowDesignerNodes",
                        principalColumn: "NodeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkflowDesignerEdges_Workflows_WorkflowId",
                        column: x => x.WorkflowId,
                        principalTable: "Workflows",
                        principalColumn: "WorkflowId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    LogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InstanceId = table.Column<int>(type: "int", nullable: true),
                    ActionBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LogDetails = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.LogId);
                    table.ForeignKey(
                        name: "FK_AuditLogs_WorkflowInstances_InstanceId",
                        column: x => x.InstanceId,
                        principalTable: "WorkflowInstances",
                        principalColumn: "InstanceId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowAssignments",
                columns: table => new
                {
                    AssignmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InstanceId = table.Column<int>(type: "int", nullable: false),
                    LevelNumber = table.Column<int>(type: "int", nullable: false),
                    ApproverUserId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AssignedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowAssignments", x => x.AssignmentId);
                    table.ForeignKey(
                        name: "FK_WorkflowAssignments_WorkflowInstances_InstanceId",
                        column: x => x.InstanceId,
                        principalTable: "WorkflowInstances",
                        principalColumn: "InstanceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowHistory",
                columns: table => new
                {
                    HistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InstanceId = table.Column<int>(type: "int", nullable: false),
                    LevelNumber = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Action = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowHistory", x => x.HistoryId);
                    table.ForeignKey(
                        name: "FK_WorkflowHistory_WorkflowInstances_InstanceId",
                        column: x => x.InstanceId,
                        principalTable: "WorkflowInstances",
                        principalColumn: "InstanceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowInstanceParameters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InstanceId = table.Column<int>(type: "int", nullable: false),
                    ParameterName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ParameterValue = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowInstanceParameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowInstanceParameters_WorkflowInstances_InstanceId",
                        column: x => x.InstanceId,
                        principalTable: "WorkflowInstances",
                        principalColumn: "InstanceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowRules",
                columns: table => new
                {
                    RuleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LevelId = table.Column<int>(type: "int", nullable: false),
                    RuleExpression = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowRules", x => x.RuleId);
                    table.ForeignKey(
                        name: "FK_WorkflowRules_WorkflowLevels_LevelId",
                        column: x => x.LevelId,
                        principalTable: "WorkflowLevels",
                        principalColumn: "LevelId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowRuleApprovers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RuleId = table.Column<int>(type: "int", nullable: false),
                    ApproverType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ApproverValue = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowRuleApprovers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowRuleApprovers_WorkflowRules_RuleId",
                        column: x => x.RuleId,
                        principalTable: "WorkflowRules",
                        principalColumn: "RuleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_InstanceId",
                table: "AuditLogs",
                column: "InstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowAssignments_InstanceId",
                table: "WorkflowAssignments",
                column: "InstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowDesignerEdges_SourceNodeId",
                table: "WorkflowDesignerEdges",
                column: "SourceNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowDesignerEdges_TargetNodeId",
                table: "WorkflowDesignerEdges",
                column: "TargetNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowDesignerEdges_WorkflowId",
                table: "WorkflowDesignerEdges",
                column: "WorkflowId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowDesignerNodes_WorkflowId",
                table: "WorkflowDesignerNodes",
                column: "WorkflowId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowHistory_InstanceId",
                table: "WorkflowHistory",
                column: "InstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstanceParameters_InstanceId",
                table: "WorkflowInstanceParameters",
                column: "InstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstances_WorkflowId",
                table: "WorkflowInstances",
                column: "WorkflowId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowLevels_WorkflowId",
                table: "WorkflowLevels",
                column: "WorkflowId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowRuleApprovers_RuleId",
                table: "WorkflowRuleApprovers",
                column: "RuleId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowRules_LevelId",
                table: "WorkflowRules",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_Workflows_ApplicationId",
                table: "Workflows",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowTransitions_WorkflowId",
                table: "WorkflowTransitions",
                column: "WorkflowId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "WorkflowAssignments");

            migrationBuilder.DropTable(
                name: "WorkflowDelegations");

            migrationBuilder.DropTable(
                name: "WorkflowDesignerEdges");

            migrationBuilder.DropTable(
                name: "WorkflowHistory");

            migrationBuilder.DropTable(
                name: "WorkflowInstanceParameters");

            migrationBuilder.DropTable(
                name: "WorkflowRuleApprovers");

            migrationBuilder.DropTable(
                name: "WorkflowTransitions");

            migrationBuilder.DropTable(
                name: "WorkflowDesignerNodes");

            migrationBuilder.DropTable(
                name: "WorkflowInstances");

            migrationBuilder.DropTable(
                name: "WorkflowRules");

            migrationBuilder.DropTable(
                name: "WorkflowLevels");

            migrationBuilder.DropTable(
                name: "Workflows");

            migrationBuilder.DropTable(
                name: "Applications");
        }
    }
}
