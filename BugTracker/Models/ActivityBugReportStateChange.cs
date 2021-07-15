using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
	public class ActivityBugReportStateChange : Activity
	{
		public BugState NewBugReportStateId { get; set; }
		public BugState PreviousBugReportStateId { get; set; }

		public ActivityBugReportStateChange(int activityId, DateTime timestamp, int projectId, ActivityMessage messageId, int userId, BugState newBugReportStateId, BugState previousBugReportStateId)
			: base(activityId, timestamp, projectId, messageId, userId)
		{
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
