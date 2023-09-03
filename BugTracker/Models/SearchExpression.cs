using System;

namespace BugTracker.Models;

public class SearchExpression
{
	public string SearchText { get; set; } = "";
	public bool SearchTitles { get; set; } = true;
	public bool SearchInDetails { get; set; }

	public DateTime DateRangeBegin { get; set; }
	public DateTime DateRangeEnd { get; set; } = DateTime.Now;
}
