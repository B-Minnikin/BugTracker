using BugTracker.Models;
using System.Collections.Generic;

namespace BugTracker.ViewModels
{
	public class EditMilestoneViewModel
	{
		public int ProjectId { get; set; }
		public int LocalBugReportId { get; set; }

		public Milestone Milestone { get; set; }
		public List<MilestoneBugReportEntry> MilestoneBugReportEntries { get; set; } = new();
	}
}
