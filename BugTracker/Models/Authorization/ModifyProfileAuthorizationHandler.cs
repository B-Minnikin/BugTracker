using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models.Authorization
{
	public class ModifyProfileAuthorizationHandler : AuthorizationHandler<ModifyProfileRequirement, int>
	{
		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ModifyProfileRequirement requirement, int userId)
		{
			string userName = context.User.Identity.Name;
			if(userName == null)
			{
				return Task.CompletedTask;
			}
			
			bool userIsSuperadministrator = AuthorizationHelper.UserIsSuperadministrator(userName);
			bool userIsProfileOwner = AuthorizationHelper.UserIsProfileOwner(context.User.Identity.Name, userId).Result;

			if(userIsSuperadministrator || userIsProfileOwner)
			{
				context.Succeed(requirement);
			}

			return Task.CompletedTask;
		}
	}

	public class ModifyProfileRequirement : IAuthorizationRequirement { }
}
