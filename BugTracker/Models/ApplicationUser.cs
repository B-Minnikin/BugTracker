using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace BugTracker.Models;

public class ApplicationUser : IdentityUser
{
    public ICollection<UserBugReport> UserBugReports { get; set; }
    public ICollection<UserSubscription> UserSubscriptions { get; set; }
}

public class UserSubscription
{
    public int UserId { get; set; }
    public ApplicationUser User { get; set; }
    public int BugReportId { get; set; }
    public BugReport BugReport { get; set; }
}
