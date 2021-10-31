using BugTracker.Models;
using BugTracker.Repository.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BugTracker.Repository.Interfaces
{
	public interface IProjectRepository : IAdd<Project>,
		IUpdate<Project>, IDelete<Project>,
		IGetById<Project>
	{
		Task<IAsyncEnumerable<Project>> GetAll();
	}
}
