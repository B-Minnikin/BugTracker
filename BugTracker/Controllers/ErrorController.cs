using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
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
		private readonly IHttpContextAccessor httpContextAccessor;

		public ErrorController(ILogger<ErrorController> logger,
							     IHttpContextAccessor httpContextAccessor)
		{
			this.logger = logger;
			this.httpContextAccessor = httpContextAccessor;
		}

		[Route("Error/{statusCode}")]
		public IActionResult StatusCodeHandler(int statusCode)
		{
			switch (statusCode)
			{
				case 404:
					ViewBag.ErrorMessage = "404: Requested resource could not be found";
					break;
				case 401:
					ViewBag.ErrorMessage = "401: Bad request";
					break;
			}

			return View("NotFound");
		}

		[Route("Error")]
		[AllowAnonymous]
		public IActionResult Error()
		{
			var exceptionHandlerFeature = httpContextAccessor.HttpContext.Features.Get<IExceptionHandlerPathFeature>();

			logger.LogError($"Exception in path: {exceptionHandlerFeature.Path} - " +
				$"{exceptionHandlerFeature.Error}");

			return View("Error");
		}
	}
}
