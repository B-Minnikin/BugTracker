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
		BugReport GetBugReportByLocalId(int localBugReportId, int projectId);
		int GetCommentCountById(int bugReportId);
	}
}
