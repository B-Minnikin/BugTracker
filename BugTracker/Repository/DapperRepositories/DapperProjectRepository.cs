using BugTracker.Extension_Methods;
using Dapper;
using Microsoft.AspNetCore.Identity;
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

		public Project DeleteProject(int id)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var deletedProject = connection.QueryFirst<Project>("dbo.Projects_GetById @ProjectId", new { ProjectId = id });
				var result = connection.Execute("dbo.Projects_Delete @ProjectId", new { ProjectId = id });

				return deletedProject;
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

		public Project GetProjectById(int id)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var project = connection.QueryFirst<Project>("dbo.Projects_GetById @ProjectId", new { ProjectId = id });
				return project;
			}
		}

		public Project UpdateProject(Project projectChanges)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var projectId = connection.Execute("dbo.Projects_Update", new
				{
					ProjectId = projectChanges.ProjectId,
					Name = projectChanges.Name,
					Description = projectChanges.Description,
					CreationTime = projectChanges.CreationTime,
					LastUpdateTime = projectChanges.LastUpdateTime,
					Hidden = projectChanges.Hidden
				}, commandType: CommandType.StoredProcedure);
				var project = this.GetProjectById(projectChanges.ProjectId);
				return project;
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

		public void CreateLocalBugReportId(int projectId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				connection.Execute("dbo.LocalProjectBugReportIds_Insert", new { ProjectId = projectId },
					commandType: CommandType.StoredProcedure);
			}
		}

		public IEnumerable<UserTypeaheadSearchResult> GetMatchingProjectMembersBySearchQuery(string query, int projectId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var searchResults = connection.Query<UserTypeaheadSearchResult>("dbo.Users_MatchByQueryAndProject", new { Query = query, ProjectId = projectId },
					commandType: CommandType.StoredProcedure);
				return searchResults;
			}
		}

		public IEnumerable<BugReportTypeaheadSearchResult> GetMatchingBugReportsByTitleSearchQuery(string query, int projectId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var searchResults = connection.Query<BugReportTypeaheadSearchResult>("dbo.BugReports_MatchByTitleQueryAndProject", new { Query = query, ProjectId = projectId },
					commandType: CommandType.StoredProcedure);
				return searchResults;
			}
		}

		public IEnumerable<BugReportTypeaheadSearchResult> GetMatchingBugReportsByLocalIdSearchQuery(int localBugReportId, int projectId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var searchResults = connection.Query<BugReportTypeaheadSearchResult>("dbo.BugReports_MatchByLocalIdAndProject", new { Query = localBugReportId, ProjectId = projectId },
					commandType: CommandType.StoredProcedure);
				return searchResults;
			}
		}

		public void AddUserAssignedToBugReport(int userId, int bugReportId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				connection.Execute("dbo.UsersAssignedToBugReport_Insert", new { UserId = userId, BugReportId = bugReportId },
					commandType: CommandType.StoredProcedure);
			}
		}

		public void RemoveUserAssignedToBugReport(int userId, int bugReportId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				connection.Execute("dbo.UsersAssignedToBugReport_Delete", new { UserId = userId, BugReportId = bugReportId },
					commandType: CommandType.StoredProcedure);
			}
		}

		public IEnumerable<BugReport> GetBugReportsForAssignedUser(int userId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var bugReports = connection.Query<BugReport>("dbo.UsersAssignedToBugReport_GetReportsForUser", new { UserId = userId },
					commandType: CommandType.StoredProcedure);
				return bugReports;
			}
		}

		public IEnumerable<IdentityUser> GetAssignedUsersForBugReport(int bugReportId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var users = connection.Query<IdentityUser>("dbo.UsersAssignedToBugReport_GetUsersForReport", new { BugReportId = bugReportId },
					commandType: CommandType.StoredProcedure);
				return users;
			}
		}

		public void AddBugReportLink(int bugReportId, int linkToBugReportId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				connection.Execute("dbo.BugReports_InsertLink", new { BugReportId = bugReportId, LinkToBugReportId = linkToBugReportId },
					commandType: CommandType.StoredProcedure);
			}
		}

		public void RemoveBugReportLink(int bugReportId, int linkToBugReportId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				connection.Execute("dbo.BugReports_DeleteLink", new { BugReportId = bugReportId, LinkToBugReportId = linkToBugReportId },
					commandType: CommandType.StoredProcedure);
			}
		}

		public IEnumerable<BugReport> GetLinkedReports(int bugReportId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var bugReports = connection.Query<BugReport>("dbo.BugReports_GetLinkedReports", new { BugReportId = bugReportId },
					commandType: CommandType.StoredProcedure);
				return bugReports;
			}
		}

		private int InsertActivityByTable(object fieldProperties, string activityTablePostfix)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				int insertedActivityId = (Int32)connection.ExecuteScalar($"dbo.Activity{ activityTablePostfix }_Insert", fieldProperties,
					commandType: CommandType.StoredProcedure);
				return insertedActivityId;
			}
		}
	}
}
