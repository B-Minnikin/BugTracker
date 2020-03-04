using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models.Authorization
{
	public class ApplicationUserManager : UserManager<IdentityUser>
	{
		public ApplicationUserManager() : this(new UserStore(), null, null, null, null, null, null, null, null)
		{
		}

		public ApplicationUserManager(IUserStore<IdentityUser> store, IOptions<IdentityOptions> optionsAccessor,
			IPasswordHasher<IdentityUser> passwordHasher, IEnumerable<IUserValidator<IdentityUser>> userValidators,
			IEnumerable<IPasswordValidator<IdentityUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors,
			IServiceProvider services, ILogger<UserManager<IdentityUser>> logger) : base(store, optionsAccessor,
				passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
		{
		}
	}
}
