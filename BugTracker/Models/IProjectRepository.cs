using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
	public interface IProjectRepository
	{
		// Projects
		Project GetProject(int Id);
		IEnumerable<Project> GetAllProjects();
		Project Add(Project project);
		Project Update(Project projectChanges);
		Project Delete(int Id);

		// Bug Reports
		BugReport AddBugReport(BugReport bugReport);
		IEnumerable<BugReport> GetAllBugReports(int ProjectId);
	}
}
