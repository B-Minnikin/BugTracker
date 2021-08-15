using System;

namespace BugTracker.Models
{
	public class ActivityBugReportStateChange : Activity
	{
		public int BugReportId { get; set; }
		public BugState NewBugReportStateId { get; set; }
		public BugState PreviousBugReportStateId { get; set; }

		public ActivityBugReportStateChange() { }

		public ActivityBugReportStateChange(DateTime timestamp, int projectId, ActivityMessage messageId, int userId, int bugReportId, BugState newBugReportStateId, BugState previousBugReportStateId)
			: base(timestamp, projectId, messageId, userId)
		{
			BugReportId = bugReportId;
			NewBugReportStateId = newBugReportStateId;
			PreviousBugReportStateId = previousBugReportStateId;
		}
	}
}
