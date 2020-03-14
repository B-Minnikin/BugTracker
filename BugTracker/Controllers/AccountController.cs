﻿using BugTracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
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
		[AllowAnonymous]
		public ViewResult Login()
		{
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		public async Task<IActionResult> Login(LoginViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await userManager.FindByEmailAsync(model.Email);
				if (user != null && !user.EmailConfirmed && (await userManager.CheckPasswordAsync(user, model.Password)))
				{
					ModelState.AddModelError(string.Empty, "Email not confirmed");
					return View(model);
				}

				var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: true, false);

				if (result.Succeeded)
				{
					return RedirectToAction("Index", "Home");
					//return RedirectToAction("Projects", "Projects");
				}

				ModelState.AddModelError(string.Empty, "Invalid login attempt");
			}

			return View(model);
		}

		[HttpGet]
		[AllowAnonymous]
		public ViewResult Register()
		{
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		public async Task<IActionResult> Register(RegisterViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = new IdentityUser
				{
					UserName = model.Email,
					Email = model.Email,
					NormalizedEmail = model.Email.ToUpper()
				};
				var result = await userManager.CreateAsync(user, model.Password);

				if (result.Succeeded)
				{
					var createdUser = await userManager.FindByEmailAsync(user.Email);

					var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
					var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = createdUser.Id, token = token }, Request.Scheme );
					logger.Log(LogLevel.Warning, confirmationLink);

					return View("RegistrationComplete");
				}

				AddErrorToModelState(result);
			}

			return View(model);
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> ConfirmEmail(string userId, string token)
		{
			if(userId == null || token == null)
			{
				return RedirectToAction("Index", "Home");
			}

			var user = await userManager.FindByIdAsync(userId);
			if (user == null)
			{
				// generate error message
				return View("Error");
			}

			var result = await userManager.ConfirmEmailAsync(user, token);
			return View(result.Succeeded ? "ConfirmEmail" : "Error");
		}

		[HttpPost]
		public async Task<IActionResult> LogOut()
		{
			await signInManager.SignOutAsync();

			return RedirectToAction("Index", "Home");
		}

		[HttpGet]
		[Authorize]
		public ViewResult EditProfile()
		{
			return View();
		}

		[HttpPost]
		[Authorize]
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
