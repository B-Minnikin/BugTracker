﻿using BugTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Database.DbContext;

public class ProjectContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectBugReportId> ProjectBugReportIds { get; set; }
}
