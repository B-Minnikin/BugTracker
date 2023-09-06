using System;

namespace BugTracker.Models
{
	public class ActivityMilestoneBugReport : Activity
	{
		public int MilestoneId{ get; set; }

		public ActivityMilestoneBugReport() { }

		public ActivityMilestoneBugReport(DateTime timestamp, int projectId, ActivityMessage messageId, string userId, int milestoneId, int bugReportId)
			: base(timestamp, projectId, messageId, userId)
		{
			MilestoneId = milestoneId;
			BugReportId = bugReportId;
		}
	}
}
