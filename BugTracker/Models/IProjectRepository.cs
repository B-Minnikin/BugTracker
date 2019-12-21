using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
	public interface IProjectRepository
	{
		Project GetProjectById(int Id);
		IEnumerable<Project> GetAllProjects();
		Project CreateProject(Project project);
		BugReport AddBugReport(BugReport bugReport);
		Project Update(Project projectChanges);
		Project DeleteProject(int Id);
	}
}
