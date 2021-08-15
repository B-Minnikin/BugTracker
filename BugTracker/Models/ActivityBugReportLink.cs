using System;

namespace BugTracker.Models
{
	public class ActivityBugReportLink : Activity
	{
		public int FirstBugReportId { get; set; }
		public int SecondBugReportId { get; set; }

		public ActivityBugReportLink() { }

		public ActivityBugReportLink(DateTime timestamp, int projectId, ActivityMessage messageId, int userId, int firstBugReportId, int secondBugReportId)
			: base(timestamp, projectId, messageId, userId)
		{
			FirstBugReportId = firstBugReportId;
			SecondBugReportId = secondBugReportId;
		}
	}
}
