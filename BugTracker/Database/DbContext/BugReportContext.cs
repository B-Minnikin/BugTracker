using BugTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Database.DbContext;

public class BugReportContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbSet<BugReport> BugReports { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<UserBugReport> UserBugReports { get; set; }
    public DbSet<BugReportLink> BugReportLinks { get; set; }
    
    // TODO - attachment paths

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Users assigned to bug reports
        modelBuilder.Entity<UserBugReport>()
            .HasKey(ub => new { ub.UserId, ub.BugReportId });

        modelBuilder.Entity<UserBugReport>()
            .HasOne(ub => ub.User)
            .WithMany(u => u.UserBugReports)
            .HasForeignKey(ub => ub.UserId);

        modelBuilder.Entity<UserBugReport>()
            .HasOne(ub => ub.BugReport)
            .WithMany(b => b.UserBugReports)
            .HasForeignKey(ub => ub.UserId);

        // Bug report links
        modelBuilder.Entity<BugReportLink>()
            .HasKey(bl => new { bl.BugReportId, bl.LinkedBugReportId });

        modelBuilder.Entity<BugReportLink>()
            .HasOne(bl => bl.BugReport)
            .WithMany(b => b.BugReportLinks)
            .HasForeignKey(bl => bl.BugReportId);

        modelBuilder.Entity<BugReportLink>()
            .HasOne(bl => bl.LinkedBugReport)
            .WithMany(b => b.BugReportLinks)
            .HasForeignKey(bl => bl.LinkedBugReportId);
    }
}
