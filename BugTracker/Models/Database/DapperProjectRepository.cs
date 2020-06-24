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

		public BugReportComment CreateComment(BugReportComment bugReportComment)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var insertedCommentId = connection.ExecuteScalar("dbo.Comments_Insert", new
				{
					Author = bugReportComment.Author,
					Date = bugReportComment.Date,
					MainText = bugReportComment.MainText,
					BugReportId = bugReportComment.BugReportId
				},
					commandType: CommandType.StoredProcedure);
				BugReportComment insertedComment = connection.QueryFirst<BugReportComment>("dbo.Comments_GetById @BugReportCommentId", new { BugReportCommentId = insertedCommentId });
				return insertedComment;
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

		public IEnumerable<BugReport> GetAllBugReports(int projectId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var bugReports = connection.Query<BugReport>("dbo.BugReports_GetAll @ProjectId", new { ProjectId = projectId});
				return bugReports;
			}
		}

		public BugReport GetBugReportById(int bugReportId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var bugReport = connection.QueryFirst<BugReport>("dbo.BugReports_GetById @BugReportId", new { BugReportId = bugReportId });
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

		public Project GetProjectById(int id)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var project = connection.QueryFirst<Project>("dbo.Projects_GetById @ProjectId", new { ProjectId = id });
				return project;
			}
		}

		public BugReportComment GetBugReportCommentById(int bugReportCommentId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var comment = connection.QueryFirst<BugReportComment>("dbo.Comments_GetById @BugReportCommentId", new { BugReportCommentId = bugReportCommentId });
				return comment;
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

		public BugReport UpdateBugReport(BugReport bugReportChanges)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				connection.ExecuteScalar("dbo.BugReports_Update", new
				{
					BugReportId = bugReportChanges.BugReportId,
					Title = bugReportChanges.Title,
					ProgramBehaviour = bugReportChanges.ProgramBehaviour,
					DetailsToReproduce = bugReportChanges.DetailsToReproduce,
					Severity = bugReportChanges.Severity,
					Importance = bugReportChanges.Importance,
					Hidden = bugReportChanges.Hidden
				}, commandType: CommandType.StoredProcedure);
				BugReport updatedBugReport = connection.QueryFirst<BugReport>("dbo.BugReports_GetById @BugReportId", new { BugReportId = bugReportChanges.BugReportId });
				return updatedBugReport;
			}
		}

		public BugReportComment UpdateBugReportComment(BugReportComment bugReportCommentChanges)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				connection.Execute("dbo.Comments_Update", new
				{
					BugReportCommentId = bugReportCommentChanges.BugReportCommentId,
					Author = bugReportCommentChanges.Author,
					MainText = bugReportCommentChanges.MainText
				}, commandType: CommandType.StoredProcedure);
				BugReportComment updatedComment = connection.QueryFirst<BugReportComment>("dbo.Comments_GetById @BugReportCommentId", new { BugReportCommentId = bugReportCommentChanges.BugReportCommentId });
				return updatedComment;
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

		public void DeleteComment(int bugReportCommentId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				connection.Execute("dbo.Comments_DeleteById", new { BugReportCommentId = bugReportCommentId }, 
					commandType: CommandType.StoredProcedure);
			}
		}

		public int GetCommentParentId(int bugReportCommentId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var parentId = connection.QueryFirst<int>("dbo.Comments_GetParentId @BugReportCommentId", new { BugReportCommentId = bugReportCommentId });
				return parentId;
			}
		}

		public BugReport DeleteBugReport(int id)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var deletedBugReport = connection.QueryFirst<BugReport>("dbo.BugReports_GetById @BugReportId", new { BugReportId = id });
				connection.Execute("dbo.BugReports_DeleteById", new { BugReportId = id },
					commandType: CommandType.StoredProcedure);
				return deletedBugReport;
			}
		}

		public int GetCommentCountById(int bugReportId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				int count = connection.ExecuteScalar<int>("dbo.BugReports_CommentCount @BugReportId", new { BugReportId = bugReportId });
				return count;
			}
		}

		public BugState CreateBugState(BugState bugState)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var insertedBugStateId = connection.ExecuteScalar("dbo.BugStates_Insert", new
				{
					Author = bugState.Author,
					Time = bugState.Time,
					StateType = bugState.StateType,
					BugReportId = bugState.BugReportId
				},
					commandType: CommandType.StoredProcedure);
				BugState insertedState = connection.QueryFirst<BugState>("dbo.BugStates_GetById @BugStateId", new { BugStateId = insertedBugStateId });
				return insertedState;
			}
		}

		public BugState GetLatestState(int bugReportId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				BugState currentState = connection.QueryFirst<BugState>("dbo.BugStates_GetLatest @BugReportId", new { BugReportId = bugReportId });
				return currentState;
			}
		}

		public void CreateSubscription(int userId, int bugReportId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var insertedBugStateId = connection.ExecuteScalar("dbo.UserSubscriptions_Insert", new
				{
					UserId = userId,
					BugReportId = bugReportId
				},
					commandType: CommandType.StoredProcedure);
			}
		}

		public IEnumerable<BugReport> GetSubscribedReports(int userId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var bugReports = connection.Query<BugReport>("dbo.UserSubscriptions_GetAll @UserId", new { UserId = userId });
				return bugReports;
			}
		}

		public bool IsSubscribed(int userId, int bugReportId)
		{
			throw new NotImplementedException();
		}

		public void DeleteSubscription(int userId, int bugReportId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				connection.Execute("dbo.UserSubscriptions_Delete", new { UserId = userId, BugReportId = bugReportId },
					commandType: CommandType.StoredProcedure);
			}
		}
	}
}
