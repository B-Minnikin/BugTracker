using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.ViewModels
{
	public class SearchResultsViewModel
	{
		public SearchExpression SearchExpression { get; set; } = new SearchExpression
		{
			SearchText = "",
			SearchInDetails = false,
			SearchTitles = true
		};
		public List<BugReport> SearchResults { get; set; } = new List<BugReport>();
	}
}
