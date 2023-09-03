using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BugTracker.Controllers;

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
		ViewBag.ErrorMessage = statusCode switch
		{
			404 => "404: Requested resource could not be found",
			_ => ViewBag.ErrorMessage
		};

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
