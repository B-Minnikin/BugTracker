using BugTracker.Models;
using BugTracker.Repository.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Repository
{
	public interface IBugReportRepository : IAdd<BugReport>,
		IUpdate<BugReport>, IDelete<BugReport>,
		IGetById<BugReport>, IGetAllById<BugReport>
	{
		//BugReport AddBugReport(BugReport bugReport);
		//IEnumerable<BugReport> GetAllBugReports(int projectId);
		//BugReport GetBugReportById(int bugReportId);
		BugReport GetBugReportByLocalId(int localBugReportId, int projectId);
		//BugReport UpdateBugReport(BugReport reportChanges);
		//BugReport DeleteBugReport(int id);
		int GetCommentCountById(int bugReportId);
	}
}
