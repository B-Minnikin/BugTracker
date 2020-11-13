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
	public class ProjectInviter : IProjectInviter
	{
		private readonly IProjectRepository projectRepository;
		private readonly IAuthorizationService authorizationService;
		private readonly IEmailHelper emailHelper;
		private readonly IUserClaimsPrincipalFactory<IdentityUser> userClaimsPrincipalFactory;
		private readonly ILogger<ProjectInviter> logger;
		private readonly ApplicationUserManager userManager;

		public ProjectInviter(IProjectRepository projectRepository,
			IAuthorizationService authorizationService,
			IEmailHelper emailHelper,
			IUserClaimsPrincipalFactory<IdentityUser> userClaimsPrincipalFactory,
			ILogger<ProjectInviter> logger)
		{
			this.projectRepository = projectRepository;
			this.authorizationService = authorizationService;
			this.emailHelper = emailHelper;
			this.userClaimsPrincipalFactory = userClaimsPrincipalFactory;
			this.logger = logger;
			userManager = new ApplicationUserManager();
		}

		public async Task AddProjectInvitation(ProjectInvitation invitation)
		{
			invitation.ToUser = await userManager.FindByEmailAsync(invitation.EmailAddress);

			bool userAlreadyExists = invitation.ToUser != null;
			bool emailAddressIsPendingRegistration = EmailAddressPendingRegistration(invitation.EmailAddress, invitation.Project.ProjectId);

			if (!userAlreadyExists)
			{
				if(!emailAddressIsPendingRegistration)
				{
					projectRepository.CreatePendingProjectInvitation(invitation.EmailAddress, invitation.Project.ProjectId);
					SendProjectInvitationEmail(invitation);
				}
			}
			else // email already registered
			{
				if(!UserHasProjectAuthorization(invitation.EmailAddress, invitation.Project.ProjectId).Result)
				{
					AddUserToProjectMemberRole(invitation.EmailAddress, invitation.Project.ProjectId);
					SendProjectRoleNotificationEmail(invitation);
				}

				RemovePendingProjectInvitation(invitation.EmailAddress, invitation.Project.ProjectId);
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
					RemovePendingProjectInvitation(emailAddress, projectId);
				}
			}
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

			await userManager.AddToRoleAsync(user, "Member", projectId);
		}

		private void SendProjectInvitationEmail(ProjectInvitation invitation)
		{
			string emailSubject = $"Invitation to join project: { invitation.Project.Name }";
			string emailMessage = $"You have been invited by { invitation.FromUser.UserName } to join the project { invitation.Project.Name } as a member.";

			emailHelper.Send(invitation.EmailAddress, invitation.EmailAddress, emailSubject, emailMessage);
		}

		private void SendProjectRoleNotificationEmail(ProjectInvitation invitation)
		{
			string emailSubject = $"Membership added to project: { invitation.Project.Name }";
			string emailMessage = $"Dear { invitation.ToUser.UserName},\n" +
				$"You have been added as a member to the project { invitation.Project.Name } by { invitation.FromUser.UserName }.";

			emailHelper.Send(invitation.EmailAddress, invitation.EmailAddress, emailSubject, emailMessage);
		}
	}
}
