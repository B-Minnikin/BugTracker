using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
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

		[Route("Error/{statusCode}")]
		public IActionResult StatusCodeHandler(int statusCode)
		{
			switch (statusCode)
			{
				case 404:
					ViewBag.ErrorMessage = "404: Requested resource could not be found";
					break;
			}

			return View("NotFound");
		}

		[Route("Error")]
		[AllowAnonymous]
		public IActionResult Error()
		{
			var exceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

			logger.LogError($"Exception in path: {exceptionHandlerFeature.Path} - " +
				$"{exceptionHandlerFeature.Error}");

			return View("Error");
		}
	}
}
