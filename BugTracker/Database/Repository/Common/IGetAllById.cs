using System.Collections.Generic;
using System.Threading.Tasks;

namespace BugTracker.Database.Repository.Common
{
	public interface IGetAllById<T>
	{
		Task<IEnumerable<T>> GetAllById(int id);
	}
}
