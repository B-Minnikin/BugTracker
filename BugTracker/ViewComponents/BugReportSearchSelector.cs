using BugTracker.Models;
using BugTracker.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.ViewComponents
{
	public class BugReportSearchSelector : ViewComponent
	{
		public IViewComponentResult Invoke(int projectId, List<MilestoneBugReportEntry> milestoneBugReportEntries)
		{
			BugReportSearchSelectorViewModel viewModel = new BugReportSearchSelectorViewModel
			{
				ProjectId = projectId,
				MilestoneBugReportEntries = milestoneBugReportEntries,
				CanRemoveEntry = true,
				ShowSearchBar = true
			};

			return View(viewModel);
		}
	}
}
