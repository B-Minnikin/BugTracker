using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
	public class ActivityProject : Activity
	{
		public ActivityProject() { }

		public ActivityProject(DateTime timestamp, int projectId, ActivityMessage messageId, int userId)
			: base(timestamp, projectId, messageId, userId)
		{ }

		public override string ActivityMessage { 
			get => throw new NotImplementedException(); 
			set => throw new NotImplementedException(); 
		}
	}
}
