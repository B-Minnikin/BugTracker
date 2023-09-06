using System.Collections.Generic;
using System.Threading.Tasks;

namespace BugTracker.Database.Repository.Interfaces
{
	public interface IProjectInvitationsRepository
	{
		Task AddPendingProjectInvitation(string emailAddress, int projectId);
		Task DeletePendingProjectInvitation(string emailAddress, int projectId);
		Task<bool> IsEmailAddressPendingRegistration(string emailAddress, int projectId);
		Task<IEnumerable<int>> GetProjectInvitationsForEmailAddress(string emailAddress);
	}
}
