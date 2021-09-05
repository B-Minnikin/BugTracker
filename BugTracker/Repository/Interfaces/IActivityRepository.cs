﻿using BugTracker.Models;
using BugTracker.Repository.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Repository.Interfaces
{
	public interface IActivityRepository : IAdd<Activity>,
		IDelete<Activity>
	{
		IEnumerable<Activity> GetUserActivities(int userId);
		IEnumerable<Activity> GetBugReportActivities(int bugReportId);
	}
}
