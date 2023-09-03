using BugTracker.ViewModels;
using Microsoft.AspNetCore.Mvc;

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
