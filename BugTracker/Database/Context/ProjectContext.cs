using BugTracker.Models;
using BugTracker.Models.ProjectInvitation;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Database.Context;

public class ProjectContext : Microsoft.EntityFrameworkCore.DbContext
{
    public ProjectContext(DbContextOptions<ProjectContext> contextOptions) : base(contextOptions)
    {
    }
    
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectBugReportId> ProjectBugReportIds { get; set; }
    public DbSet<PendingProjectInvitation> PendingProjectInvitations { get; set; }
}
