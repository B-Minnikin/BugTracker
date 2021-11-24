using BugTracker.Models;
using BugTracker.Repository.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BugTracker.Repository.Interfaces
{
	public interface IActivityRepository : IAdd<Activity>,
		IDelete<Activity>
	{
		Task<IEnumerable<Activity>> GetUserActivities(int userId);
		Task<IEnumerable<Activity>> GetBugReportActivities(int bugReportId);
	}
}
