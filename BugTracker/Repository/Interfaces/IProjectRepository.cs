using BugTracker.Models;
using BugTracker.Repository.Common;
using System.Collections.Generic;

namespace BugTracker.Repository.Interfaces
{
	public interface IProjectRepository : IAdd<Project>,
		IUpdate<Project>, IDelete<Project>,
		IGetById<Project>
	{
		IEnumerable<Project> GetAll();
	}
}
