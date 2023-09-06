
namespace BugTracker.Database.Repository.DapperRepositories
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
