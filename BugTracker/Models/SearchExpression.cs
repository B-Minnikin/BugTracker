using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
	public class SearchExpression
	{
		public string SearchText { get; set; }
		public bool SearchTitles { get; set; } = true;
		public bool SearchInDetails { get; set; } = false;
	}
}
