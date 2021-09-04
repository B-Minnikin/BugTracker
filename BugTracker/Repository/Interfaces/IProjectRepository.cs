using Microsoft.AspNetCore.Identity;
using Org.BouncyCastle.Bcpg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
	public interface IProjectRepository
	{
		// Projects
		Project GetProjectById(int id);
		IEnumerable<Project> GetAllProjects();
		Project CreateProject(Project project);
		Project UpdateProject(Project projectChanges);
		Project DeleteProject(int id);

		// Bug Reports - Local IDs
		void CreateLocalBugReportId(int projectId);

		// Users assigned to bug reports
		void AddUserAssignedToBugReport(int userId, int bugReportId);
		void RemoveUserAssignedToBugReport(int userId, int bugReportId);
		IEnumerable<BugReport> GetBugReportsForAssignedUser(int userId);
		IEnumerable<IdentityUser> GetAssignedUsersForBugReport(int bugReportId);

		// Bug Report Comments
		BugReportComment CreateComment(BugReportComment bugReportComment);
		IEnumerable<BugReportComment> GetBugReportComments(int bugReportId);
		BugReportComment GetBugReportCommentById(int bugReportCommentId);
		BugReportComment UpdateBugReportComment(BugReportComment bugReportCommentChanges);
		void DeleteComment(int bugReportCommentId);
		int GetCommentParentId(int bugReportCommentId);

		// Bug Report Linking
		void AddBugReportLink(int bugReportId, int linkToBugReportId);
		void RemoveBugReportLink(int bugReportId, int linkToBugReportId);
		IEnumerable<BugReport> GetLinkedReports(int bugReportId);

		// Attachment Paths
		IEnumerable<AttachmentPath> GetAttachmentPaths(AttachmentParentType parentType, int parentId);

		// Project Invitations
		void CreatePendingProjectInvitation(string emailAddress, int projectId);
		void RemovePendingProjectInvitation(string emailAddress, int projectId);
		bool IsEmailAddressPendingRegistration(string emailAddress, int projectId);
		IEnumerable<int> GetProjectInvitationsForEmailAddress(string emailAddress);

		// Search
		IEnumerable<UserTypeaheadSearchResult> GetMatchingProjectMembersBySearchQuery(string query, int projectId);
		IEnumerable<BugReportTypeaheadSearchResult> GetMatchingBugReportsByLocalIdSearchQuery(int localBugReportId, int projectId);
		IEnumerable<BugReportTypeaheadSearchResult> GetMatchingBugReportsByTitleSearchQuery(string query, int projectId);

		// Activities
		void AddActivity(Activity activity);
		void RemoveActivity(int activityId);
		IEnumerable<Activity> GetUserActivities(int userId);
		IEnumerable<Activity> GetBugReportActivities(int bugReportId);
	}
}
