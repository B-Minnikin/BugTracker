using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.ViewModels
{
	public class BugReportSearchSelectorViewModel
	{
		public int ProjectId { get; set; }
		public List<MilestoneBugReportEntry> MilestoneBugReportEntries { get; set; } = new List<MilestoneBugReportEntry>();

		// Flags
		public bool CanRemoveEntry { get; set; } = false;
		public bool ShowSearchBar { get; set; } = false;
	}
}
