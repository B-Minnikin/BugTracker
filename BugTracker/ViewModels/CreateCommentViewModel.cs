﻿using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.ViewModels
{
	public class CreateCommentViewModel
	{
		public bool Subscribe { get; set; } = false;
		public BugReportComment Comment { get; set; } = new BugReportComment();
	}
}