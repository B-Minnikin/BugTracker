using BugTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
		private readonly IAuthorizationService authorizationService;
		private readonly IHttpContextAccessor httpContextAccessor;

		public MilestoneController(ILogger<MilestoneController> logger, 
			IProjectRepository projectRepository,
			IAuthorizationService authorizationService,
			IHttpContextAccessor httpContextAccessor)
		{
			this.logger = logger;
			this.projectRepository = projectRepository;
			this.authorizationService = authorizationService;
			this.httpContextAccessor = httpContextAccessor;
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

			logger.LogWarning($"Invalid Milestone model state");
			return View(model);
		}

		public IActionResult Delete(int milestoneId)
		{
			var currentProjectId = HttpContext.Session.GetInt32("currentProject");
			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, currentProjectId, "ProjectAdministratorPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				projectRepository.DeleteMilestone(milestoneId);

				return RedirectToAction("Overview", "Projects", new { id = currentProjectId});
			}

			return RedirectToAction("Index", "Home");
		}
	}
}
