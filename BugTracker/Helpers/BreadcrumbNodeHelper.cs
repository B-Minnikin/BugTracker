using BugTracker.Models;
using SmartBreadcrumbs.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Helpers
{
	public static class BreadcrumbNodeHelper
	{
		public static MvcBreadcrumbNode BugReportOverview(Project project, string bugReportTitle)
		{
			var projectsNode = new MvcBreadcrumbNode("Projects", "Projects", "Projects");
			var overviewNode = new MvcBreadcrumbNode("Overview", "Projects", project.Name)
			{
				RouteValues = new { id = project.ProjectId },
				Parent = projectsNode
			};
			var reportNode = new MvcBreadcrumbNode("CreateReport", "BugReport", bugReportTitle)
			{
				Parent = overviewNode
			};
			return reportNode;
		}

		public static MvcBreadcrumbNode BugReportManageLinks(Project project, BugReport bugReport)
		{
			var projectsNode = new MvcBreadcrumbNode("Projects", "Projects", "Projects");
			var overviewNode = new MvcBreadcrumbNode("Overview", "Projects", project.Name)
			{
				RouteValues = new { id = project.ProjectId },
				Parent = projectsNode
			};
			var reportNode = new MvcBreadcrumbNode("ReportOverview", "BugReport", bugReport.Title)
			{
				RouteValues = new { id = bugReport.BugReportId },
				Parent = overviewNode
			};
			var manageLinksNode = new MvcBreadcrumbNode("ManageLinks", "BugReport", "Manage Links")
			{
				Parent = reportNode
			};
			return manageLinksNode;
		}

		public static MvcBreadcrumbNode BugReportAssignMember(Project project, BugReport bugReport)
		{
			var projectsNode = new MvcBreadcrumbNode("Projects", "Projects", "Projects");
			var overviewNode = new MvcBreadcrumbNode("Overview", "Projects", project.Name)
			{
				RouteValues = new { id = project.ProjectId },
				Parent = projectsNode
			};
			var reportNode = new MvcBreadcrumbNode("ReportOverview", "BugReport", bugReport.Title)
			{
				RouteValues = new { id = bugReport.BugReportId },
				Parent = overviewNode
			};
			var assignMembersNode = new MvcBreadcrumbNode("AssignMember", "BugReport", "Assign Members")
			{
				Parent = reportNode
			};
			return assignMembersNode;
		}

		public static MvcBreadcrumbNode BugReportEdit(Project project, BugReport bugReport)
		{
			var projectsNode = new MvcBreadcrumbNode("Projects", "Projects", "Projects");
			var overviewNode = new MvcBreadcrumbNode("Overview", "Projects", project.Name)
			{
				RouteValues = new { id = project.ProjectId },
				Parent = projectsNode
			};
			var reportNode = new MvcBreadcrumbNode("ReportOverview", "BugReport", bugReport.Title)
			{
				RouteValues = new { id = bugReport.BugReportId },
				Parent = overviewNode
			};
			var editNode = new MvcBreadcrumbNode("Edit", "BugReport", "Edit")
			{
				Parent = reportNode
			};
			return editNode;
		}

		public static MvcBreadcrumbNode BugReportCreate(Project project)
		{
			var projectsNode = new MvcBreadcrumbNode("Projects", "Projects", "Projects");
			var overviewNode = new MvcBreadcrumbNode("Overview", "Projects", project.Name)
			{
				RouteValues = new { id = project.ProjectId },
				Parent = projectsNode
			};
			var reportNode = new MvcBreadcrumbNode("CreateReport", "BugReport", "Create Bug Report")
			{
				Parent = overviewNode
			};
			return reportNode;
		}

		public static MvcBreadcrumbNode CommentEdit(Project project, BugReport bugReport)
		{
			var projectsNode = new MvcBreadcrumbNode("Projects", "Projects", "Projects");
			var overviewNode = new MvcBreadcrumbNode("Overview", "Projects", project.Name)
			{
				RouteValues = new { id = project.ProjectId },
				Parent = projectsNode
			};
			var reportNode = new MvcBreadcrumbNode("ReportOverview", "BugReport", bugReport.Title)
			{
				RouteValues = new { id = bugReport.BugReportId },
				Parent = overviewNode
			};
			var commentNode = new MvcBreadcrumbNode("Edit", "Comment", "Edit Comment")
			{
				Parent = reportNode
			};
			return commentNode;
		}

		public static MvcBreadcrumbNode CommentCreate(Project project, BugReport bugReport)
		{
			var projectsNode = new MvcBreadcrumbNode("Projects", "Projects", "Projects");
			var overviewNode = new MvcBreadcrumbNode("Overview", "Projects", project.Name)
			{
				RouteValues = new { id = project.ProjectId },
				Parent = projectsNode
			};
			var reportNode = new MvcBreadcrumbNode("ReportOverview", "BugReport", bugReport.Title)
			{
				RouteValues = new { id = bugReport.BugReportId },
				Parent = overviewNode
			};
			var commentNode = new MvcBreadcrumbNode("Create", "Comment", "Comment")
			{
				Parent = reportNode
			};
			return commentNode;
		}
	}
}
