using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Repository.Common
{
	public interface IGetAllById<T>
	{
		IEnumerable<T> GetAllById(int id);
	}
}
