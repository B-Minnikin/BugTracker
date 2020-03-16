using BugTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Controllers
{
	public class ProfileController : Controller
	{
		private readonly ILogger<ProfileController> logger;

		public ProfileController(ILogger<ProfileController> logger,
						IProjectRepository projectRepository)
		{
			this.logger = logger;
		}
	}
}
