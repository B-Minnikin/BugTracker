
using System.Threading.Tasks;

namespace BugTracker.Database.Repository.Common
{
	public interface IDelete<T>
	{
		Task<T> Delete(int id);
	}
}
