
namespace BugTracker.Repository.Common
{
	public interface IDelete<T>
	{
		T Delete(int id);
	}
}
