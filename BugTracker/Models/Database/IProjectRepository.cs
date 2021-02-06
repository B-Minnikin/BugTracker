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

		// Milestones
		Milestone AddMilestone(Milestone milestone);
		Milestone DeleteMilestone(int milestoneId);
		Milestone UpdateMilestone(Milestone milestone);
		IEnumerable<Milestone> GetAllMilestones(int projectId);
		Milestone GetMilestoneById(int id);
		void AddMilestoneBugReport(int milestoneId, int bugReportId);
		void RemoveMilestoneBugReport(int milestoneId, int bugReportId);

		// Bug Reports - Local IDs
		void CreateLocalBugReportId(int projectId);

		// Bug Reports
		BugReport AddBugReport(BugReport bugReport);
		IEnumerable<BugReport> GetAllBugReports(int projectId);
		BugReport GetBugReportById(int bugReportId);
		BugReport GetBugReportByLocalId(int localBugReportId, int projectId);
		BugReport UpdateBugReport(BugReport reportChanges);
		BugReport DeleteBugReport(int id);
		int GetCommentCountById(int bugReportId);

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

		// Bug Report States
		IEnumerable<BugState> GetBugStates(int bugReportId);
		BugState GetLatestState(int bugReportId);
		BugState CreateBugState(BugState bugState);

		// Bug Report Linking
		void AddBugReportLink(int bugReportId, int linkToBugReportId);
		void RemoveBugReportLink(int bugReportId, int linkToBugReportId);
		IEnumerable<BugReport> GetLinkedReports(int bugReportId);

		// Attachment Paths
		IEnumerable<AttachmentPath> GetAttachmentPaths(AttachmentParentType parentType, int parentId);

		// User Subscriptions
		void CreateSubscription(int userId, int bugReportId);
		IEnumerable<BugReport> GetSubscribedReports(int userId);
		bool IsSubscribed(int userId, int bugReportId);
		void DeleteSubscription(int userId, int bugReportId);
		IEnumerable<int> GetAllSubscribedUserIds(int bugReportId);

		// Project Invitations
		void CreatePendingProjectInvitation(string emailAddress, int projectId);
		void RemovePendingProjectInvitation(string emailAddress, int projectId);
		bool IsEmailAddressPendingRegistration(string emailAddress, int projectId);
		IEnumerable<int> GetProjectInvitationsForEmailAddress(string emailAddress);

		// Search
		IEnumerable<UserTypeaheadSearchResult> GetMatchingProjectMembersBySearchQuery(string query, int projectId);
		IEnumerable<BugReportTypeaheadSearchResult> GetMatchingBugReportsByLocalIdSearchQuery(int localBugReportId, int projectId);
		IEnumerable<BugReportTypeaheadSearchResult> GetMatchingBugReportsByTitleSearchQuery(string query, int projectId);
	}
}
