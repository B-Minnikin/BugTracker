
using System.Threading.Tasks;

namespace BugTracker.Repository.Common
{
	public interface IAdd<T>
	{
		Task<T> Add(T model);
	}
}
