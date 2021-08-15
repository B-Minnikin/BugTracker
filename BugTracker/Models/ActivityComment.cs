using System;

namespace BugTracker.Models
{
	public class ActivityComment : Activity
	{
		public int BugReportId { get; set; }
		public int BugReportCommentId { get; set; }

		public ActivityComment() { }

		public ActivityComment(DateTime timestamp, int projectId, ActivityMessage messageId, int userId, int bugReportId, int bugReportCommentId) 
			: base(timestamp, projectId, messageId, userId)
		{
			BugReportId = bugReportId;
			BugReportCommentId = bugReportCommentId;
		}
	}
}
