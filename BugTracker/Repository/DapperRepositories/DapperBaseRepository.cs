namespace BugTracker.Repository.DapperRepositories;

public class DapperBaseRepository
{
	private readonly string connectionString;

	protected DapperBaseRepository(string connectionString)
	{
		this.connectionString = connectionString;
	}

	protected System.Data.SqlClient.SqlConnection GetConnectionString()
	{
		return new System.Data.SqlClient.SqlConnection(connectionString);
	}
}
