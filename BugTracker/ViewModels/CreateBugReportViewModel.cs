using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.ViewModels
{
	public class CreateBugReportViewModel
	{
		public BugReport BugReport { get; set; }
		public string Title { get; set; }
		public string ProgramBehaviour { get; set; }
		public string DetailsToReproduce {get; set;}
		public Severity Severity { get; set; } = Severity.Medium;
		public int Importance { get; set; }
		public bool Hidden { get; set; }

		public bool Subscribe { get; set; } = false;
	}
}
