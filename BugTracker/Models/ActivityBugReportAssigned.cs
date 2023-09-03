using System;

namespace BugTracker.Models
{
	public class ActivityBugReportAssigned : Activity
	{
		public int BugReportId { get; set; }
		public string AssigneeId { get; set; }

		public ActivityBugReportAssigned()	{ }

		public ActivityBugReportAssigned(DateTime timestamp, int projectId, ActivityMessage messageId, string userId, int bugReportId, string assigneeId)
			: base(timestamp, projectId, messageId, userId)
		{
			BugReportId = bugReportId;
			AssigneeId = assigneeId;
		}
	}
}
