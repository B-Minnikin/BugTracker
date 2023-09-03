using BugTracker.Models;
using System.Collections.Generic;

namespace BugTracker.ViewModels
{
	public class MilestoneOverviewViewModel
	{
		public Milestone Milestone { get; set; }
		public List<MilestoneBugReportEntry> MilestoneBugReportEntries { get; set; }  = new();

		public MilestoneContainer ProjectMilestone { get; set; }
	}
}
