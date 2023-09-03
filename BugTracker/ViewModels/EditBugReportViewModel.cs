using BugTracker.Models;

namespace BugTracker.ViewModels
{
	public class EditBugReportViewModel
	{
		public BugReport BugReport { get; set; }
		public StateType CurrentState { get; set; }
	}
}
