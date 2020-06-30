using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models.Database
{
	public interface ISubscriptionHelper
	{
		bool IsSubscribed(int userId, int bugReportId);

		void NotifyBugReportStateChanged(int bugReportId, BugState bugState);
	}
}
