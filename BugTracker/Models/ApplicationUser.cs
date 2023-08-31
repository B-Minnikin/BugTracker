using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace BugTracker.Models;

public class ApplicationUser : IdentityUser
{
    public ICollection<UserBugReport> UserBugReports { get; set; }
}
