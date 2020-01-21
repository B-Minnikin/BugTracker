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

		public ViewResult Login()
		{
			return View();
		}

		[HttpGet]
		public ViewResult Register()
		{
			return View();
		}

		[HttpPost]
		public ViewResult Register(RegisterViewModel model)
		{
			return View();
		}

		public ViewResult LogOut()
		{
			return View();
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
	}
}
