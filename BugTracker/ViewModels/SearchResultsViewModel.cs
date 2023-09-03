using BugTracker.Models;
using System.Collections.Generic;

namespace BugTracker.ViewModels;

public class SearchResultsViewModel
{
	public bool AdvancedSearchResultsBeginCollapsed { get; set; } = true;
	public SearchExpression SearchExpression { get; set; } = new()
	{
		SearchText = "",
		SearchInDetails = false,
		SearchTitles = true
	};
	public List<BugReport> SearchResults { get; set; } = new();
}
