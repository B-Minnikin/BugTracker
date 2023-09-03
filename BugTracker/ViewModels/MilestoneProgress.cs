using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace BugTracker.ViewModels;

public class MilestoneProgress
{
	private string _percentCompleted = "0%";

	public int CompletedBugReports { get; }
	public int TotalBugReports { get; }
	public string PercentCompleted
	{
		get => _percentCompleted;
		private set
		{
			if (TotalBugReports > 0)
			{
				_percentCompleted = $"{value}%";
			}
		}
	}

	public string PercentCompletedText => _percentCompleted.Replace("%", " percent");

	public MilestoneProgress(IEnumerable<MilestoneBugReportEntry> bugReports)
	{
		CompletedBugReports = bugReports.Where(m => m.CurrentState.StateType != StateType.Open).Count();
		TotalBugReports = bugReports.Count();

		PercentCompleted = TotalBugReports > 0
			? Math.Floor((double)CompletedBugReports / TotalBugReports * 100).ToString(CultureInfo.CurrentCulture)
			: "0";
	}
}
