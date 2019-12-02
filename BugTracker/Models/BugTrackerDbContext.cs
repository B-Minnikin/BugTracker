using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
	public class BugTrackerDbContext : DbContext
	{
		public BugTrackerDbContext(DbContextOptions<BugTrackerDbContext> options) : base(options)
		{

		}

		public DbSet<Project> Projects { get; set; }
		public DbSet<BugReport> BugReports { get; set; }
		public DbSet<BugState> BugStates { get; set; }
		public DbSet<BugReportComment> BugReportComments { get; set; }
		public DbSet<AttachmentPath> AttachmentPaths { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
		}
	}
}
