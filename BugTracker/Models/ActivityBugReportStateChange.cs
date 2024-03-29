﻿using System;

namespace BugTracker.Models
{
	public class ActivityBugReportStateChange : Activity
	{
		public int BugReportId { get; set; }
		public int NewBugReportStateId { get; set; }
		public int PreviousBugReportStateId { get; set; }

		public ActivityBugReportStateChange() { }

		public ActivityBugReportStateChange(DateTime timestamp, int projectId, ActivityMessage messageId, int userId, int bugReportId, int newBugReportStateId, int previousBugReportStateId)
			: base(timestamp, projectId, messageId, userId)
		{
			BugReportId = bugReportId;
			NewBugReportStateId = newBugReportStateId;
			PreviousBugReportStateId = previousBugReportStateId;
		}
	}
}
