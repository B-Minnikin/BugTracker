using BugTracker.Models;
using System.Collections.Generic;

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
