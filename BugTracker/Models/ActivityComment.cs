using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
	public class ActivityComment : Activity
	{
		public int BugReportCommentId { get; set; }

		public ActivityComment(int activityId, DateTime timestamp, int projectId, ActivityMessage messageId, int userId, int bugReportCommentId) 
			: base(activityId, timestamp, projectId, messageId, userId)
		{
			BugReportCommentId = bugReportCommentId;
		}

		public override string ActivityMessage
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}
	}
}
