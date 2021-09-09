using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Repository.DapperRepositories
{
	public class DapperBaseRepository
	{
		private readonly string connectionString;

		public DapperBaseRepository(string connectionString)
		{
			this.connectionString = connectionString;
		}

		protected System.Data.SqlClient.SqlConnection GetConnectionString()
		{
			return new System.Data.SqlClient.SqlConnection(connectionString);
		}
	}
}
