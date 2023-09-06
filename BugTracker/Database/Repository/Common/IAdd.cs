
using System.Threading.Tasks;

namespace BugTracker.Database.Repository.Common
{
	public interface IAdd<T>
	{
		Task<T> Add(T model);
	}
}
