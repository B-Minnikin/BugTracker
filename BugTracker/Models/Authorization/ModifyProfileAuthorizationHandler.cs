using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models.Authorization
{
	public class ModifyProfileAuthorizationHandler : AuthorizationHandler<ModifyProfileRequirement, int>
	{
		private readonly string connectionString;

		public ModifyProfileAuthorizationHandler(string connectionString)
		{
			this.connectionString = connectionString;
		}

		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ModifyProfileRequirement requirement, int userId)
		{
			string userName = context.User.Identity.Name;
			if(userName == null)
			{
				return Task.CompletedTask;
			}
			
			bool userIsSuperadministrator = AuthorizationHelper.UserIsSuperadministrator(userName, connectionString);
			bool userIsProfileOwner = AuthorizationHelper.UserIsProfileOwner(context.User.Identity.Name, userId, connectionString).Result;

			if(userIsSuperadministrator || userIsProfileOwner)
			{
				context.Succeed(requirement);
			}

			return Task.CompletedTask;
		}
	}

	public class ModifyProfileRequirement : IAuthorizationRequirement { }
}
