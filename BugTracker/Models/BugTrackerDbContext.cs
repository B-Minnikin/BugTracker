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

		public DbSet<BugReport> BugReports { get; set; }
	}
}
