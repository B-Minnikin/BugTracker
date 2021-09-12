using System.Collections.Generic;

namespace BugTracker.Repository.Common
{
	public interface IGetAllById<T>
	{
		IEnumerable<T> GetAllById(int id);
	}
}
