using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace BugTracker.ViewModels;

public class AssignMemberViewModel
{
    public int ProjectId { get; set; }
    public int BugReportId { get; set; }
    public string MemberEmail { get; set; }

    public List<IdentityUser> AssignedUsers { get; set; } = new();
}
