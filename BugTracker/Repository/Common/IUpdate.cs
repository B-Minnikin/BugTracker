﻿
using System.Threading.Tasks;

namespace BugTracker.Repository.Common
{
	public interface IUpdate<T>
	{
		Task<T> Update(T model);
	}
}
