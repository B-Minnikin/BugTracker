using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
	public class BugTrackerRepository : IBugTrackerRepository
	{
		private readonly BugTrackerDbContext _context;

		public BugTrackerRepository(BugTrackerDbContext context)
		{
			this._context = context;
		}

		public BugReport Add(BugReport bugReport)
		{
			_context.BugReports.Add(bugReport);
			_context.SaveChanges();
			return bugReport;
		}

		public BugReport Delete(int Id)
		{
			BugReport bugReport = _context.BugReports.Find(Id);
			if (bugReport != null)
			{
				_context.BugReports.Remove(bugReport);
				_context.SaveChanges();
			}
			return bugReport;
		}

		public IEnumerable<BugReport> GetAllBugReports()
		{
			return _context.BugReports;
		}

		public BugReport GetBugReport(int Id)
		{
			return _context.BugReports.Find(Id);
		}

		public BugReport Update(BugReport bugReportChanges)
		{
			var bugReport = _context.BugReports.Attach(bugReportChanges);
			bugReport.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
			_context.SaveChanges();
			return bugReportChanges;
		}
	}
}
