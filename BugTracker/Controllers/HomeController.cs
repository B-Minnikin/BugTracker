using Microsoft.AspNetCore.Mvc;
using SmartBreadcrumbs.Attributes;

namespace BugTracker.Controllers;

public class HomeController : Controller
{
	[DefaultBreadcrumb("Home")]
	public IActionResult Index()
	{
		return View();
	}

	public IActionResult Privacy()
	{
		return View();
	}
}
