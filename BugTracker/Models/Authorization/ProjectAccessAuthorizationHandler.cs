﻿using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models.Authorization
{
	public class ProjectAccessAuthorizationHandler
	{
	}

	public class ProjectAccessRequirement : IAuthorizationRequirement { }
}
