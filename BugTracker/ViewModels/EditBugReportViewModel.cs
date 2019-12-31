using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.ViewModels
{
	public class EditBugReportViewModel
	{
		public BugReport BugReport { get; set; }
		public StateType CurrentState { get; set; }
	}
}
