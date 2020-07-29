using BugTracker.Models;
using BugTracker.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Controllers
{
	public class SearchController : Controller
	{
		private readonly ILogger<SearchController> logger;

		public SearchController(ILogger<SearchController> logger)
		{
			this.logger = logger;
		}

		[HttpGet]
		public ViewResult Search()
		{
			SearchResultsViewModel searchResultsViewModel = new SearchResultsViewModel
			{
				SearchResults = new List<BugReport>()
			};

			return View(searchResultsViewModel);
		}

		[HttpPost]
		public ViewResult Result(SearchResultsViewModel searchString)
		{
			return View(searchString);
		}
	}
}
