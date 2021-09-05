using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Repository.Interfaces
{
	public interface IProjectInvitationsRepository
	{
		void AddPendingProjectInvitation(string emailAddress, int projectId);
		void DeletePendingProjectInvitation(string emailAddress, int projectId);
		bool IsEmailAddressPendingRegistration(string emailAddress, int projectId);
		IEnumerable<int> GetProjectInvitationsForEmailAddress(string emailAddress);
	}
}
