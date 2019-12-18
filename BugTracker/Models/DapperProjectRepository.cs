using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
	public class DapperProjectRepository : IProjectRepository
	{
		public Project Add(Project project)
		{
			throw new NotImplementedException();
		}

		public Project Delete(int Id)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Project> GetAllProjects()
		{
			throw new NotImplementedException();
		}

		public Project GetProject(int Id)
		{
			throw new NotImplementedException();
		}

		public Project Update(Project projectChanges)
		{
			throw new NotImplementedException();
		}
	}
}
