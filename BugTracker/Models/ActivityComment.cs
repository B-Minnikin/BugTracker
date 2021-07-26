using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
	public class ActivityComment : Activity
	{
		public int BugReportId { get; set; }
		public int BugReportCommentId { get; set; }

		public ActivityComment() { }

		public ActivityComment(DateTime timestamp, int projectId, ActivityMessage messageId, int userId, int bugReportId, int bugReportCommentId) 
			: base(timestamp, projectId, messageId, userId)
		{
			BugReportId = bugReportId;
			BugReportCommentId = bugReportCommentId;
		}

		public override string ActivityMessage
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}
	}
}
