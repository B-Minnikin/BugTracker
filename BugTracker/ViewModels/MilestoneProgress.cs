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
		private string _percentCompleted = "0%";

		public int CompletedBugReports { get; set; }
		public int TotalBugReports { get; set; }
		public string PercentCompleted
		{
			get {
				return _percentCompleted;
			}
			private set
			{
				if (TotalBugReports > 0)
				{
					_percentCompleted = String.Format("{0}%", value);
				}
			}
		}

		public string PercentCompletedText
		{
			get
			{
				return _percentCompleted.Replace("%", " percent");
			}
		}

		public MilestoneProgress(IEnumerable<MilestoneBugReportEntry> bugReports)
		{
			CompletedBugReports = bugReports.Where(m => m.CurrentState.StateType != StateType.open).Count();
			TotalBugReports = bugReports.Count();

			if(TotalBugReports > 0)
			{
				PercentCompleted = (Math.Floor(((double)CompletedBugReports / (double)TotalBugReports) * 100)).ToString();
			}
			else
			{
				PercentCompleted = "0";
			}
		}
	}
}
