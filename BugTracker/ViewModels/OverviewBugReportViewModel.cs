using BugTracker.Models;
using System.Collections.Generic;

namespace BugTracker.ViewModels;
	
public class OverviewBugReportViewModel
{
	public BugReport BugReport { get; set; }
	public StateType CurrentState { get; set; }
	public List<Comment> Comments { get; set; }
	public List<BugState> BugStates { get; set; }
	public List<AttachmentPath> AttachmentPaths { get; set; }
	public List<Activity> Activities { get; set; }
	public string AssignedMembersDisplay { get; set; }

	public bool DisableSubscribeButton { get; set; }
	public bool DisableAssignMembersButton { get; set; } = true;
}
