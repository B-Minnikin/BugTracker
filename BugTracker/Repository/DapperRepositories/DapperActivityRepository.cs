using BugTracker.Extension_Methods;
using BugTracker.Models;
using BugTracker.Repository.Interfaces;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Repository.DapperRepositories
{
	public class DapperActivityRepository : DapperBaseRepository, IActivityRepository
	{
		public DapperActivityRepository(string connectionString) : base(connectionString) { }

		public async Task<Activity> Add(Activity activity)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				var sql = @"INSERT INTO dbo.ActivityEvents (Timestamp, ProjectId, MessageId, UserId, BugReportId, AssigneeId, LinkedBugReportId, NewBugReportStateId, PreviousBugReportStateId, CommentId, MilestoneId)
					OUTPUT inserted.ActivityId 
					VALUES(@Timestamp, @ProjectId, @MessageId, @UserId, @BugReportId, @AssigneeId, @LinkedBugReportId, @NewBugReportStateId, @PreviousBugReportStateId, @CommentId, @MilestoneId)";
				var parameters = new
				{
					Timestamp = DateTime.Now,
					ProjectId = activity.ProjectId,
					MessageId = activity.MessageId,
					UserId = activity.UserId,
					BugReportId = GetDerivedPropertyOrNull(activity, nameof(ActivityBugReport.BugReportId)), // sets to null if member does not exist
					AssigneeId = GetDerivedPropertyOrNull(activity, nameof(ActivityBugReportAssigned.AssigneeId)),
					LinkedBugReportId = GetDerivedPropertyOrNull(activity, nameof(ActivityBugReportLink.LinkedBugReportId)),
					NewBugReportStateId = GetDerivedPropertyOrNull(activity, nameof(ActivityBugReportStateChange.NewBugReportStateId)),
					PreviousBugReportStateId = GetDerivedPropertyOrNull(activity, nameof(ActivityBugReportStateChange.PreviousBugReportStateId)),
					CommentId = GetDerivedPropertyOrNull(activity, nameof(ActivityComment.CommentId)),
					MilestoneId = GetDerivedPropertyOrNull(activity, nameof(ActivityMilestone.MilestoneId))
				};

				var insertedActivityId = await connection.ExecuteScalarAsync<int>(sql, parameters);
				var insertedActivity = GetActivities("ActivityId", insertedActivityId).ToList().FirstOrDefault();
				return insertedActivity;
			}
		}

		private int? GetDerivedPropertyOrNull(Activity activity, string propertyName)
		{
			return activity.HasProperty(propertyName) ? activity.GetDerivedProperty<int?>(propertyName) : null;
		}

		public async Task<Activity> Delete(int activityId)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				var activityToDelete = GetActivities("ActivityId", activityId).ToList().FirstOrDefault();
				var sql = @"DELETE FROM dbo.ActivityEvents WHERE ActivityId = @activityId";
				var parameters = new
				{
					ActivityId = activityId
				};

				await connection.ExecuteAsync(sql, parameters);
				return activityToDelete;
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
			var parameters = new { Key = key, Id = id.ToString() };

			using (IDbConnection connection = GetConnectionString())
			using (var reader = connection.ExecuteReader(sql, parameters))
			{
				var activityProjectParser = reader.GetRowParser<ActivityProject>();
				var activityBugReportParser = reader.GetRowParser<ActivityBugReport>();
				var activityBugReportLinkParser = reader.GetRowParser<ActivityBugReportLink>();
				var activityBugReportStateChangeParser = reader.GetRowParser<ActivityBugReportStateChange>();
				var activityBugReportAssignedParser = reader.GetRowParser<ActivityBugReportAssigned>();
				var activityCommentParser = reader.GetRowParser<ActivityComment>();
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
							activityEvents.Add(activityCommentParser(reader));
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
