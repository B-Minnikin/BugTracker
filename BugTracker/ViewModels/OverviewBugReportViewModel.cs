using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.ViewModels
{
	public class OverviewBugReportViewModel
	{
		BugReport bugReport { get; set; }
		List<BugReportComment> BugReportComments { get; set; }
		List<BugState> BugStates { get; set; }
		List<AttachmentPath> AttachmentPaths { get; set; }
	}
}
