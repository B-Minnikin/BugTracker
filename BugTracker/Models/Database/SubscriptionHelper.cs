using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models.Database
{
	public class SubscriptionHelper : ISubscriptionHelper
	{
		private readonly IProjectRepository projectRepository;

		public SubscriptionHelper(IProjectRepository projectRepository)
		{
			this.projectRepository = projectRepository;
		}

		public bool IsSubscribed(int userId, int bugReportId)
		{
			return projectRepository.IsSubscribed(userId, bugReportId);
		}
	}
}
