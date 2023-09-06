using System.Collections.Generic;
using System.Threading.Tasks;
using BugTracker.Models;

namespace BugTracker.Database.Repository.Interfaces
{
	public interface IUserSubscriptionsRepository
	{
		Task AddSubscription(string userId, int bugReportId);
		Task<IEnumerable<BugReport>> GetSubscribedReports(string userId);
		Task<bool> IsSubscribed(string userId, int bugReportId);
		Task DeleteSubscription(string userId, int bugReportId);
		Task<IEnumerable<string>> GetAllSubscribedUserIds(int bugReportId);
	}
}
