using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models.Database
{
	public interface ISubscriptions
	{
		bool IsSubscribed(int userId, int bugReportId);

		void NotifyBugReportStateChanged(int bugReportId, BugState bugState);
		void NotifyBugReportNewComment(BugReportComment bugReportComment);
	}
}
