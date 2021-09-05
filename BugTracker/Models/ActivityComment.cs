using System;

namespace BugTracker.Models
{
	public class ActivityComment : Activity
	{
		public int BugReportId { get; set; }
		public int CommentId { get; set; }

		public ActivityComment() { }

		public ActivityComment(DateTime timestamp, int projectId, ActivityMessage messageId, int userId, int bugReportId, int commentId) 
			: base(timestamp, projectId, messageId, userId)
		{
			BugReportId = bugReportId;
			CommentId = commentId;
		}
	}
}
