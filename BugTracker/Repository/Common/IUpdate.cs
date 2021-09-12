
namespace BugTracker.Repository.Common
{
	public interface IUpdate<T>
	{
		T Update(T model);
	}
}
