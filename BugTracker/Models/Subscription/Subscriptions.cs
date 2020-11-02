using BugTracker.Models.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models.Database
{
	public class Subscriptions : ISubscriptions
	{
		private readonly IProjectRepository projectRepository;
		private readonly IEmailHelper emailHelper;
		private readonly ApplicationUserManager userManager;

		public Subscriptions(IProjectRepository projectRepository,
			IEmailHelper emailHelper)
		{
			this.projectRepository = projectRepository;
			this.emailHelper = emailHelper;
			userManager = new ApplicationUserManager();
		}

		public bool IsSubscribed(int userId, int bugReportId)
		{
			return projectRepository.IsSubscribed(userId, bugReportId);
		}

		public void CreateSubscriptionIfNotSubscribed(int userId, int bugReportId)
		{
			if(!IsSubscribed(userId, bugReportId))
			{
				projectRepository.CreateSubscription(userId, bugReportId);
			}
		}

		public void DeleteSubscription(int userId, int bugReportId)
		{
			if(IsSubscribed(userId, bugReportId))
			{
				projectRepository.DeleteSubscription(userId, bugReportId);
			}
		}

		public async void NotifyBugReportStateChanged(BugState bugState, string bugReportUrl)
		{
			var subscribedUserIds = projectRepository.GetAllSubscribedUserIds(bugState.BugReportId);

			string emailSubject = ComposeBugStateEmailSubject(bugState);
			string emailMessage = ComposeBugStateEmailMessage(bugState, bugReportUrl);

			foreach (var userId in subscribedUserIds)
			{
				IdentityUser user = await userManager.FindByIdAsync(userId.ToString());

				if (bugState.Author != user.UserName)
				{
					emailHelper.Send(user.UserName, user.Email, emailSubject, emailMessage);
				}
			}
		}

		public async void NotifyBugReportNewComment(BugReportComment bugReportComment, string bugReportUrl)
		{
			var subscribedUserIds = projectRepository.GetAllSubscribedUserIds(bugReportComment.BugReportId);

			var bugReport = projectRepository.GetBugReportById(bugReportComment.BugReportId);
			string projectName = projectRepository.GetProjectById(bugReport.ProjectId).Name;
			string emailSubject = $"Bug report updated: {bugReport.Title}";
			string emailMessage = $"Project: {projectName}\nNew comment posted in bug report {bugReport.Title} by {bugReportComment.Author}.\n" +
				$"Please <a href=\"{ bugReportUrl}\">click here</a> to review new content.";

			foreach (var userId in subscribedUserIds)
			{
				IdentityUser user = await userManager.FindByIdAsync(userId.ToString());

				if(bugReportComment.Author != user.UserName)
				{
					emailHelper.Send(user.UserName, user.Email, emailSubject, emailMessage);
				}
			}
		}

		private string ComposeBugStateEmailSubject(BugState bugState)
		{
			var bugReport = projectRepository.GetBugReportById(bugState.BugReportId);
			string message = $"Bug report updated: {bugReport.Title}";

			return message;
		}

		private string ComposeBugStateEmailMessage(BugState bugState, string bugReportUrl)
		{
			var bugReport = projectRepository.GetBugReportById(bugState.BugReportId);
			string projectName = projectRepository.GetProjectById(bugReport.ProjectId).Name;
			string stateName = bugState.StateType.ToString().First().ToString().ToUpper() + bugState.StateType.ToString().Substring(1);

			string message = $"Project: {projectName}\n{bugReport.Title} state updated to {stateName}\n" +
				$"Please <a href=\"{bugReportUrl}\">click here</a> to review new content.";

			return message;
		}
	}
}
