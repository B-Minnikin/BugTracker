using BugTracker.Models;
using BugTracker.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.ViewModels
{
	public class OverviewBugReportViewModel
	{
		public BugReport BugReport { get; set; }
		public StateType CurrentState { get; set; }
		public List<BugReportComment> BugReportComments { get; set; }
		public List<BugState> BugStates { get; set; }
		public List<AttachmentPath> AttachmentPaths { get; set; }

		public bool DisableSubscribeButton { get; set; }
		public bool DisableAssignMembersButton { get; set; } = true;
	}
}
