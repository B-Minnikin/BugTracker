﻿using System;
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

		// Bug Reports
		BugReport AddBugReport(BugReport bugReport);
		IEnumerable<BugReport> GetAllBugReports(int projectId);
		BugReport GetBugReportById(int bugReportId);
		BugReport UpdateBugReport(BugReport reportChanges);
		BugReport DeleteBugReport(int id);
		int GetCommentCountById(int bugReportId);

		// Bug Report Comments
		BugReportComment CreateComment(BugReportComment bugReportComment);
		IEnumerable<BugReportComment> GetBugReportComments(int bugReportId);
		BugReportComment GetBugReportCommentById(int bugReportCommentId);
		BugReportComment UpdateBugReportComment(BugReportComment bugReportCommentChanges);
		void DeleteComment(int bugReportCommentId);
		int GetCommentParentId(int bugReportCommentId);

		// Bug Report States
		IEnumerable<BugState> GetBugStates(int bugReportId);

		// Attachment Paths
		IEnumerable<AttachmentPath> GetAttachmentPaths(AttachmentParentType parentType, int parentId);
	}
}
