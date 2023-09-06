using System.Collections.Generic;
using BugTracker.Models.Authorization;
using Microsoft.AspNetCore.Identity;

namespace BugTracker.Models;

public class ApplicationUser : IdentityUser
{
    public int ApplicationUserId { get; set; }
    public bool Hidden { get; set; }
    public ICollection<UserBugReport> UserBugReports { get; set; }
    public ICollection<UserSubscription> UserSubscriptions { get; set; }
    public ICollection<UserRole> UserRoles { get; set; }
}

public class UserSubscription
{
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    public int BugReportId { get; set; }
    public BugReport BugReport { get; set; }
}

public class UserRole
{
    //[Key]
    public int UserRoleId { get; set; }
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    public int RoleId { get; set; }
    public Role Role { get; set; }
    
    public int ProjectId { get; set; }
}
