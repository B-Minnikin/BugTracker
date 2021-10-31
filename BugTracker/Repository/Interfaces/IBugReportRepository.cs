using BugTracker.Models;
using BugTracker.Repository.Common;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BugTracker.Repository.Interfaces
{
	public interface IBugReportRepository : IAdd<BugReport>,
		IUpdate<BugReport>, IDelete<BugReport>,
		IGetById<BugReport>, IGetAllById<BugReport>
	{
		Task<BugReport> GetBugReportByLocalId(int localBugReportId, int projectId);
		Task<int> GetCommentCountById(int bugReportId);

		// Bug Reports - Local IDs
		Task AddLocalBugReportId(int projectId);

		// Users assigned to bug reports
		Task AddUserAssignedToBugReport(int userId, int bugReportId);
		Task DeleteUserAssignedToBugReport(int userId, int bugReportId);
		Task<IEnumerable<BugReport>> GetBugReportsForAssignedUser(int userId);
		Task<IEnumerable<IdentityUser>> GetAssignedUsersForBugReport(int bugReportId);

		// Bug Report Linking
		Task AddBugReportLink(int bugReportId, int linkToBugReportId);
		Task DeleteBugReportLink(int bugReportId, int linkToBugReportId);
		Task<IEnumerable<BugReport>> GetLinkedReports(int bugReportId);

		// Attachment Paths
		Task<IEnumerable<AttachmentPath>> GetAttachmentPaths(AttachmentParentType parentType, int parentId);
	}
}
