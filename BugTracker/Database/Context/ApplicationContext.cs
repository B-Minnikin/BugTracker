using BugTracker.Models;
using BugTracker.Models.ProjectInvitation;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Database.Context;

public class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
        
    }
    
    public DbSet<ApplicationUser> Users { get; set; }
    public DbSet<UserSubscription> UserSubscriptions { get; set; }
    
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectBugReportId> ProjectBugReportIds { get; set; }
    public DbSet<PendingProjectInvitation> PendingProjectInvitations { get; set; }
    
    public DbSet<BugReport> BugReports { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<UserBugReport> UserBugReports { get; set; }
    public DbSet<BugReportLink> BugReportLinks { get; set; }
    public DbSet<BugState> BugStates { get; set; }

    // TODO - attachment paths

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProjectBugReportId>()
            .HasKey(pbr => pbr.ProjectId);

        modelBuilder.Entity<ProjectBugReportId>()
            .HasOne<Project>()
            .WithMany()
            .HasForeignKey(p => p.ProjectId);
        
        modelBuilder.Entity<UserSubscription>()
            .HasKey(us => new { us.UserId, us.BugReportId });

        modelBuilder.Entity<UserSubscription>()
            .HasOne(us => us.User)
            .WithMany(au => au.UserSubscriptions)
            .HasForeignKey(us => us.UserId);

        modelBuilder.Entity<UserSubscription>()
            .HasOne(us => us.BugReport)
            .WithMany(br => br.UserSubscriptions)
            .HasForeignKey(us => us.BugReportId);
        
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
            .WithMany()
            .HasForeignKey(bl => bl.BugReportId);

        modelBuilder.Entity<BugReportLink>()
            .HasOne(bl => bl.LinkedBugReport)
            .WithMany()
            .HasForeignKey(bl => bl.LinkedBugReportId);
    }
}