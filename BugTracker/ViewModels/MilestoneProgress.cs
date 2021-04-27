using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.ViewModels
{
	public class MilestoneProgress
	{
		public int CompletedBugReports { get; set; }
		public int TotalBugReports { get; set; }
		public string PercentCompleted { get; set; }

		public MilestoneProgress(IEnumerable<MilestoneBugReportEntry> bugReports)
		{
			CompletedBugReports = bugReports.Where(m => m.CurrentState.StateType != StateType.open).Count();
			TotalBugReports = bugReports.Count();
			if(TotalBugReports > 0)
			{
				PercentCompleted = ((double)CompletedBugReports / (double)TotalBugReports).ToString("P", CultureInfo.InvariantCulture);
			}
			else
			{
				PercentCompleted = "0 %";
			}
		}
	}
}
