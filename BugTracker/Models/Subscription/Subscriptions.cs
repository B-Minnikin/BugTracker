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
		private readonly UserManager<ApplicationUser> userManager;

		public Subscriptions(IProjectRepository projectRepository,
			IBugReportRepository bugReportRepository,
			IUserSubscriptionsRepository userSubscriptionsRepository,
			IEmailHelper emailHelper,
			UserManager<ApplicationUser> userManager)
		{
			this.projectRepository = projectRepository;
			this.bugReportRepository = bugReportRepository;
			this.userSubscriptionsRepository = userSubscriptionsRepository;
			this.emailHelper = emailHelper;
			this.userManager = userManager;
		}

		public async Task<bool> IsSubscribed(int userId, int bugReportId)
		{
			return await userSubscriptionsRepository.IsSubscribed(userId, bugReportId);
		}

		public async Task CreateSubscriptionIfNotSubscribed(int userId, int bugReportId)
		{
			if(!await IsSubscribed (userId, bugReportId))
			{
				await userSubscriptionsRepository.AddSubscription(userId, bugReportId);
			}
		}

		public async Task DeleteSubscription(int userId, int bugReportId)
		{
			if(await IsSubscribed (userId, bugReportId))
			{
				await userSubscriptionsRepository .DeleteSubscription(userId, bugReportId);
			}
		}

		public async Task NotifyBugReportStateChanged(BugState bugState, string bugReportUrl)
		{
			var subscribedUserIds = await userSubscriptionsRepository.GetAllSubscribedUserIds(bugState.BugReportId);

			string emailSubject = await ComposeBugStateEmailSubject(bugState);
			string emailMessage = await ComposeBugStateEmailMessage (bugState, bugReportUrl);

			foreach (var userId in subscribedUserIds)
			{
				ApplicationUser user = await userManager.FindByIdAsync(userId.ToString());

				if (bugState.Author != user.UserName)
				{
					emailHelper.Send(user.UserName, user.Email, emailSubject, emailMessage);
				}
			}
		}

		public async Task NotifyBugReportNewComment(Comment comment, string bugReportUrl)
		{
			var subscribedUserIds = await userSubscriptionsRepository.GetAllSubscribedUserIds(comment.BugReportId);

			var bugReport = await bugReportRepository.GetById(comment.BugReportId);
			var project = await projectRepository.GetById(bugReport.ProjectId);
			string projectName = project.Name;
			string emailSubject = $"Bug report updated: {bugReport.Title}";
			string emailMessage = $"Project: {projectName}\nNew comment posted in bug report {bugReport.Title} by {comment.AuthorId}.\n" +
				$"Please <a href=\"{ bugReportUrl}\">click here</a> to review new content.";

			foreach (var userId in subscribedUserIds)
			{
				ApplicationUser user = await userManager.FindByIdAsync(userId.ToString());

				if(comment.AuthorId != int.Parse(user.Id))
				{
					emailHelper.Send(user.UserName, user.Email, emailSubject, emailMessage);
				}
			}
		}

		private async Task<string> ComposeBugStateEmailSubject(BugState bugState)
		{
			var bugReport = await bugReportRepository.GetById(bugState.BugReportId);
			string message = $"Bug report updated: {bugReport.Title}";

			return message;
		}

		private async Task<string> ComposeBugStateEmailMessage(BugState bugState, string bugReportUrl)
		{
			var bugReport = await bugReportRepository.GetById(bugState.BugReportId);
			var project = await projectRepository.GetById(bugReport.ProjectId);
			string projectName = project.Name;
			string stateName = bugState.StateType.ToString().First().ToString().ToUpper() + bugState.StateType.ToString().Substring(1);

			string message = $"Project: {projectName}\n{bugReport.Title} state updated to {stateName}\n" +
				$"Please <a href=\"{bugReportUrl}\">click here</a> to review new content.";

			return message;
		}
	}
}
