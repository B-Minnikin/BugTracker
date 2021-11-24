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
using BugTracker.Repository;
using BugTracker.Repository.Interfaces;

namespace BugTracker.Services
{
	public class ActivityMessageBuilder : IActivityMessageBuilder
	{
		private readonly ILinkGenerator linkGenerator;
		private readonly UserManager<IdentityUser> userManager;
		private readonly IProjectRepository projectRepository;
		private readonly IBugReportRepository bugReportRepository;
		private readonly IMilestoneRepository milestoneRepository;
		private readonly IBugReportStatesRepository bugReportStatesRepository;

		public ActivityMessageBuilder(ILinkGenerator linkGenerator,
			UserManager<IdentityUser> userManager,
			IProjectRepository projectRepository,
			IBugReportRepository bugReportRepository,
			IMilestoneRepository milestoneRepository,
			IBugReportStatesRepository bugReportStatesRepository)
		{
			this.linkGenerator = linkGenerator;
			this.userManager = userManager;
			this.projectRepository = projectRepository;
			this.bugReportRepository = bugReportRepository;
			this.milestoneRepository = milestoneRepository;
			this.bugReportStatesRepository = bugReportStatesRepository;
		}

		public async Task GenerateMessages(IEnumerable<Activity> activities)
		{
			if (activities == null) throw new ArgumentNullException("Unable to generate activity messages. Activity collection is null.");

			foreach (var activity in activities)
			{
				activity.ActivityMessage = await this.GetMessage(activity);
			}
		}

		public async Task<string> GetMessage(Activity activity)
		{
			if (activity == null) throw new ArgumentNullException("Unable to generate activity message. Activity is null.");

			string message = $"{activity.Timestamp} ";
			var usernameAnchorString = await GetUserAnchorString(activity);

			switch (activity.MessageId)
			{
				case ActivityMessage.ProjectCreated:
					message += $"{usernameAnchorString} created a new project: {await GetProjectAnchorString (activity)}.";
					break;
				case ActivityMessage.ProjectEdited:
					message += $"{usernameAnchorString} edited a project: {await GetProjectAnchorString (activity)}.";
					break;
				case ActivityMessage.BugReportPosted:
					message += $"{usernameAnchorString} posted a bug report: {await GetBugReportAnchorString (activity)}.";
					break;
				case ActivityMessage.BugReportEdited:
					message += $"{usernameAnchorString} edited a bug report: {await GetBugReportAnchorString (activity)}.";
					break;
				case ActivityMessage.CommentPosted:
					message += $"{usernameAnchorString} posted a new comment in bug report: {await GetBugReportAnchorString(activity)}.";
					break;
				case ActivityMessage.CommentEdited:
					message += $"{usernameAnchorString} edited their comment in bug report: {await GetBugReportAnchorString (activity)}.";
					break;
				case ActivityMessage.BugReportStateChanged:
					message += $"{usernameAnchorString} changed bug report in {await GetBugReportAnchorString (activity)} state from " +
						$"{await GetBugReportStateAnchorString (activity, nameof(ActivityBugReportStateChange.PreviousBugReportStateId))} " +
						$"to {await GetBugReportStateAnchorString (activity, nameof(ActivityBugReportStateChange.NewBugReportStateId))}.";
					break;
				case ActivityMessage.BugReportsLinked:
					message += $"{usernameAnchorString} linked {await GetBugReportAnchorString (activity)} to {await GetSecondBugReportAnchorString (activity)}.";
					break;
				case ActivityMessage.BugReportAssignedToUser:
					message += $"{usernameAnchorString} assigned {await GetAssignedUsernameAnchorString (activity)} to {await GetBugReportAnchorString (activity)}.";
					break;
				case ActivityMessage.MilestonePosted:
					message += $"{usernameAnchorString} posted a new milestone: {await GetMilestoneAnchorString (activity)}.";
					break;
				case ActivityMessage.MilestoneEdited:
					message += $"{usernameAnchorString} edited milestone: {await GetMilestoneAnchorString (activity)}.";
					break;
				case ActivityMessage.BugReportAddedToMilestone:
					message += $"{usernameAnchorString} added the bug report {await GetBugReportAnchorString (activity)} to" +
						$"milestone: {await GetMilestoneAnchorString (activity)}.";
					break;
				case ActivityMessage.BugReportRemovedFromMilestone:
					message += $"{usernameAnchorString} removed the bug report {await GetBugReportAnchorString(activity)} from " +
						$"milestone: {await GetMilestoneAnchorString(activity)}.";
					break;
				default:
					throw new ArgumentNullException("Unable to generate activity message. No matching case.");
			}

			return message;
	}

