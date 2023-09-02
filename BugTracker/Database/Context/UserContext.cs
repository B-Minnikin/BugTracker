using BugTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Database.Context;

public class UserContext : Microsoft.EntityFrameworkCore.DbContext
{
    public UserContext(DbContextOptions contextOptions) : base(contextOptions)
    {
        
    }
    
    public DbSet<ApplicationUser> Users { get; set; }
    public DbSet<UserSubscription> UserSubscriptions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserSubscription>()
            .HasKey(us => new { us.BugReportId, us.UserId });

        modelBuilder.Entity<UserSubscription>()
            .HasOne(us => us.User)
            .WithMany(u => u.UserSubscriptions)
            .HasForeignKey(us => us.UserId);

        modelBuilder.Entity<UserSubscription>()
            .HasOne(us => us.BugReport)
            .WithMany(br => br.UserSubscriptions)
            .HasForeignKey(us => us.BugReportId);
    }
}
