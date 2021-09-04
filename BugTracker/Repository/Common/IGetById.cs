using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Repository
{
	public interface IGetById<T>
	{
		T GetById(int id);
	}
}
