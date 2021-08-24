using BugTracker.Models;
using BugTracker.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.ViewComponents
{
	public class ActivityFeed : ViewComponent
	{
		public IViewComponentResult Invoke(List<Activity> activities)
		{
			ActivityFeedViewModel viewModel = new ActivityFeedViewModel()
			{
				Activities =  activities
			};

			return View(viewModel);
		}
	}
}
