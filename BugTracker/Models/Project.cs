using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BugTracker.Models.ProjectInvitation;

namespace BugTracker.Models;

public class Project
{
	[Key]
	public int ProjectId { get; set; }
	[Required]
	public string Name { get; set; }
	public string Description { get; set; }
	public DateTime CreationTime { get; set; }
	public DateTime LastUpdateTime { get; set; }
	public int NextFreeBugReportId { get; set; }
	public bool Hidden { get; set; }

	public IEnumerable<BugReport> BugReports { get; set; }
	public ICollection<PendingProjectInvitation> PendingProjectInvitations { get; set; }
}
