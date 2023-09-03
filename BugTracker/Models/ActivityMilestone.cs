using System;

namespace BugTracker.Models
{
	public class ActivityMilestone : Activity
	{
		public int MilestoneId { get; set; }

		public ActivityMilestone() { }

		public ActivityMilestone(DateTime timestamp, int projectId, ActivityMessage messageId, string userId, int milestoneId)
			: base(timestamp, projectId, messageId, userId)
		{
			MilestoneId = milestoneId;
		}
	}
}
