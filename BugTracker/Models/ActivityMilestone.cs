using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
	public class ActivityMilestone : Activity
	{
		public int MilestoneId { get; set; }

		public ActivityMilestone() { }

		public ActivityMilestone(DateTime timestamp, int projectId, ActivityMessage messageId, int userId, int milestoneId)
			: base(timestamp, projectId, messageId, userId)
		{
			MilestoneId = milestoneId;
		}

		public override string ActivityMessage
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}
	}
}
