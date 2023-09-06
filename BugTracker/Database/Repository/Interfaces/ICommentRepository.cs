using System.Threading.Tasks;
using BugTracker.Database.Repository.Common;
using BugTracker.Models;

namespace BugTracker.Database.Repository.Interfaces
{
	public interface ICommentRepository : IAdd<Comment>,
		IUpdate<Comment>, IDelete<Comment>,
		IGetById<Comment>, IGetAllById<Comment>
	{
		Task<int> GetCommentParentId(int commentId);
	}
}
