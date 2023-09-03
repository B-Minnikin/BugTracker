using BugTracker.Models;
using System.Collections.Generic;

namespace BugTracker.ViewModels
{
	public class ManageLinksViewModel
	{
		public int ProjectId { get; set; }
		public int BugReportId { get; set; }
		public int LinkToBugReportLocalId { get; set; }

		public List<BugReport> LinkedReports { get; set; } = new();
	}
}
