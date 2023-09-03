using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugTracker.Models
{
	public class Milestone
	{
		public int MilestoneId { get; set; }
		public int ProjectId { get; set; }
		[Required(ErrorMessage = "Please enter a title")]
		public string Title { get; set; }
		public string Description { get; set; }
		public DateTime CreationTime { get; set; } = DateTime.Now;
		public DateTime DueDate { get; set; } = DateTime.Now.AddDays(7);
		public bool Hidden { get; set; }

		public ICollection<MilestoneBugReport> MilestoneBugReports { get; set; }
	}

	public class MilestoneBugReport
	{
		[ForeignKey("Milestone")]
		public int MilestoneId { get; set; }
		public Milestone Milestone { get; set; }
		[ForeignKey("BugReport")]
		public int BugReportId { get; set; }
		public BugReport BugReport { get; set; }
	}
}
