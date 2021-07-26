using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
	public abstract class Activity
	{
		public int ActivityId { get; set; }
		public DateTime Timestamp { get; set; }
		public int ProjectId { get; set; }
		public ActivityMessage MessageId { get; set; }
		public int UserId { get; set; }

		public abstract string ActivityMessage { get; set; }

		public Activity() { }

		public Activity(int activityId, DateTime timestamp, int projectId, ActivityMessage messageId, int userId)
		{
			ActivityId = activityId;
			Timestamp = timestamp;
			ProjectId = projectId;
			MessageId = messageId;
			UserId = userId;
		}
	}

	public enum ActivityMessage
	{
		ProjectCreated,
		ProjectEdited,
		CommentPosted,
		CommentEdited,
		BugReportPosted,
		BugReportEdited,
		BugReportStateChanged,
		BugReportsLinked,
		BugReportAssignedToUser,
		MilestonePosted,
		MilestoneEdited,
		BugReportAddedToMilestone,
		BugReportRemovedFromMilestone
	}
}
