using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
	public class DapperProjectRepository : IProjectRepository
	{
		public Project CreateProject(Project project)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var insertedProjectId = connection.ExecuteScalar("dbo.Projects_Insert", new { 
					Name = project.Name, Description = project.Description, CreationTime = project.CreationTime, 
					LastUpdateTime = project.LastUpdateTime, Hidden = project.Hidden },
					commandType: CommandType.StoredProcedure);
				var insertedProject = connection.QueryFirst<Project>("dbo.Projects_GetById @ProjectId", new { ProjectId = insertedProjectId });
				return insertedProject;
			}
		}

		public BugReport AddBugReport(BugReport bugReport)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var insertedBugReportId = connection.ExecuteScalar("dbo.BugReports_Insert", new {
					Title = bugReport.Title, ProgramBehaviour = bugReport.ProgramBehaviour, DetailsToReproduce = bugReport.DetailsToReproduce, 
					CreationTime = bugReport.CreationTime, Severity = bugReport.Severity, Importance = bugReport.Importance, 
					PersonReporting = bugReport.PersonReporting, Hidden = bugReport.Hidden, ProjectId = bugReport.ProjectId },
					commandType: CommandType.StoredProcedure);
				BugReport insertedBugReport = connection.QueryFirst<BugReport>("dbo.BugReports_GetById @BugReportId", new { BugReportId = insertedBugReportId });
				return insertedBugReport;
			}
		}

		public Project DeleteProject(int Id)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var deletedProject = connection.QueryFirst<Project>("dbo.Projects_GetById @ProjectId", new { ProjectId = Id });
				var result = connection.Execute("dbo.Projects_Delete @ProjectId", new { ProjectId = Id });

				return deletedProject;
			}
		}

		public IEnumerable<BugReport> GetAllBugReports(int ProjectId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var bugReports = connection.Query<BugReport>("dbo.BugReports_GetAll @ProjectId", new { ProjectId = ProjectId});
				return bugReports;
			}
		}

		public BugReport GetBugReportById(int BugReportId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var bugReport = connection.QueryFirst<BugReport>("dbo.BugReports_GetById @BugReportId", new { BugReportId = BugReportId });
				return bugReport;
			}
		}

		public IEnumerable<Project> GetAllProjects()
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var projects = connection.Query<Project>("dbo.Projects_GetAll");
				return projects;
			}
		}

		public Project GetProjectById(int Id)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var project = connection.QueryFirst<Project>("dbo.Projects_GetById @ProjectId", new { ProjectId = Id });
				return project;
			}
		}

		public Project UpdateProject(Project projectChanges)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var projectId = connection.Execute("dbo.Projects_Update @ProjectId @Name @Description @CreationTime @LastUpdateTime @Hidden", projectChanges);
				var project = this.GetProjectById(projectId);
				return project;
			}
		}

		public BugReport UpdateBugReport(BugReport bugReportChanges)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var updatedBugReportId = connection.ExecuteScalar("dbo.BugReports_Update", new
				{
					Title = bugReportChanges.Title,
					ProgramBehaviour = bugReportChanges.ProgramBehaviour,
					DetailsToReproduce = bugReportChanges.DetailsToReproduce,
					Severity = bugReportChanges.Severity,
					Importance = bugReportChanges.Importance,
					Hidden = bugReportChanges.Hidden
				}, commandType: CommandType.StoredProcedure);
				BugReport updatedBugReport = connection.QueryFirst<BugReport>("dbo.BugReports_GetById @BugReportId", new { BugReportId = updatedBugReportId });
				return updatedBugReport;
			}
		}

		public IEnumerable<BugReportComment> GetBugReportComments(int bugReportId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var comments = connection.Query<BugReportComment>("dbo.Comments_GetAll @BugReportId", new { BugReportId = bugReportId});
				return comments;
			}
		}

		public IEnumerable<BugState> GetBugStates(int bugReportId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var bugStates = connection.Query<BugState>("dbo.BugStates_GetAll @BugReportId", new { BugReportId = bugReportId });
				return bugStates;
			}
		}

		public IEnumerable<AttachmentPath> GetAttachmentPaths(AttachmentParentType parentType, int parentId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				string procedure;

				switch (parentType)
				{
					case AttachmentParentType.BugReport:
						procedure = "dbo.AttachmentPaths_BugReport_GetAll @ParentId";
						break;
					case AttachmentParentType.Comment:
						procedure = "dbo.AttachmentPaths_Comment_GetAll @ParentId";
						break;
					default:
						throw new System.ArgumentException("Parameter must be a valid type", "parentType");
				}

				var attachmentPaths = connection.Query<AttachmentPath>(procedure, new { ParentId = parentId });
				return attachmentPaths;
			}
		}
	}
}
