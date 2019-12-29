using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.ViewModels
{
	public delegate int GetCommentCount(int bugReportId);

	public class OverviewProjectViewModel
	{
		public Project Project { get; set; }
		public List<BugReport> BugReports {get; set; }
		public GetCommentCount CommentCountHandler;
	}
}
