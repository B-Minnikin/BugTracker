using System;

namespace BugTracker.Models;

public class ActivityBugReport : Activity
{
	public ActivityBugReport(DateTime timestamp, int projectId, ActivityMessage messageId, string userId, int bugReportId)
		: base(timestamp, projectId, messageId, userId)
	{
		BugReportId = bugReportId;
	}
}
