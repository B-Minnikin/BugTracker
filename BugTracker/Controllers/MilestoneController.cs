using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Controllers
{
	public class MilestoneController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
