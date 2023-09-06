using System.Collections.Generic;
using System.Threading.Tasks;
using BugTracker.Database.Repository.Common;
using BugTracker.Models;

namespace BugTracker.Database.Repository.Interfaces
{
	public interface IProjectRepository : IAdd<Project>,
		IUpdate<Project>, IDelete<Project>,
		IGetById<Project>
	{
		Task<IEnumerable<Project>> GetAll();
	}
}
