using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models.ProjectInvitation
{
	public class ProjectInvitation : IProjectInvitation
	{
		public void AddProjectInvitation(string emailAddress, int projectId)
		{
			throw new NotImplementedException();
		}

		public void RemovePendingProjectInvitation(string emailAddress, int projectId)
		{
			throw new NotImplementedException();
		}
	}
}
