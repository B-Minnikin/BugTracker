using System.Collections.Generic;

namespace BugTracker.ViewModels
{
	public class ActivityFeedViewModel
	{
		public List<Models.Activity> Activities { get; set; } = new();
	}
}
