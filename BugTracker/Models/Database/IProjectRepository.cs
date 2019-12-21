using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
	public interface IProjectRepository
	{
		// Projects
		Project GetProjectById(int Id);
		IEnumerable<Project> GetAllProjects();
		Project CreateProject(Project project);
		Project UpdateProject(Project projectChanges);
		Project DeleteProject(int Id);

		// Bug Reports
		BugReport AddBugReport(BugReport bugReport);
		IEnumerable<BugReport> GetAllBugReports(int ProjectId);
	}
}
