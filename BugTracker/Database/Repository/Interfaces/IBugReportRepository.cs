using System.Collections.Generic;
using System.Threading.Tasks;
using BugTracker.Database.Repository.Common;
using BugTracker.Models;

namespace BugTracker.Database.Repository.Interfaces
{
	public interface IBugReportRepository : IAdd<BugReport>,
		IUpdate<BugReport>, IDelete<BugReport>,
		IGetById<BugReport>, IGetAllById<BugReport>
	{
		Task<BugReport> GetBugReportByLocalId(int localBugReportId, int projectId);
		int GetCommentCountById(int bugReportId);

		// Bug Reports - Local IDs
		Task AddLocalBugReportId(int projectId);

		// Users assigned to bug reports
		Task AddUserAssignedToBugReport(string userId, int bugReportId);
		Task DeleteUserAssignedToBugReport(string userId, int bugReportId);
		Task<IEnumerable<BugReport>> GetBugReportsForAssignedUser(string userId);
		Task<IEnumerable<ApplicationUser>> GetAssignedUsersForBugReport(int bugReportId);

		// Bug Report Linking
		Task AddBugReportLink(int bugReportId, int linkToBugReportId);
		Task DeleteBugReportLink(int bugReportId, int linkToBugReportId);
		Task<IEnumerable<BugReport>> GetLinkedReports(int bugReportId);

		// Attachment Paths
		Task<IEnumerable<AttachmentPath>> GetAttachmentPaths(AttachmentParentType parentType, int parentId);
	}
}
