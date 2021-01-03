using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.ViewModels
{
	public class ManageLinksViewModel
	{
		public int ProjectId { get; set; }
		public int BugReportId { get; set; }
		public int LinkToBugReportId { get; set; }

		public List<BugReport> LinkedReports { get; set; }
	}
}
