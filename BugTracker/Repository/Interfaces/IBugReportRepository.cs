using BugTracker.Models;
using BugTracker.Repository.Common;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace BugTracker.Repository.Interfaces
{
	public interface IBugReportRepository : IAdd<BugReport>,
		IUpdate<BugReport>, IDelete<BugReport>,
		IGetById<BugReport>, IGetAllById<BugReport>
	{
		BugReport GetBugReportByLocalId(int localBugReportId, int projectId);
		int GetCommentCountById(int bugReportId);

		// Bug Reports - Local IDs
		void AddLocalBugReportId(int projectId);

		// Users assigned to bug reports
		void AddUserAssignedToBugReport(int userId, int bugReportId);
		void DeleteUserAssignedToBugReport(int userId, int bugReportId);
		IEnumerable<BugReport> GetBugReportsForAssignedUser(int userId);
		IEnumerable<IdentityUser> GetAssignedUsersForBugReport(int bugReportId);

		// Bug Report Linking
		void AddBugReportLink(int bugReportId, int linkToBugReportId);
		void DeleteBugReportLink(int bugReportId, int linkToBugReportId);
		IEnumerable<BugReport> GetLinkedReports(int bugReportId);

		// Attachment Paths
		IEnumerable<AttachmentPath> GetAttachmentPaths(AttachmentParentType parentType, int parentId);
	}
}
