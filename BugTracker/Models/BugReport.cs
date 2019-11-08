using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
	public class BugReport
	{
		[Key]
		public int Id { get; set; }
		public bool Hidden { get; set; }
		public DateTime ReportTime { get; set; }
		public int Severity { get; set; }
		public int Importance { get; set; }
		public string Title { get; set; }
		public string ProgramBehaviour { get; set; }
		public string DetailsToReproduce { get; set; }
		public string PersonReporting { get; set; }
		public IEnumerable<BugReportComment> Comments { get; set; }
		public IEnumerable<BugState> StateHistory { get; set; }
	}
}
