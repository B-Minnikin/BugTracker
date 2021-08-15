using System;

namespace BugTracker.Models
{
	public class ActivityProject : Activity
	{
		public ActivityProject() { }

		public ActivityProject(DateTime timestamp, int projectId, ActivityMessage messageId, int userId)
			: base(timestamp, projectId, messageId, userId)
		{ }
	}
}
