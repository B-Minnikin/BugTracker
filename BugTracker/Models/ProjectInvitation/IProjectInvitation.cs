using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models.ProjectInvitation
{
	interface IProjectInvitation
	{
		void AddProjectInvitation(string emailAddress, int projectId);
		void RemovePendingProjectInvitation(string emailAddress, int projectId);
	}
}
