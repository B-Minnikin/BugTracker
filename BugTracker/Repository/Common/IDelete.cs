
using System.Threading.Tasks;

namespace BugTracker.Repository.Common
{
	public interface IDelete<T>
	{
		Task<T> Delete(int id);
	}
}
