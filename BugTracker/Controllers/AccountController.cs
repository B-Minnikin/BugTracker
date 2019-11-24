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

		public AccountController(ILogger<AccountController> logger)
		{
			this.logger = logger;
		}

		public ViewResult LogIn()
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
	}
}
