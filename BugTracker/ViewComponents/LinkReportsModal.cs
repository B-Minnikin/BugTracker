using BugTracker.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.ViewComponents
{
	public class LinkReportsModal : ViewComponent
	{
		public IViewComponentResult Invoke(int projectId, int bugReportId)
		{
			LinkReportsViewModel model = new LinkReportsViewModel
			{
				ProjectId = projectId,
				BugReportId = bugReportId
			};

			return View(model);
		}
	}
}