		private async Task<string> GetUserAnchorString(Activity activity)
		{
			var user = await userManager.FindByIdAsync(activity.UserId.ToString());
			string userUri = linkGenerator.GetPathByAction("View", "Profile", new { id = activity.UserId });
			string userName = user.UserName;
			var userNameAnchorString = GetHTMLAnchorString(userUri, userName);
			return userNameAnchorString;
		}

		private async Task<string> GetProjectAnchorString(Activity activity)
		{
			var projectId = activity.GetDerivedProperty<int>(nameof(Activity.ProjectId));
			string projectUri = linkGenerator.GetUriByAction("Overview", "Projects", new { id = projectId });
			var project = await projectRepository .GetById(projectId);
			string projectName = project.Name;
			string projectAnchorString = GetHTMLAnchorString(projectUri, projectName);
			return projectAnchorString;
		}

		private async Task<string> GetBugReportAnchorString(Activity activity)
		{
			var bugReportId = activity.GetDerivedProperty<int>(nameof(ActivityBugReport.BugReportId));
			string bugReportUri = linkGenerator.GetUriByAction("ReportOverview", "BugReport", new { id = bugReportId });
			var bugReport = await bugReportRepository.GetById(bugReportId);
			string bugReportName = bugReport.Title;
			string bugReportAnchorString = GetHTMLAnchorString(bugReportUri, bugReportName);
			return bugReportAnchorString;
		}

		private async Task<string> GetSecondBugReportAnchorString(Activity activity)
		{
			var secondBugReportId = activity.GetDerivedProperty<int>(nameof(ActivityBugReportLink.LinkedBugReportId));
			string secondBugReportUri = linkGenerator.GetUriByAction("ReportOverview", "BugReport", new { id = secondBugReportId });
			var secondBugReport = await bugReportRepository.GetById(secondBugReportId);
			string secondBugReportName = secondBugReport.Title;
			string secondBugReportAnchorString = GetHTMLAnchorString(secondBugReportUri, secondBugReportName);
			return secondBugReportAnchorString;
		}

		private async Task<string> GetAssignedUsernameAnchorString(Activity activity)
		{
			var assignedUser = await userManager.FindByIdAsync(activity.UserId.ToString());
			string assignedUserUri = linkGenerator.GetUriByAction("View", "Profile", new { id = assignedUser.Id });
			string assignedUserName = assignedUser.UserName;
			string assignedUserAnchorString = GetHTMLAnchorString(assignedUserUri, assignedUserName);
			return assignedUserAnchorString;
		}

		private async Task<string> GetBugReportStateAnchorString(Activity activity, string propertyName)
		{
			var bugReportStateId = activity.GetDerivedProperty<int>(propertyName);
			var bugState = await bugReportStatesRepository.GetById(bugReportStateId);
			string bugReportStateName = Enum.GetName(typeof(StateType), bugState.StateType);
			return bugReportStateName;
		}

		private async Task<string> GetMilestoneAnchorString(Activity activity)
		{
			var milestoneId = activity.GetDerivedProperty<int>(nameof(ActivityMilestone.MilestoneId));
			string milestoneUri = linkGenerator.GetUriByAction("Overview", "Milestone", new { milestoneId = milestoneId });
			var milestone = await milestoneRepository.GetById(milestoneId);
			string milestoneName = milestone.Title;
			string milestoneAnchorString = GetHTMLAnchorString(milestoneUri, milestoneName);
			return milestoneAnchorString;
		}

		private string GetHTMLAnchorString(string href, string name)
		{
			return "<a href=\"" + href + "\">" + name + "</a>"; 
		}
	}
}
