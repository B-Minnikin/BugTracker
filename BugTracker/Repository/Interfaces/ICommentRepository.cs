using BugTracker.Models;
using BugTracker.Repository.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Repository.Interfaces
{
	public interface ICommentRepository : IAdd<Comment>,
		IUpdate<Comment>, IDelete<Comment>,
		IGetById<Comment>, IGetAllById<Comment>
	{
		int GetCommentParentId(int commentId);
	}
}
