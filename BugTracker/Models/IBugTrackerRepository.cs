using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
	interface IBugTrackerRepository
	{
		BugReport GetBugReport(int Id);
		IEnumerable<BugReport> GetAllBugReports();
		BugReport Add(BugReport bugReport);
		BugReport Update(BugReport bugReportChanges);
		BugReport Delete(int Id);
	}
}
