using System;

namespace BugTracker.Models;

public class ActivityBugReportLink : Activity
{
	public int LinkedBugReportId { get; set; }

	public ActivityBugReportLink(DateTime timestamp, int projectId, ActivityMessage messageId, string userId, int bugReportId, int linkedBugReportId)
		: base(timestamp, projectId, messageId, userId)
	{
		BugReportId = bugReportId;
		LinkedBugReportId = linkedBugReportId;
	}
}
