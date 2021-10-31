using BugTracker.Models;
using BugTracker.Repository.Common;
using System.Threading.Tasks;

namespace BugTracker.Repository.Interfaces
{
	public interface IBugReportStatesRepository : IAdd<BugState>,
		IGetById<BugState>, IGetAllById<BugState>
	{
		Task<BugState> GetLatestState(int bugReportId);
	}
}
