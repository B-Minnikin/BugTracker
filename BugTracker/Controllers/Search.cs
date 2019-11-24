using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Controllers
{
	public class Search : Controller
	{
		private readonly ILogger<Search> logger;

		public Search(ILogger<Search> logger)
		{
			this.logger = logger;
		}

		public ViewResult Result()
		{
			return View();
		}
	}
}
