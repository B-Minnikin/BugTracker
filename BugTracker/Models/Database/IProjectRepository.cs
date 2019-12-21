using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
	public interface IProjectRepository
	{
		// Projects
		Project GetProjectById(int Id);
		IEnumerable<Project> GetAllProjects();
		Project CreateProject(Project project);
		Project UpdateProject(Project projectChanges);
		Project DeleteProject(int Id);

		// Bug Reports
		BugReport AddBugReport(BugReport bugReport);
		IEnumerable<BugReport> GetAllBugReports(int ProjectId);
		BugReport GetBugReportById(int BugReportId);
		BugReport UpdateBugReport(BugReport reportChanges);

		// Bug Report Comments
		BugReportComment CreateComment(BugReportComment bugReportComment);
		IEnumerable<BugReportComment> GetBugReportComments(int bugReportId);
		BugReportComment GetBugReportCommentById(int bugReportCommentId);

		// Bug Report States
		IEnumerable<BugState> GetBugStates(int bugReportId);

		// Attachment Paths
		IEnumerable<AttachmentPath> GetAttachmentPaths(AttachmentParentType parentType, int parentId);
	}
}
