using BugTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Database.Context;

public class UserContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbSet<ApplicationUser> Users { get; set; }
}
