using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BugTracker.Migrations
{
    public partial class InitDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    LastUpdateTime = table.Column<DateTime>(nullable: false),
                    Hidden = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BugReport",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Hidden = table.Column<bool>(nullable: false),
                    ReportTime = table.Column<DateTime>(nullable: false),
                    Severity = table.Column<int>(nullable: false),
                    Importance = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    ProgramBehaviour = table.Column<string>(nullable: true),
                    DetailsToReproduce = table.Column<string>(nullable: true),
                    PersonReporting = table.Column<string>(nullable: true),
                    ProjectId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BugReport", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BugReport_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BugReportComment",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Author = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    MainText = table.Column<string>(nullable: true),
                    BugReportId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BugReportComment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BugReportComment_BugReport_BugReportId",
                        column: x => x.BugReportId,
                        principalTable: "BugReport",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BugState",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Time = table.Column<DateTime>(nullable: false),
                    Author = table.Column<string>(nullable: true),
                    StateName = table.Column<string>(nullable: true),
                    BugReportId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BugState", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BugState_BugReport_BugReportId",
                        column: x => x.BugReportId,
                        principalTable: "BugReport",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AttachmentPath",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Path = table.Column<string>(nullable: true),
                    BugReportCommentId = table.Column<int>(nullable: true),
                    BugReportId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttachmentPath", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttachmentPath_BugReportComment_BugReportCommentId",
                        column: x => x.BugReportCommentId,
                        principalTable: "BugReportComment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AttachmentPath_BugReport_BugReportId",
                        column: x => x.BugReportId,
                        principalTable: "BugReport",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttachmentPath_BugReportCommentId",
                table: "AttachmentPath",
                column: "BugReportCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_AttachmentPath_BugReportId",
                table: "AttachmentPath",
                column: "BugReportId");

            migrationBuilder.CreateIndex(
                name: "IX_BugReport_ProjectId",
                table: "BugReport",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_BugReportComment_BugReportId",
                table: "BugReportComment",
                column: "BugReportId");

            migrationBuilder.CreateIndex(
                name: "IX_BugState_BugReportId",
                table: "BugState",
                column: "BugReportId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttachmentPath");

            migrationBuilder.DropTable(
                name: "BugState");

            migrationBuilder.DropTable(
                name: "BugReportComment");

            migrationBuilder.DropTable(
                name: "BugReport");

            migrationBuilder.DropTable(
                name: "Projects");
        }
    }
}
