using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
	public class ActivityBugReportLink : Activity
	{
		public int FirstBugReportId { get; set; }
		public int SecondBugReportId { get; set; }

		public ActivityBugReportLink() { }

		public ActivityBugReportLink(int activityId, DateTime timestamp, int projectId, ActivityMessage messageId, int userId, int firstBugReportId, int secondBugReportId)
			: base(activityId, timestamp, projectId, messageId, userId)
		{
			FirstBugReportId = firstBugReportId;
			SecondBugReportId = secondBugReportId;
		}

		public override string ActivityMessage 
		{ 
			get => throw new NotImplementedException(); 
			set => throw new NotImplementedException(); 
		}
	}
}
