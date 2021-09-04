using BugTracker.Models;
using BugTracker.Repository.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Repository.Interfaces
{
	public interface IBugReportStatesRepository : IAdd<BugState>,
		IGetById<BugState>, IGetAllById<BugState>
	{
		BugState GetLatestState(int bugReportId);
	}
}
