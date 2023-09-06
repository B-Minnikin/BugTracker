
using System.Threading.Tasks;

namespace BugTracker.Database.Repository.Common
{
	public interface IUpdate<T>
	{
		Task<T> Update(T model);
	}
}
