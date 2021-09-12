using BugTracker.Models;
using BugTracker.Repository.Common;

namespace BugTracker.Repository.Interfaces
{
	public interface ICommentRepository : IAdd<Comment>,
		IUpdate<Comment>, IDelete<Comment>,
		IGetById<Comment>, IGetAllById<Comment>
	{
		int GetCommentParentId(int commentId);
	}
}
