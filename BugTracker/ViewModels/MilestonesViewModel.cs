using System.Collections.Generic;

namespace BugTracker.ViewModels
{
	public class MilestonesViewModel
	{
		public int ProjectId { get; set; }
		public bool ShowNewButton { get; set; }

		public List<MilestoneContainer> ProjectMilestones { get; set; }
	}
}
