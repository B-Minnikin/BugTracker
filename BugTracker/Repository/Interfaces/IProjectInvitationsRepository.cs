using System.Collections.Generic;

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
