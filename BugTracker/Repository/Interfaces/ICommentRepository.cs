using BugTracker.Models;
using BugTracker.Repository.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Repository.Interfaces
{
	public interface ICommentRepository : IAdd<BugReportComment>,
		IUpdate<BugReportComment>, IDelete<BugReportComment>,
		IGetById<BugReportComment>, IGetAllById<BugReportComment>
	{
		int GetCommentParentId(int bugReportCommentId);
	}
}
