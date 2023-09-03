using BugTracker.Models;

namespace BugTracker.ViewModels
{
	public class CreateBugReportViewModel
	{
		public BugReport BugReport { get; set; }
		public string Title { get; set; }
		public string ProgramBehaviour { get; set; }
		public string DetailsToReproduce {get; set;}
		public Severity Severity { get; set; } = Severity.Medium;
		public Importance Importance { get; set; } = Importance.Medium;
		public bool Hidden { get; set; }

		public bool Subscribe { get; set; }
	}
}
