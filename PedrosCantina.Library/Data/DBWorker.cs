using DotNetEnv;
using Microsoft.Data.SqlClient;

namespace PedrosCantina.Library;

public class DBWorker
{
	/// <summary>
	/// The SQL connection used to interact with the database.
	/// </summary>
	private SqlConnection _connection;

	/// <summary>
	/// The connection string used to connect to the database.
	/// </summary>
	private string _connectionString;

	/// <summary>
	/// Initializes a new instance of the <see cref="DBWorker"/> class with the specified connection string.
	/// </summary>
	public SqlConnection Connection => _connection;

	/// <summary>
	/// Initializes a new instance of the <see cref="DBWorker"/> class and loads the connection string from the `DB_CONNECTION_STRING` environment variable.
	/// </summary>
	/// <exception cref="InvalidOperationException">Thrown when the `DB_CONNECTION_STRING` environment variable is not set.</exception>
	public DBWorker()
	{
		Env.Load();
		_connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
			?? throw new InvalidOperationException("DB_CONNECTION_STRING environment variable is not set.");
	}

	/// <summary>
	/// Connects to the database using the connection string and returns the established SQL connection.
	/// </summary>
	/// <returns>The established SQL connection.</returns>
	public SqlConnection Connect()
	{
		_connection = new SqlConnection(_connectionString);
		_connection.Open();
		return _connection;
	}
}
