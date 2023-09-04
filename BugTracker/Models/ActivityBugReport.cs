using System;

namespace BugTracker.Models;

public class ActivityBugReport : Activity
{
	public int BugReportId { get; set; }

	public ActivityBugReport(DateTime timestamp, int projectId, ActivityMessage messageId, string userId, int bugReportId)
		: base(timestamp, projectId, messageId, userId)
	{
		BugReportId = bugReportId;
	}
}
