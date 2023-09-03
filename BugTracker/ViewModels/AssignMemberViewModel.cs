using System.Collections.Generic;
using BugTracker.Models;

namespace BugTracker.ViewModels;
	
public class AssignMemberViewModel
 {
 	public int ProjectId { get; set; }
 	public int BugReportId { get; set; }
 	public string MemberEmail { get; set; }
 
 	public List<ApplicationUser> AssignedUsers { get; set; } = new();
 }
