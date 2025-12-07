using BugTracker.Models;
using System.Collections.Generic;

namespace BugTracker.ViewModels;

public class BugReportSearchSelectorViewModel
{
	public int ProjectId { get; set; }
	public List<MilestoneBugReportEntry> MilestoneBugReportEntries { get; set; } = new();

	// Flags
	public bool CanRemoveEntry { get; set; }
	public bool ShowSearchBar { get; set; }
}
