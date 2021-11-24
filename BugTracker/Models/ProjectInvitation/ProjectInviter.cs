using BugTracker.Models.Authorization;
using BugTracker.Repository.Interfaces;
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
		private readonly IProjectInvitationsRepository projectInvitationsRepository;
		private readonly IAuthorizationService authorizationService;
		private readonly IEmailHelper emailHelper;
		private readonly IUserClaimsPrincipalFactory<IdentityUser> userClaimsPrincipalFactory;
		private readonly ILogger<ProjectInviter> logger;
		private readonly ApplicationUserManager userManager;

		public ProjectInviter(IProjectRepository projectRepository,
			IProjectInvitationsRepository projectInvitationsRepository,
			IAuthorizationService authorizationService,
			IEmailHelper emailHelper,
			IUserClaimsPrincipalFactory<IdentityUser> userClaimsPrincipalFactory,
			ILogger<ProjectInviter> logger,
			ApplicationUserManager userManager)
		{
			this.projectRepository = projectRepository;
			this.projectInvitationsRepository = projectInvitationsRepository;
			this.authorizationService = authorizationService;
			this.emailHelper = emailHelper;
			this.userClaimsPrincipalFactory = userClaimsPrincipalFactory;
			this.logger = logger;
			this.userManager = userManager;
		}

		public async Task AddProjectInvitation(ProjectInvitation invitation)
		{
			invitation.ToUser = await userManager.FindByEmailAsync(invitation.EmailAddress);

			bool userAlreadyExists = invitation.ToUser != null;
			bool emailAddressIsPendingRegistration = await EmailAddressPendingRegistration(invitation.EmailAddress, invitation.Project.ProjectId);

			if (!userAlreadyExists)
			{
				if(!emailAddressIsPendingRegistration)
				{
					await projectInvitationsRepository.AddPendingProjectInvitation(invitation.EmailAddress, invitation.Project.ProjectId);
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

				await RemovePendingProjectInvitation(invitation.EmailAddress, invitation.Project.ProjectId);
			}
		}

		public async Task RemovePendingProjectInvitation(string emailAddress, int projectId)
		{
			if(await EmailAddressPendingRegistration (emailAddress, projectId))
			{
				await projectInvitationsRepository .DeletePendingProjectInvitation(emailAddress, projectId);
			}
			else
			{
				// email address didn't exist in pending registration table - warn
				logger.LogWarning($"Email address ({emailAddress}) couldn't be removed from pending invitations for project ID ({projectId}) because it doesn't exist");
			}
		}

		public async Task AddUserToProjectMemberRoleForAllPendingInvitations(string emailAddress)
		{
			var user = await userManager.FindByEmailAsync(emailAddress);

			if(user != null)
			{
				var projectIds = await projectInvitationsRepository.GetProjectInvitationsForEmailAddress(emailAddress);
				foreach (int projectId in projectIds.ToList())
				{
					await userManager.AddToRoleAsync(user, "Member", projectId);
					await RemovePendingProjectInvitation (emailAddress, projectId);
				}
			}
		}

		private async Task<bool> EmailAddressPendingRegistration(string emailAddress, int projectId)
		{
			return await projectInvitationsRepository.IsEmailAddressPendingRegistration(emailAddress, projectId);
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

		private async Task AddUserToProjectMemberRole(string emailAddress, int projectId)
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
