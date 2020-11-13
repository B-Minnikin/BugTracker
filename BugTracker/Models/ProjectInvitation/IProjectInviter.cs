using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models.ProjectInvitation
{
	public interface IProjectInviter
	{
		Task AddProjectInvitation(ProjectInvitation projectInvitation);
		void RemovePendingProjectInvitation(string emailAddress, int projectId);

		void AddUserToProjectMemberRoleForAllPendingInvitations(string emailAddress);
	}
}
