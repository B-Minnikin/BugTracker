using BugTracker.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Controllers
{
	public class AccountController : Controller
	{
		private readonly ILogger<AccountController> logger;
		private readonly UserManager<IdentityUser> userManager;
		private readonly SignInManager<IdentityUser> signInManager;

		public AccountController(ILogger<AccountController> logger,
										UserManager<IdentityUser> userManager,
										SignInManager<IdentityUser> signInManager)
		{
			this.logger = logger;
			this.userManager = userManager;
			this.signInManager = signInManager;
		}

		[HttpGet]
		public ViewResult Login()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel model)
		{
			if (ModelState.IsValid)
			{
				var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: true, false);

				if (result.Succeeded)
				{
					return RedirectToAction("Index", "Home");
				}

				ModelState.AddModelError(string.Empty, "Invalid login attempt");
			}

			return View(model);
		}

		[HttpGet]
		public ViewResult Register()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Register(RegisterViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = new IdentityUser
				{
					UserName = model.Email,
					Email = model.Email
				};
				var result = await userManager.CreateAsync(user, model.Password);

				if (result.Succeeded)
				{
					await signInManager.SignInAsync(user, isPersistent: false);
					return RedirectToAction("Projects", "Projects");
				}

				AddErrorToModelState(result);
			}

			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> LogOut()
		{
			await signInManager.SignOutAsync();

			return RedirectToAction("Index", "Home");
		}

		[HttpGet]
		public ViewResult EditProfile()
		{
			return View();
		}

		[HttpPost]
		public ViewResult EditProfile(EditProfileViewModel profile)
		{
			return View();
		}

		private void AddErrorToModelState(IdentityResult result)
		{
			foreach(var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}
		}
	}
}
