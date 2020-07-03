using BugTracker.Models.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models.Database
{
	public class SubscriptionHelper : ISubscriptionHelper
	{
		private readonly IProjectRepository projectRepository;
		private readonly IEmailHelper emailHelper;
		private readonly ApplicationUserManager userManager;

		public SubscriptionHelper(IProjectRepository projectRepository,
			IEmailHelper emailHelper)
		{
			this.projectRepository = projectRepository;
			this.emailHelper = emailHelper;
			this.userManager = new ApplicationUserManager();
		}

		public bool IsSubscribed(int userId, int bugReportId)
		{
			return projectRepository.IsSubscribed(userId, bugReportId);
		}

		public async void NotifyBugReportStateChanged(int bugReportId, BugState bugState)
		{
			var subscribedUserIds = projectRepository.GetAllSubscribedUserIds(bugReportId);

			foreach(var userId in subscribedUserIds)
			{
				IdentityUser user = await userManager.FindByIdAsync(userId.ToString());

				if (bugState.Author != user.UserName)
				{
					string emailSubject = ComposeEmailSubject(bugState); // TODO - LAMBDA METHOD
					string emailMessage = ComposeEmailMessage(bugState); // move out of loop

					emailHelper.Send(user.UserName, user.Email, emailSubject, emailMessage);
				}
			}
		}

		public async void NotifyBugReportNewComment(BugReportComment bugReportComment)
		{
			var subscribedUserIds = projectRepository.GetAllSubscribedUserIds(bugReportComment.BugReportId);

			foreach(var userId in subscribedUserIds)
			{
				IdentityUser user = await userManager.FindByIdAsync(userId.ToString());

				if(bugReportComment.Author != user.UserName)
				{
					var bugReport = projectRepository.GetBugReportById(bugReportComment.BugReportId);
					string emailSubject = $"Bug report update: {bugReport.Title}";
					string emailMessage = "New comment in bug report" + bugReport.Title + ":\n\t" + bugReportComment.MainText;

					emailHelper.Send(user.UserName, user.Email, emailSubject, emailMessage);
				}
			}
		}

		private string ComposeEmailSubject(BugState bugState)
		{
			var bugReport = projectRepository.GetBugReportById(bugState.BugReportId);
			string message = $"Bug report update: {bugReport.Title}";

			return message;
		}

		private string ComposeEmailMessage(BugState bugState)
		{
			var bugReport = projectRepository.GetBugReportById(bugState.BugReportId);
			string message = $"Bug state updated: {bugReport.Title}\n\t{bugState.StateType.ToString()}";

			return message;
		}
	}
}
