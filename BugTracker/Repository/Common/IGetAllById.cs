using System.Collections.Generic;
using System.Threading.Tasks;

namespace BugTracker.Repository.Common
{
	public interface IGetAllById<T>
	{
		Task<IEnumerable<T>> GetAllById(int id);
	}
}
