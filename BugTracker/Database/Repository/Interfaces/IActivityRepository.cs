using System.Collections.Generic;
using System.Threading.Tasks;
using BugTracker.Database.Repository.Common;
using BugTracker.Models;

namespace BugTracker.Database.Repository.Interfaces
{
	public interface IActivityRepository : IAdd<Activity>,
		IDelete<Activity>
	{
		Task<IEnumerable<Activity>> GetUserActivities(string id);
		Task<IEnumerable<Activity>> GetBugReportActivities(int bugReportId);
	}
}
