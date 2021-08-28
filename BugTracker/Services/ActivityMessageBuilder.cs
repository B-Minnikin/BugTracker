using BugTracker.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugTracker.Extension_Methods;
using System.Text;
using BugTracker.Models.Authorization;
using Microsoft.AspNetCore.Identity;

namespace BugTracker.Services
{
	public class ActivityMessageBuilder : IActivityMessageBuilder
	{
		private readonly ILinkGenerator linkGenerator;
		private readonly UserManager<IdentityUser> userManager;
		private readonly IProjectRepository projectRepository;

		public ActivityMessageBuilder(ILinkGenerator linkGenerator,
			UserManager<IdentityUser> userManager,
			IProjectRepository projectRepository)
		{
			this.linkGenerator = linkGenerator;
			this.userManager = userManager;
			this.projectRepository = projectRepository;
		}

		public void GenerateMessages(IEnumerable<Activity> activities)
		{
			if (activities == null) throw new ArgumentNullException("Unable to generate activity messages. Activity collection is null.");

			foreach (var activity in activities)
			{
				activity.ActivityMessage = this.GetMessage(activity);
			}
		}

		public string GetMessage(Activity activity)
		{
			if (activity == null) throw new ArgumentNullException("Unable to generate activity message. Activity is null.");

			string message = $"{activity.Timestamp} ";
			var usernameAnchorString = GetUserAnchorString(activity);

			switch (activity.MessageId)
			{
				case ActivityMessage.ProjectCreated:
					message += $"{usernameAnchorString} created a new project: {GetProjectAnchorString(activity)}.";
					break;
				case ActivityMessage.ProjectEdited:
					message += $"{usernameAnchorString} edited a project: {GetProjectAnchorString(activity)}.";
					break;
				case ActivityMessage.BugReportPosted:
					message += $"{usernameAnchorString} posted a bug report: {GetBugReportAnchorString(activity)}.";
					break;
				case ActivityMessage.BugReportEdited:
					message += $"{usernameAnchorString} edited a bug report: {GetBugReportAnchorString(activity)}.";
					break;
				case ActivityMessage.CommentPosted:
					message += $"{usernameAnchorString} posted a new comment in bug report: {GetBugReportAnchorString(activity)}.";
					break;
				case ActivityMessage.CommentEdited:
					message += $"{usernameAnchorString} edited their comment in bug report: {GetBugReportAnchorString(activity)}.";
					break;
				case ActivityMessage.BugReportStateChanged:
					message += $"{usernameAnchorString} changed bug report in {GetBugReportAnchorString(activity)} state from " +
						$"{GetBugReportStateAnchorString(activity, nameof(ActivityBugReportStateChange.PreviousBugReportStateId))} " +
						$"to {GetBugReportStateAnchorString(activity, nameof(ActivityBugReportStateChange.NewBugReportStateId))}.";
					break;
				case ActivityMessage.BugReportsLinked:
					message += $"{usernameAnchorString} linked {GetBugReportAnchorString(activity)} to {GetSecondBugReportAnchorString(activity)}.";
					break;
				case ActivityMessage.BugReportAssignedToUser:
					message += $"{usernameAnchorString} assigned {GetAssignedUsernameAnchorString(activity)} to {GetBugReportAnchorString(activity)}.";
					break;
				case ActivityMessage.MilestonePosted:
					message += $"{usernameAnchorString} posted a new milestone: {GetMilestoneAnchorString(activity)}.";
					break;
				case ActivityMessage.MilestoneEdited:
					message += $"{usernameAnchorString} edited milestone: {GetMilestoneAnchorString(activity)}.";
					break;
				case ActivityMessage.BugReportAddedToMilestone:
					message += $"{usernameAnchorString} added the bug report {GetBugReportAnchorString(activity)} to" +
						$"milestone: {GetMilestoneAnchorString(activity)}.";
					break;
				case ActivityMessage.BugReportRemovedFromMilestone:
					message += $"{usernameAnchorString} removed the bug report {GetBugReportAnchorString(activity)} from " +
						$"milestone: {GetMilestoneAnchorString(activity)}.";
					break;
				default:
					throw new ArgumentNullException("Unable to generate activity message. No matching case.");
			}

			return message;
		}

		private string GetUserAnchorString(Activity activity)
		{
			var user = userManager.FindByIdAsync(activity.UserId.ToString());
			string userUri = linkGenerator.GetPathByAction("View", "Profile", new { id = activity.UserId });
			string userName = user.Result.UserName;
			var userNameAnchorString = GetHTMLAnchorString(userUri, userName);
			return userNameAnchorString;
		}

		private string GetProjectAnchorString(Activity activity)
		{
			var projectId = activity.GetDerivedProperty<int>(nameof(Activity.ProjectId));
			string projectUri = linkGenerator.GetUriByAction("Overview", "Projects", new { id = projectId });
			string projectName = projectRepository.GetProjectById(projectId).Name;
			string projectAnchorString = GetHTMLAnchorString(projectUri, projectName);
			return projectAnchorString;
		}

		private string GetBugReportAnchorString(Activity activity)
		{
			var bugReportId = activity.GetDerivedProperty<int>(nameof(ActivityBugReport.BugReportId));
			string bugReportUri = linkGenerator.GetUriByAction("ReportOverview", "BugReport", new { id = bugReportId });
			string bugReportName = projectRepository.GetBugReportById(bugReportId).Title;
			string bugReportAnchorString = GetHTMLAnchorString(bugReportUri, bugReportName);
			return bugReportAnchorString;
		}

		private string GetSecondBugReportAnchorString(Activity activity)
		{
			var secondBugReportId = activity.GetDerivedProperty<int>(nameof(ActivityBugReportLink.SecondBugReportId));
			string secondBugReportUri = linkGenerator.GetUriByAction("ReportOverview", "BugReport", new { id = secondBugReportId });
			string secondBugReportName = projectRepository.GetBugReportById(secondBugReportId).Title;
			string secondBugReportAnchorString = GetHTMLAnchorString(secondBugReportUri, secondBugReportName);
			return secondBugReportAnchorString;
		}

		private string GetAssignedUsernameAnchorString(Activity activity)
		{
			var assignedUser = userManager.FindByIdAsync(activity.UserId.ToString());
			string assignedUserUri = linkGenerator.GetUriByAction("View", "Profile", new { id = assignedUser.Id });
			string assignedUserName = assignedUser.Result.UserName;
			string assignedUserAnchorString = GetHTMLAnchorString(assignedUserUri, assignedUserName);
			return assignedUserAnchorString;
		}

		private string GetBugReportStateAnchorString(Activity activity, string propertyName)
		{
			var bugReportStateId = activity.GetDerivedProperty<int>(propertyName);
			var bugState = projectRepository.GetBugStateById(bugReportStateId);
			string bugReportStateName = Enum.GetName(typeof(StateType), bugState.StateType);
			return bugReportStateName;
		}

		private string GetMilestoneAnchorString(Activity activity)
		{
			var milestoneId = activity.GetDerivedProperty<int>(nameof(ActivityMilestone.MilestoneId));
			string milestoneUri = linkGenerator.GetUriByAction("Overview", "Milestone", new { milestoneId = milestoneId });
			string milestoneName = projectRepository.GetMilestoneById(milestoneId).Title;
			string milestoneAnchorString = GetHTMLAnchorString(milestoneUri, milestoneName);
			return milestoneAnchorString;
		}

		private string GetHTMLAnchorString(string href, string name)
		{
			return "<a href=\"" + href + "\">" + name + "</a>"; 
		}
	}
}
