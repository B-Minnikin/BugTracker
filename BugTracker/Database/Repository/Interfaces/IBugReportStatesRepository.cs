using System.Threading.Tasks;
using BugTracker.Database.Repository.Common;
using BugTracker.Models;

namespace BugTracker.Database.Repository.Interfaces
{
	public interface IBugReportStatesRepository : IAdd<BugState>,
		IGetById<BugState>, IGetAllById<BugState>
	{
		Task<BugState> GetLatestState(int bugReportId);
	}
}
