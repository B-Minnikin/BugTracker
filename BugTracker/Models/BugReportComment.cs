﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
	public class BugReportComment
	{
		[Key]
		public int Id { get; set; }
		public string Author { get; set; }
		public DateTime Date { get; set; }
		public string MainText { get; set; }
		public IEnumerable<AttachmentPath> AttachmentPaths { get; set; }
	}
}
