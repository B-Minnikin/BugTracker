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
		public IActionResult Edit(int id)
		{
			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, id, "CanModifyProfilePolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{

				return View();
			}

			return RedirectToAction("Index", "Home");
		}

		[HttpPost]
		[Authorize]
		public IActionResult Edit(EditProfileViewModel model)
		{
			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, model.User.Id, "CanModifyProfilePolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				if (ModelState.IsValid)
				{

				}
			}

			return View();
		}
	}
}
