using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
	public class MilestoneBugReportEntry
	{
		public int BugReportId { get; set; }
		public int LocalBugReportId { get; set; }
		public string Title { get; set; }
		public string Url { get; set; }
	}
}
