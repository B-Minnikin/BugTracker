using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
	public enum Severity
	{
		Low,
		Medium,
		High,
		Critical
	}

	public enum Importance
	{
		Low,
		Medium,
		High,
		Immediate
	}

	public class BugReport
	{
		[Key]
		public int BugReportId { get; set; }
		public int LocalBugReportId { get; set; }
		public bool Hidden { get; set; }
		public DateTime CreationTime { get; set; }
		public Severity Severity { get; set; } = Severity.Medium;
		public Importance Importance { get; set; } = Importance.Medium;
		public string Title { get; set; }
		public string ProgramBehaviour { get; set; }
		public string DetailsToReproduce { get; set; }
		public string PersonReporting { get; set; }
		
		public int ProjectId { get; set; }
		
		public ICollection<UserBugReport> UserBugReports { get; set; }
		//public ICollection<BugReportLink> BugReportLinks { get; set; }
		public ICollection<UserSubscription> UserSubscriptions { get; set; }
		public ICollection<MilestoneBugReport> MilestoneBugReports { get; set; }
	}

	public class UserBugReport
	{
		public int BugReportId { get; set; }
		public BugReport BugReport { get; set; }
		public string UserId { get; set; }
		public ApplicationUser User { get; set; }
	}

	public class BugReportLink
	{
		public int BugReportId { get; set; }
		public BugReport BugReport { get; set; }
		
		public int LinkedBugReportId { get; set; }
		public BugReport LinkedBugReport { get; set; }
	}
}
