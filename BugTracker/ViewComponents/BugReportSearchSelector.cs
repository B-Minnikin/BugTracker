﻿using BugTracker.Models;
using BugTracker.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace BugTracker.ViewComponents
{
	public class BugReportSearchSelector : ViewComponent
	{
		public IViewComponentResult Invoke(int projectId, List<MilestoneBugReportEntry> milestoneBugReportEntries, bool showSearchBar = false, bool canRemoveEntry = false)
		{
			BugReportSearchSelectorViewModel viewModel = new BugReportSearchSelectorViewModel
			{
				ProjectId = projectId,
				MilestoneBugReportEntries = milestoneBugReportEntries,
				CanRemoveEntry = canRemoveEntry,
				ShowSearchBar = showSearchBar
			};

			return View(viewModel);
		}
	}
}
