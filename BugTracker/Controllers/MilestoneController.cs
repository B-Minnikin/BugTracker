using BugTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Controllers
{
	public class MilestoneController : Controller
	{
		private readonly ILogger<MilestoneController> logger;
		private readonly IProjectRepository projectRepository;

		public MilestoneController(ILogger<MilestoneController> logger, IProjectRepository projectRepository)
		{
			this.logger = logger;
			this.projectRepository = projectRepository;
		}

		[HttpGet]
		public IActionResult New(int projectId)
		{
			Milestone model = new Milestone()
			{
				ProjectId = projectId
			};

			return View(model);
		}

		[HttpPost]
		public IActionResult New(Milestone model)
		{
			if (ModelState.IsValid)
			{
				projectRepository.AddMilestone(model);

				return RedirectToAction("Overview", "Projects", new { id = model.ProjectId });
			}

			return View(model);
		}
	}
}
