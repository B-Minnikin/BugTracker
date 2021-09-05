using BugTracker.Repository;
using BugTracker.Repository.Common;
using Microsoft.AspNetCore.Identity;
using Org.BouncyCastle.Bcpg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
	public interface IProjectRepository : IAdd<Project>,
		IUpdate<Project>, IDelete<Project>,
		IGetById<Project>
	{
		// Projects
		//Project GetProjectById(int id);
		IEnumerable<Project> GetAll();
		//Project CreateProject(Project project);
		//Project UpdateProject(Project projectChanges);
		//Project DeleteProject(int id);
	}
}
