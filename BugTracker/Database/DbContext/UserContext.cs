using BugTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Database.DbContext;

public class UserContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbSet<ApplicationUser> Users { get; set; }
}
