using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace BugTracker.ViewModels;

public class MilestoneProgress
{
	private string percentCompleted = "0%";

	public int CompletedBugReports { get; set; }
	public int TotalBugReports { get; set; }
	public string PercentCompleted
	{
		get => percentCompleted;
		private set
		{
			if (TotalBugReports > 0)
			{
				percentCompleted = $"{value}%";
			}
		}
	}

	public string PercentCompletedText => percentCompleted.Replace("%", " percent");

	public MilestoneProgress(List<MilestoneBugReportEntry> bugReports)
	{
		CompletedBugReports = bugReports.Count(m => m.CurrentState.StateType != StateType.Open);
		TotalBugReports = bugReports.Count();

		PercentCompleted = TotalBugReports > 0
			? Math.Floor((double)CompletedBugReports / TotalBugReports * 100).ToString(CultureInfo.InvariantCulture)
			: "0";
	}
}
