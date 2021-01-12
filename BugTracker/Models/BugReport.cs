using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
	public enum Severity
	{
		Low,
		Medium,
		High,
		Critical
	}

	public class BugReport
	{
		[Key]
		public int BugReportId { get; set; }
		public int LocalBugReportId { get; set; }
		public bool Hidden { get; set; }
		public DateTime CreationTime { get; set; }
		public Severity Severity { get; set; } = Severity.Medium;
		public int Importance { get; set; }
		public string Title { get; set; }
		public string ProgramBehaviour { get; set; }
		public string DetailsToReproduce { get; set; }
		public string PersonReporting { get; set; }
		
		public int ProjectId { get; set; }
	}
}
