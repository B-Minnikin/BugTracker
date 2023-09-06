using BugTracker.Models;
using BugTracker.Models.Authorization;
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
    public DbSet<PendingProjectInvitation> PendingProjectInvitations { get; set; }
    
    public DbSet<BugReport> BugReports { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<UserBugReport> UserBugReports { get; set; }
    public DbSet<BugReportLink> BugReportLinks { get; set; }
    public DbSet<BugState> BugStates { get; set; }
    public DbSet<Milestone> Milestones { get; set; }
    public DbSet<MilestoneBugReport> MilestoneBugReports { get; set; }
    public DbSet<Activity> Activities { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    
    
    public DbSet<Role> Roles { get; set; }

    // TODO - attachment paths

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PendingProjectInvitation>()
            .HasKey(pi => new { pi.ProjectId, pi.EmailAddress });

        modelBuilder.Entity<PendingProjectInvitation>()
            .HasOne(pi => pi.Project)
            .WithMany(p => p.PendingProjectInvitations)
            .HasForeignKey(pi => pi.ProjectId);
        
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

        modelBuilder.Entity<UserRole>()
            .HasKey(ur => ur.UserRoleId);

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId);

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId);

        modelBuilder.Entity<MilestoneBugReport>()
            .HasKey(mbr => new { mbr.BugReportId, mbr.MilestoneId });

        modelBuilder.Entity<MilestoneBugReport>()
            .HasOne(mbr => mbr.Milestone)
            .WithMany(m => m.MilestoneBugReports)
            .HasForeignKey(mbr => mbr.MilestoneId);

        modelBuilder.Entity<MilestoneBugReport>()
            .HasOne(mbr => mbr.BugReport)
            .WithMany(br => br.MilestoneBugReports)
            .HasForeignKey(mbr => mbr.BugReportId);
        
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
            .HasForeignKey(ub => ub.BugReportId);

        // Bug report links
        modelBuilder.Entity<BugReportLink>()
            .HasKey(bl => new { bl.BugReportId, bl.LinkedBugReportId });

        modelBuilder.Entity<BugReportLink>()
            .HasOne(bl => bl.BugReport)
            .WithMany()
            .HasForeignKey(bl => bl.BugReportId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<BugReportLink>()
            .HasOne(bl => bl.LinkedBugReport)
            .WithMany()
            .HasForeignKey(bl => bl.LinkedBugReportId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
