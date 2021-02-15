using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.ViewModels
{
	public class MilestoneOverviewViewModel
	{
		public Milestone Milestone { get; set; }
		public List<MilestoneBugReportEntry> MilestoneBugReportEntries { get; set; }  = new List<MilestoneBugReportEntry>();
	}
}
