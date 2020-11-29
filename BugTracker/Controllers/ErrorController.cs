using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Controllers
{
	public class ErrorController : Controller
	{
		private readonly ILogger<ErrorController> logger;

		public ErrorController(ILogger<ErrorController> logger)
		{
			this.logger = logger;
		}
	}
}
