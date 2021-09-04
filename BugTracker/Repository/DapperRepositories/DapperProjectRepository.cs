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
				var comments = connection.Query<BugReportComment>("dbo.Comments_GetAll @BugReportId", new { BugReportId = bugReportId });
				return comments;
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

		public void CreatePendingProjectInvitation(string emailAddress, int projectId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				connection.Execute("dbo.ProjectInvitations_Insert", new { EmailAddress = emailAddress, ProjectId = projectId },
					commandType: CommandType.StoredProcedure);
			}
		}

		public void RemovePendingProjectInvitation(string emailAddress, int projectId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				connection.Execute("dbo.ProjectInvitations_Delete", new { EmailAddress = emailAddress, ProjectId = projectId },
					commandType: CommandType.StoredProcedure);
			}
		}

		public bool IsEmailAddressPendingRegistration(string emailAddress, int projectId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var pendingRegistration = connection.ExecuteScalar<bool>("dbo.ProjectInvitations_IsPendingRegistration", new { EmailAddress = emailAddress, ProjectId = projectId },
					commandType: CommandType.StoredProcedure);
				return pendingRegistration;
			}
		}

		public IEnumerable<int> GetProjectInvitationsForEmailAddress(string emailAddress)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var projectIds = connection.Query<int>("dbo.ProjectInvitations_GetInvitationsForEmailAddress", new { EmailAddress = emailAddress },
					commandType: CommandType.StoredProcedure);
				return projectIds;
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

		public void AddActivity(Activity activity)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var sql = @"INSERT INTO dbo.ActivityEvents (Timestamp, ProjectId, MessageId, UserId, BugReportId, AssigneeId, LinkedBugReportId, NewBugReportStateId, PreviousBugReportStateId, BugReportCommentId, MilestoneId)
					OUTPUT inserted.ActivityId 
					VALUES(@Timestamp, @ProjectId, @MessageId, @UserId, @BugReportId, @AssigneeId, @LinkedBugReportId, @NewBugReportStateId, @PreviousBugReportStateId, @BugReportCommentId, @MilestoneId)";
				var parameters = new { 
					Timestamp = DateTime.Now,
					ProjectId = activity.ProjectId,
					MessageId = activity.MessageId,
					UserId = activity.UserId,
					BugReportId = GetDerivedPropertyOrNull(activity, nameof(ActivityBugReport.BugReportId)), // sets to null if member does not exist
					AssigneeId = GetDerivedPropertyOrNull(activity, nameof(ActivityBugReportAssigned.AssigneeId)),
					LinkedBugReportId = GetDerivedPropertyOrNull(activity, nameof(ActivityBugReportLink.LinkedBugReportId)),
					NewBugReportStateId = GetDerivedPropertyOrNull(activity, nameof(ActivityBugReportStateChange.NewBugReportStateId)),
					PreviousBugReportStateId = GetDerivedPropertyOrNull(activity, nameof(ActivityBugReportStateChange.PreviousBugReportStateId)),
					BugReportCommentId = GetDerivedPropertyOrNull(activity, nameof(ActivityComment.BugReportCommentId)),
					MilestoneId = GetDerivedPropertyOrNull(activity, nameof(ActivityMilestone.MilestoneId))
				};

				connection.Execute(sql, parameters);
			}
		}

		private int? GetDerivedPropertyOrNull(Activity activity, string propertyName)
		{
			return activity.HasProperty(propertyName) ? activity.GetDerivedProperty<int?>(propertyName) : null;
		}

		public void RemoveActivity(int activityId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var sql = @"DELETE FROM dbo.ActivityEvents WHERE ActivityId = @activityId";
				var parameters = new
				{
					ActivityId = activityId
				};

				connection.Execute(sql, parameters);
			}
		}

		public IEnumerable<Activity> GetUserActivities(int userId)
		{
			return GetActivities(nameof(Activity.UserId), userId);
		}

		public IEnumerable<Activity> GetBugReportActivities(int bugReportId)
		{
			return GetActivities(nameof(ActivityBugReport.BugReportId), bugReportId);
		}

		private IEnumerable<Activity> GetActivities(string key, int id)
		{
			var activityEvents = new List<Activity>();

			var sql = $"SELECT * FROM ActivityEvents WHERE {key} = @Id;";
			var parameters = new { Key = key, Id = id.ToString()};

			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			using (var reader = connection.ExecuteReader(sql, parameters))
			{
				var activityProjectParser = reader.GetRowParser<ActivityProject>();
				var activityBugReportParser = reader.GetRowParser<ActivityBugReport>();
				var activityBugReportLinkParser = reader.GetRowParser<ActivityBugReportLink>();
				var activityBugReportStateChangeParser = reader.GetRowParser<ActivityBugReportStateChange>();
				var activityBugReportAssignedParser = reader.GetRowParser<ActivityBugReportAssigned>();
				var activityBugReportCommentParser = reader.GetRowParser<ActivityComment>();
				var activityMilestoneParser = reader.GetRowParser<ActivityMilestone>();
				var activityMilestoneBugReportParser = reader.GetRowParser<ActivityMilestoneBugReport>();

				while (reader.Read())
				{
					var discriminator = (ActivityMessage)reader.GetInt32(reader.GetOrdinal("MessageId"));
					switch (discriminator)
					{
						case ActivityMessage.ProjectCreated:
						case ActivityMessage.ProjectEdited:
							activityEvents.Add(activityProjectParser(reader));
							break;
						case ActivityMessage.BugReportPosted:
						case ActivityMessage.BugReportEdited:
							activityEvents.Add(activityBugReportParser(reader));
							break;
						case ActivityMessage.CommentPosted:
						case ActivityMessage.CommentEdited:
							activityEvents.Add(activityBugReportCommentParser(reader));
							break;
						case ActivityMessage.BugReportStateChanged:
							activityEvents.Add(activityBugReportStateChangeParser(reader));
							break;
						case ActivityMessage.BugReportsLinked:
							activityEvents.Add(activityBugReportLinkParser(reader));
							break;
						case ActivityMessage.BugReportAssignedToUser:
							activityEvents.Add(activityBugReportAssignedParser(reader));
							break;
						case ActivityMessage.MilestonePosted:
						case ActivityMessage.MilestoneEdited:
							activityEvents.Add(activityMilestoneParser(reader));
							break;
						case ActivityMessage.BugReportAddedToMilestone:
						case ActivityMessage.BugReportRemovedFromMilestone:
							activityEvents.Add(activityMilestoneBugReportParser(reader));
							break;
						default:
							break;
					}
				}

				return activityEvents;
			}
		}
	}
}
