using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
	public class ProjectRepository
	{
		private readonly BugTrackerDbContext _context;

		public ProjectRepository(BugTrackerDbContext context)
		{
			this._context = context;
		}

		public Project Add(Project project)
		{
			_context.Projects.Add(project);
			_context.SaveChanges();
			return project;
		}

		public Project Delete(int Id)
		{
			Project project = _context.Projects.Find(Id);
			if (project != null)
			{
				_context.Projects.Remove(project);
				_context.SaveChanges();
			}
			return project;
		}

		public IEnumerable<Project> GetAllProjects()
		{
			return _context.Projects;
		}

		public Project GetProject(int Id)
		{
			return _context.Projects.Find(Id);
		}

		public Project Update(Project projectChanges)
		{
			var project = _context.Projects.Attach(projectChanges);
			project.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
			_context.SaveChanges();
			return projectChanges;
		}
	}
}
