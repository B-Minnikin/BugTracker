using BugTracker.Models.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BugTracker.Models.ProjectInvitation
{
	public class ProjectInvitation : IProjectInvitation
	{
		private readonly IProjectRepository projectRepository;
		private readonly IAuthorizationService authorizationService;
		private readonly IEmailHelper emailHelper;
		private readonly IUserClaimsPrincipalFactory<IdentityUser> userClaimsPrincipalFactory;
		private readonly ILogger logger;
		private readonly ApplicationUserManager userManager;

		public ProjectInvitation(IProjectRepository projectRepository,
			IAuthorizationService authorizationService,
			IEmailHelper emailHelper,
			IUserClaimsPrincipalFactory<IdentityUser> userClaimsPrincipalFactory,
			ILogger logger)
		{
			this.projectRepository = projectRepository;
			this.authorizationService = authorizationService;
			this.emailHelper = emailHelper;
			this.userClaimsPrincipalFactory = userClaimsPrincipalFactory;
			this.logger = logger;
			userManager = new ApplicationUserManager();
		}

		public void AddProjectInvitation(string emailAddress, int projectId)
		{
			bool emailAddressIsPendingRegistration = EmailAddressPendingRegistration(emailAddress, projectId);
			bool userAlreadyExists = UserAlreadyExists(emailAddress).Result;

			if (!userAlreadyExists)
			{
				if(!emailAddressIsPendingRegistration)
				{
					// when user registers, check the list for matches + then add to project

					// add to project invitation repository
					projectRepository.CreatePendingProjectInvitation(emailAddress, projectId);
					SendProjectInvitationEmail(emailAddress);
				}
			}
			else // email already registered
			{
				// check that user not already in the specified role
				if(!UserHasProjectAuthorization(emailAddress, projectId).Result)
				{
					// if not then add to it and send notification email
					AddUserToProjectMemberRole(emailAddress, projectId);
					SendProjectRoleNotificationEmail(emailAddress);
				}

				RemovePendingProjectInvitation(emailAddress, projectId);
			}
		}

		public void RemovePendingProjectInvitation(string emailAddress, int projectId)
		{
			if(EmailAddressPendingRegistration(emailAddress, projectId))
			{
				projectRepository.RemovePendingProjectInvitation(emailAddress, projectId);
			}
			else
			{
				// email address didn't exist in pending registration table - warn
				logger.LogWarning($"Email address ({emailAddress}) couldn't be removed from pending invitations for project ID ({projectId}) because it doesn't exist");
			}
		}

		public async void AddUserToProjectMemberRoleForAllPendingInvitations(string emailAddress)
		{
			var user = await userManager.FindByEmailAsync(emailAddress);

			if(user != null)
			{
				List<int> projectIds = projectRepository.GetProjectInvitationsForEmailAddress(emailAddress).ToList();
				foreach (int projectId in projectIds)
				{
					await userManager.AddToRoleAsync(user, "Member", projectId);
				}
			}
		}

		private async Task<bool> UserAlreadyExists(string emailAddress)
		{
			var user = await userManager.FindByEmailAsync(emailAddress);

			return (user != null);
		}

		private bool EmailAddressPendingRegistration(string emailAddress, int projectId)
		{
			return projectRepository.IsEmailAddressPendingRegistration(emailAddress, projectId);
		}

		private async Task<bool> UserHasProjectAuthorization(string emailAddress, int projectId)
		{
			var user = await userManager.FindByEmailAsync(emailAddress);
			if(user != null)
			{
				var principal = await userClaimsPrincipalFactory.CreateAsync(user);

				var authorizationResult = authorizationService.AuthorizeAsync(principal, projectId, "ProjectAdministratorAuthorizationHandler");
				if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
				{
					return true;
				}
			}

			return false;
		}

		private async void AddUserToProjectMemberRole(string emailAddress, int projectId)
		{
			var user = await userManager.FindByEmailAsync(emailAddress);

			// check user not already in role

			await userManager.AddToRoleAsync(user, "Member", projectId);
		}

		private void SendProjectInvitationEmail(string emailAddress)
		{
			// compose invitation email
			string emailSubject = $"Invitation to project PLACEHOLDER";
			string emailMessage = $"Invitation to project PLACEHOLDER: Message body";

			// send invitation email
			emailHelper.Send(emailAddress, emailAddress, emailSubject, emailMessage);
		}

		private void SendProjectRoleNotificationEmail(string emailAddress)
		{
			// compose notification email
			string emailSubject = $"Added to project notification PLACEHOLDER";
			string emailMessage = $"Added to project notification PLACEHOLDER: Message body";

			// send notification email
			emailHelper.Send(emailAddress, emailAddress, emailSubject, emailMessage);
		}
	}
}
