using System;

namespace BugTracker.Models
{
	public abstract class Activity
	{
		protected Activity() { }
		protected Activity(DateTime timestamp, int projectId, ActivityMessage messageId, string userId)
		{
			Timestamp = timestamp;
			ProjectId = projectId;
			MessageId = messageId;
			UserId = userId;
		}
		
		public int ActivityId { get; set; }
		public DateTime Timestamp { get; set; }
		public int ProjectId { get; set; }
		public ActivityMessage MessageId { get; set; }
		public string UserId { get; set; }
		public int BugReportId { get; set; }
		public bool Hidden { get; set; }

		public string ActivityMessage { get; set; }
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
