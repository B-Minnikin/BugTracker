using System;

namespace BugTracker.Models
{
	public class ActivityBugReportAssigned : Activity
	{
		public int BugReportId { get; set; }
		public int AssigneeId { get; set; }

		public ActivityBugReportAssigned()	{ }

		public ActivityBugReportAssigned(DateTime timestamp, int projectId, ActivityMessage messageId, int userId, int bugReportId, int assigneeId)
			: base(timestamp, projectId, messageId, userId)
		{
			BugReportId = bugReportId;
			AssigneeId = assigneeId;
		}
	}
}
