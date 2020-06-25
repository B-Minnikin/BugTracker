using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models.Database
{
	public class SubscriptionHelper : ISubscriptionHelper
	{
		private readonly IProjectRepository projectRepository;
		private readonly IEmailHelper emailHelper;

		public SubscriptionHelper(IProjectRepository projectRepository,
			IEmailHelper emailHelper)
		{
			this.projectRepository = projectRepository;
			this.emailHelper = emailHelper;
		}

		public bool IsSubscribed(int userId, int bugReportId)
		{
			return projectRepository.IsSubscribed(userId, bugReportId);
		}

		public void ComposeMessage()
		{

		}

		// check database for everyone subscribing
		// ignore authors of the changes
			// author of comment
			// person changing the state of a report
		// get recipients
	}
}
