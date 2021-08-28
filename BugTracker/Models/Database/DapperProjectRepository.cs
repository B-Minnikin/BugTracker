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

		public BugReport AddBugReport(BugReport bugReport)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var nextFreeId = connection.ExecuteScalar("dbo.LocalProjectBugReportIds_GetNextFreeId @ProjectId", new { ProjectId = bugReport.ProjectId });
				connection.Execute("dbo.LocalProjectBugReportIds_IncrementNextFreeId @ProjectId", new { ProjectId = bugReport.ProjectId });

				var insertedBugReportId = connection.ExecuteScalar("dbo.BugReports_Insert", new {
					Title = bugReport.Title, ProgramBehaviour = bugReport.ProgramBehaviour, DetailsToReproduce = bugReport.DetailsToReproduce,
					CreationTime = bugReport.CreationTime, Severity = bugReport.Severity, Importance = bugReport.Importance,
					PersonReporting = bugReport.PersonReporting, Hidden = bugReport.Hidden, ProjectId = bugReport.ProjectId,
					LocalBugReportId = nextFreeId },
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
				var bugReports = connection.Query<BugReport>("dbo.BugReports_GetAll @ProjectId", new { ProjectId = projectId });
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

		public BugReport GetBugReportByLocalId(int localBugReportId, int projectId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var bugReport = connection.QueryFirst<BugReport>("dbo.BugReports_GetByLocalId", new { LocalBugReportId = localBugReportId, ProjectId = projectId },
					commandType: CommandType.StoredProcedure);
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
				var comments = connection.Query<BugReportComment>("dbo.Comments_GetAll @BugReportId", new { BugReportId = bugReportId });
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

		public BugState GetBugStateById(int bugStateId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				BugState bugState = connection.QueryFirst<BugState>("dbo.BugStates_GetById @BugStateId", new { BugStateId = bugStateId });
				return bugState;
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
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var alreadySubscribed = connection.ExecuteScalar<bool>("dbo.UserSubscriptions_IsSubscribed", new { UserId = userId, BugReportId = bugReportId },
					commandType: CommandType.StoredProcedure);
				return alreadySubscribed;
			}
		}

		public void DeleteSubscription(int userId, int bugReportId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				connection.Execute("dbo.UserSubscriptions_Delete", new { UserId = userId, BugReportId = bugReportId },
					commandType: CommandType.StoredProcedure);
			}
		}

		public IEnumerable<int> GetAllSubscribedUserIds(int bugReportId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var userIds = connection.Query<int>("dbo.UserSubscriptions_GetUsersForReport", new { BugReportId = bugReportId },
					commandType: CommandType.StoredProcedure);
				return userIds;
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

		public Milestone AddMilestone(Milestone milestone)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				int insertedMilestoneId = (Int32)connection.ExecuteScalar("dbo.Milestones_Insert", new
				{
					ProjectId = milestone.ProjectId,
					Title = milestone.Title,
					Description = milestone.Description,
					CreationDate = milestone.CreationTime,
					DueDate = milestone.DueDate
				},
					commandType: CommandType.StoredProcedure);
				Milestone insertedMilestone = this.GetMilestoneById(insertedMilestoneId);
				return insertedMilestone;
			}
		}

		public Milestone DeleteMilestone(int milestoneId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var deletedMilestone = connection.QueryFirst<Milestone>("dbo.Milestones_GetById @MilestoneId", new { MilestoneId = milestoneId });
				connection.Execute("dbo.Milestones_DeleteById", new { MilestoneId = milestoneId },
					commandType: CommandType.StoredProcedure);

				return deletedMilestone;
			}
		}

		public Milestone UpdateMilestone(Milestone milestone)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var projectId = connection.ExecuteScalar("dbo.Milestones_Update", new
				{
					MilestoneId = milestone.MilestoneId,
					ProjectId = milestone.ProjectId,
					Title = milestone.Title,
					Description = milestone.Description,
					CreationDate = milestone.CreationTime,
					DueDate = milestone.DueDate
				}, commandType: CommandType.StoredProcedure);

				Milestone updatedMilestone = this.GetMilestoneById(milestone.MilestoneId);
				return updatedMilestone;
			}
		}

		public IEnumerable<Milestone> GetAllMilestones(int projectId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var milestones = connection.Query<Milestone>("dbo.Milestones_GetAll", new { ProjectId = projectId },
					commandType: CommandType.StoredProcedure);
				return milestones;
			}
		}

		public Milestone GetMilestoneById(int id)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var milestone = connection.QueryFirst<Milestone>("dbo.Milestones_GetById @MilestoneId", new { MilestoneId = id });
				return milestone;
			}
		}

		public void AddMilestoneBugReport(int milestoneId, int bugReportId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				connection.Execute("dbo.MilestoneBugReports_Insert", new { MilestoneId = milestoneId, BugReportId = bugReportId },
					commandType: CommandType.StoredProcedure);
			}
		}

		public void RemoveMilestoneBugReport(int milestoneId, int bugReportId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				connection.Execute("dbo.MilestoneBugReports_Delete", new { MilestoneId = milestoneId, BugReportId = bugReportId },
					commandType: CommandType.StoredProcedure);
			}
		}

		public IEnumerable<MilestoneBugReportEntry> GetMilestoneBugReportEntries(int milestoneId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var milestoneBugReportIds = connection.Query<MilestoneBugReportEntry>("dbo.MilestoneBugReports_GetAllReportEntries", new { MilestoneId = milestoneId },
					commandType: CommandType.StoredProcedure);
				return milestoneBugReportIds;
			}
		}

		public IEnumerable<BugReport> GetMilestoneBugReports(int milestoneId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var milestoneBugReports = connection.Query<BugReport>("dbo.MilestoneBugReports_GetAllReports", new { MilestoneId = milestoneId },
					commandType: CommandType.StoredProcedure);
				return milestoneBugReports;
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
					LinkedBugReportId = GetDerivedPropertyOrNull(activity, nameof(ActivityBugReportLink.SecondBugReportId)),
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
