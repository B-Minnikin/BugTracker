using BugTracker.Models;
using BugTracker.Models.Authorization;
using BugTracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Controllers
{
	public class ProfileController : Controller
	{
		private readonly ILogger<ProfileController> logger;
		private readonly IAuthorizationService authorizationService;
		private readonly ApplicationUserManager userManager;

		public ProfileController(ILogger<ProfileController> logger,
									IProjectRepository projectRepository,
									IAuthorizationService authorizationService)
		{
			this.logger = logger;
			this.authorizationService = authorizationService;
			this.userManager = new ApplicationUserManager();
		}

		[HttpGet]
		[Authorize]
		public async Task<IActionResult> Edit(int id)
		{
			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, id, "CanModifyProfilePolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				var user = await userManager.FindByIdAsync(id.ToString());
				EditProfileViewModel profileViewModel = new EditProfileViewModel
				{
					User = user
				};

				return View(profileViewModel);
			}

			return RedirectToAction("Index", "Home");
		}

		[HttpPost]
		[Authorize]
		public async Task<IActionResult> Edit(EditProfileViewModel model)
		{
			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, Int32.Parse(model.User.Id), "CanModifyProfilePolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				if (ModelState.IsValid)
				{
					var user = await userManager.FindByIdAsync(model.User.Id);
					user.PhoneNumber = model.User.PhoneNumber;

					var result = await userManager.UpdateAsync(user);
					if (!result.Succeeded)
					{
						logger.LogError($"Failed to update user profile [{model.User.Id}]");
						return View("Error");
					}
				}
			}

			return RedirectToAction("Index", "Home");
		}
	}
}
