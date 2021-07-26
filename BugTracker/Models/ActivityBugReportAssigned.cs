using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
	public class ActivityBugReportAssigned : Activity
	{
		public int BugReportId { get; set; }
		public int AssigneeId { get; set; }

		public ActivityBugReportAssigned()	{ }

		public ActivityBugReportAssigned(int activityId, DateTime timestamp, int projectId, ActivityMessage messageId, int userId, int bugReportId, int assigneeId)
			: base(activityId, timestamp, projectId, messageId, userId)
		{
			BugReportId = bugReportId;
			AssigneeId = assigneeId;
		}

		public override string ActivityMessage
		{ 
			get => throw new NotImplementedException(); 
			set => throw new NotImplementedException(); 
		}
	}
}
