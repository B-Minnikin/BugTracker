using System;

namespace BugTracker.Models
{
	public class ActivityMilestoneBugReport : Activity
	{
		public int MilestoneId{ get; set; }
		public int BugReportId { get; set; }

		public ActivityMilestoneBugReport() { }

		public ActivityMilestoneBugReport(DateTime timestamp, int projectId, ActivityMessage messageId, int userId, int milestoneId, int bugReportId)
			: base(timestamp, projectId, messageId, userId)
		{
			MilestoneId = milestoneId;
			BugReportId = bugReportId;
		}
	}
}
