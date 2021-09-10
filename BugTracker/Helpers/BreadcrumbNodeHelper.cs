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

		public static MvcBreadcrumbNode MilestoneEdit(Project project)
		{
			var projectsNode = new MvcBreadcrumbNode("Projects", "Projects", "Projects");
			var overviewNode = new MvcBreadcrumbNode("Overview", "Projects", project.Name)
			{
				RouteValues = new { id = project.ProjectId },
				Parent = projectsNode
			};
			var milestonesNode = new MvcBreadcrumbNode("Milestones", "Milestone", "Milestones")
			{
				RouteValues = new { projectId = project.ProjectId },
				Parent = overviewNode
			};
			var editMilestoneNode = new MvcBreadcrumbNode("Edit", "Milestone", "Edit")
			{
				Parent = milestonesNode
			};
			return editMilestoneNode;
		}

		public static MvcBreadcrumbNode MilestoneCreate(Project project)
		{
			var projectsNode = new MvcBreadcrumbNode("Projects", "Projects", "Projects");
			var overviewNode = new MvcBreadcrumbNode("Overview", "Projects", project.Name)
			{
				RouteValues = new { id = project.ProjectId },
				Parent = projectsNode
			};
			var milestonesNode = new MvcBreadcrumbNode("Milestones", "Milestone", "Milestones")
			{
				RouteValues = new { projectId = project.ProjectId },
				Parent = overviewNode
			};
			var newMilestoneNode = new MvcBreadcrumbNode("New", "Milestone", "New")
			{
				Parent = milestonesNode
			};
			return newMilestoneNode;
		}

		public static MvcBreadcrumbNode MilestoneOverview(Project project, string milestoneTitle)
		{
			var projectsNode = new MvcBreadcrumbNode("Projects", "Projects", "Projects");
			var overviewNode = new MvcBreadcrumbNode("Overview", "Projects", project.Name)
			{
				RouteValues = new { id = project.ProjectId },
				Parent = projectsNode
			};
			var milestonesNode = new MvcBreadcrumbNode("Milestones", "Milestone", "Milestones")
			{
				RouteValues = new { projectId = project.ProjectId },
				Parent = overviewNode
			};
			var chosenMilestoneNode = new MvcBreadcrumbNode("Overview", "Milestone", milestoneTitle)
			{
				Parent = milestonesNode
			};
			return chosenMilestoneNode;
		}

		public static MvcBreadcrumbNode Milestones(Project project)
		{
			var projectsNode = new MvcBreadcrumbNode("Projects", "Projects", "Projects");
			var overviewNode = new MvcBreadcrumbNode("Overview", "Projects", project.Name)
			{
				RouteValues = new { id = project.ProjectId },
				Parent = projectsNode
			};
			var milestonesNode = new MvcBreadcrumbNode("Milestones", "Milestone", "Milestones")
			{
				RouteValues = new { projectId = project.ProjectId },
				Parent = overviewNode
			};
			return milestonesNode;
		}

		public static MvcBreadcrumbNode ProfileEdit(int profileId)
		{
			var profileNode = new MvcBreadcrumbNode("View", "Profile", "My Profile")
			{
				RouteValues = new { id = profileId }
			};
			var editNode = new MvcBreadcrumbNode("Edit", "Profile", "Edit")
			{
				Parent = profileNode
			};
			return editNode;
		}

		public static MvcBreadcrumbNode ProfileSubscriptions(int profileId)
		{
			var profileNode = new MvcBreadcrumbNode("View", "Profile", "My Profile")
			{
				RouteValues = new { id = profileId }
			};
			var subscriptionsNode = new MvcBreadcrumbNode("Subscriptions", "Profile", "Subscriptions")
			{
				Parent = profileNode
			};
			return subscriptionsNode;
		}
	}
}
