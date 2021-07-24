using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
	public class ActivityBugReportStateChange : Activity
	{
		public int BugReportId { get; set; }
		public BugState NewBugReportStateId { get; set; }
		public BugState PreviousBugReportStateId { get; set; }

		public ActivityBugReportStateChange(int activityId, DateTime timestamp, int projectId, ActivityMessage messageId, int userId, int bugReportId, BugState newBugReportStateId, BugState previousBugReportStateId)
			: base(activityId, timestamp, projectId, messageId, userId)
		{
			BugReportId = bugReportId;
			NewBugReportStateId = newBugReportStateId;
			PreviousBugReportStateId = previousBugReportStateId;
		}

		public override string ActivityMessage
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}
	}
}
