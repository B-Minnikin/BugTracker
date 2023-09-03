using System;

namespace BugTracker.Models;

public class ActivityBugReportLink : Activity
{
	public int BugReportId { get; set; }
	public int LinkedBugReportId { get; set; }

	public ActivityBugReportLink(DateTime timestamp, int projectId, ActivityMessage messageId, string userId, int firstBugReportId, int secondBugReportId)
		: base(timestamp, projectId, messageId, userId)
	{
		BugReportId = firstBugReportId;
		LinkedBugReportId = secondBugReportId;
	}
}
