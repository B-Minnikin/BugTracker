
using System.Threading.Tasks;

namespace BugTracker.Repository.Common
{
	public interface IGetById<T>
	{
		Task<T> GetById(int id);
	}
}
