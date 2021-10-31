using BugTracker.Models;
using BugTracker.Repository.Common;
using System.Threading.Tasks;

namespace BugTracker.Repository.Interfaces
{
	public interface ICommentRepository : IAdd<Comment>,
		IUpdate<Comment>, IDelete<Comment>,
		IGetById<Comment>, IGetAllById<Comment>
	{
		Task<int> GetCommentParentId(int commentId);
	}
}
