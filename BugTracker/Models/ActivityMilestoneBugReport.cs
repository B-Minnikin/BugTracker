using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
	public class ActivityMilestoneBugReport : Activity
	{
		public int MilestoneId{ get; set; }
		public int BugReportId { get; set; }

		public ActivityMilestoneBugReport() { }

		public ActivityMilestoneBugReport(int activityId, DateTime timestamp, int projectId, ActivityMessage messageId, int userId, int milestoneId, int bugReportId)
			: base(activityId, timestamp, projectId, messageId, userId)
		{
			MilestoneId = milestoneId;
			BugReportId = bugReportId;
		}

		public override string ActivityMessage
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}
	}
}
