using BugTracker.Models.Authorization;
using BugTracker.Repository;
using BugTracker.Repository.Interfaces;
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
		private readonly IBugReportRepository bugReportRepository;
		private readonly IUserSubscriptionsRepository userSubscriptionsRepository;
		private readonly IEmailHelper emailHelper;
		private readonly ApplicationUserManager userManager;

		public Subscriptions(IProjectRepository projectRepository,
			IBugReportRepository bugReportRepository,
			IUserSubscriptionsRepository userSubscriptionsRepository,
			IEmailHelper emailHelper)
		{
			this.projectRepository = projectRepository;
			this.bugReportRepository = bugReportRepository;
			this.userSubscriptionsRepository = userSubscriptionsRepository;
			this.emailHelper = emailHelper;
			userManager = new ApplicationUserManager();
		}

		public bool IsSubscribed(int userId, int bugReportId)
		{
			return userSubscriptionsRepository.IsSubscribed(userId, bugReportId);
		}

		public void CreateSubscriptionIfNotSubscribed(int userId, int bugReportId)
		{
			if(!IsSubscribed(userId, bugReportId))
			{
				userSubscriptionsRepository.AddSubscription(userId, bugReportId);
			}
		}

		public void DeleteSubscription(int userId, int bugReportId)
		{
			if(IsSubscribed(userId, bugReportId))
			{
				userSubscriptionsRepository.DeleteSubscription(userId, bugReportId);
			}
		}

		public async Task NotifyBugReportStateChanged(BugState bugState, string bugReportUrl)
		{
			var subscribedUserIds = userSubscriptionsRepository.GetAllSubscribedUserIds(bugState.BugReportId);

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

		public async Task NotifyBugReportNewComment(Comment comment, string bugReportUrl)
		{
			var subscribedUserIds = userSubscriptionsRepository.GetAllSubscribedUserIds(comment.BugReportId);

			var bugReport = bugReportRepository.GetById(comment.BugReportId);
			string projectName = projectRepository.GetById(bugReport.ProjectId).Name;
			string emailSubject = $"Bug report updated: {bugReport.Title}";
			string emailMessage = $"Project: {projectName}\nNew comment posted in bug report {bugReport.Title} by {comment.AuthorId}.\n" +
				$"Please <a href=\"{ bugReportUrl}\">click here</a> to review new content.";

			foreach (var userId in subscribedUserIds)
			{
				IdentityUser user = await userManager.FindByIdAsync(userId.ToString());

				if(comment.AuthorId != int.Parse(user.Id))
				{
					emailHelper.Send(user.UserName, user.Email, emailSubject, emailMessage);
				}
			}
		}

		private string ComposeBugStateEmailSubject(BugState bugState)
		{
			var bugReport = bugReportRepository.GetById(bugState.BugReportId);
			string message = $"Bug report updated: {bugReport.Title}";

			return message;
		}

		private string ComposeBugStateEmailMessage(BugState bugState, string bugReportUrl)
		{
			var bugReport = bugReportRepository.GetById(bugState.BugReportId);
			string projectName = projectRepository.GetById(bugReport.ProjectId).Name;
			string stateName = bugState.StateType.ToString().First().ToString().ToUpper() + bugState.StateType.ToString().Substring(1);

			string message = $"Project: {projectName}\n{bugReport.Title} state updated to {stateName}\n" +
				$"Please <a href=\"{bugReportUrl}\">click here</a> to review new content.";

			return message;
		}
	}
}
