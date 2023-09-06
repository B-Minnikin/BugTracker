
using System.Threading.Tasks;

namespace BugTracker.Database.Repository.Common
{
	public interface IGetById<T>
	{
		Task<T> GetById(int id);
	}
}
