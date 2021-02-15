using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.ViewModels
{
	public class EditMilestoneViewModel
	{
		public int ProjectId { get; set; }
		public int LocalBugReportId { get; set; }

		public Milestone Milestone { get; set; }
		public List<MilestoneBugReportEntry> MilestoneBugReportEntries { get; set; } = new List<MilestoneBugReportEntry>();
	}
}
