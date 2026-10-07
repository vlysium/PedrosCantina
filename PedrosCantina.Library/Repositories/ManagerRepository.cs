using Microsoft.Data.SqlClient;
using PedrosCantina.Library.Models;

namespace PedrosCantina.Library.Repositories;

public class ManagerRepository : IReadOperations<Manager, int>, IWriteOperations<Manager, int>
{
	/// <summary>
	/// The database worker used to interact with the database.
	/// </summary>
	private readonly DBWorker _dbWorker;

	public ManagerRepository(DBWorker dbWorker)
	{
		_dbWorker = dbWorker;
	}

	/// <summary>
	/// Reads a manager from the database by their unique identifier.
	/// </summary>
	/// <param name="id">The unique identifier of the manager to read.</param>
	/// <returns>The manager if found, otherwise null.</returns>
	public Manager? ReadById(int id)
	{
		const string query = """
			SELECT manager_id, name, email, phone_number
			FROM vw_manager_details
			WHERE manager_id = @id;
		""";

		// Release the connection after use with `using`
		using SqlConnection connection = _dbWorker.Connect();

		using SqlCommand command = new SqlCommand(query, connection);
		command.Parameters.AddWithValue("@id", id);

		using SqlDataReader reader = command.ExecuteReader();

		if (!reader.Read())
		{
			return null;
		}

		return PopulateManager(reader);
	}
	
	/// <summary>
	/// Reads all managers from the database.
	/// </summary>
	/// <returns>A list of all managers in the database.</returns>
	public List<Manager> ReadAll()
	{
		const string query = """
			SELECT manager_id, name, email, phone_number
			FROM vw_manager_details
		""";

		List<Manager> managers = new List<Manager>();

		// Release the connection after use with `using`
		using SqlConnection connection = _dbWorker.Connect();

		using SqlCommand command = new SqlCommand(query, connection);
		using SqlDataReader reader = command.ExecuteReader();

		while (reader.Read())
		{
			managers.Add(PopulateManager(reader));
		}

		return managers;
	}

	/// <summary>
	/// Creates a new manager in the database. This method first creates a new employee and then assigns that employee as a manager.
	/// </summary>
	/// <param name="manager">The manager to insert into the database.</param>
	/// <returns>The created manager.</returns>
	/// <exception cref="InvalidOperationException">Thrown when the manager cannot be created.</exception>
	public Manager Create(Manager manager)
	{
		const string query1 = """
			INSERT INTO employees (name, email, phone_number)
			OUTPUT INSERTED.employee_id
			VALUES (@name, @email, @phone_number);
		""";

		const string query2 = """
			INSERT INTO managers (manager_id)
			VALUES (@id);
		""";

		// Release the connection after use with `using`
		using SqlConnection connection = _dbWorker.Connect();
		using SqlTransaction transaction = connection.BeginTransaction();

		try
		{
			using SqlCommand command1 = new SqlCommand(query1, connection, transaction);
			command1.Parameters.AddWithValue("@name", manager.Name);
			command1.Parameters.AddWithValue("@email", manager.Email);
			command1.Parameters.AddWithValue("@phone_number", manager.PhoneNumber);

			object newManagerId = command1.ExecuteScalar();

			if (newManagerId == null || newManagerId == DBNull.Value)
			{
				throw new InvalidOperationException("Failed to create manager.");
			}

			using SqlCommand command2 = new SqlCommand(query2, connection, transaction);
			command2.Parameters.AddWithValue("@id", Convert.ToInt32(newManagerId));

			int rowsAffected = command2.ExecuteNonQuery();

			if (rowsAffected != 1)
			{
				throw new InvalidOperationException("Failed to create manager.");
			}

			transaction.Commit();

			return new Manager(
				employeeId: Convert.ToInt32(newManagerId),
				name: manager.Name,
				email: manager.Email,
				phoneNumber: manager.PhoneNumber
			);
		}

		catch (Exception)
		{
			transaction.Rollback();
			throw;
		}
	}

	/// <summary>
	/// Promotes an existing employee to a manager by inserting their ID into the managers table.
	/// This method does not create a new employee; it assumes the employee already exists in the employees table.
	/// </summary>
	/// <param name="manager">The manager to promote.</param>
	/// <exception cref="InvalidOperationException">Thrown when the employee is already a manager.</exception>
	public void Update(Manager manager)
	{
		const string query = """
			INSERT INTO managers (manager_id)
			VALUES (@id);
		""";

		// Release the connection after use with `using`
		using SqlConnection connection = _dbWorker.Connect();

		using SqlCommand command = new SqlCommand(query, connection);
		command.Parameters.AddWithValue("@id", manager.EmployeeId);
		int rowsAffected = command.ExecuteNonQuery();

		if (rowsAffected != 1)
		{
			throw new KeyNotFoundException($"Employee with ID {manager.EmployeeId} does not exist.");
		}
	}

	/// <summary>
	/// Deletes a manager from the database by their unique identifier.
	/// This method only deletes the manager from the managers table, but does not delete the corresponding employee from the employees table, effectively demoting the manager to a regular employee.
	/// </summary>
	/// <param name="id">The unique identifier of the manager to delete from the manager table.</param>
	/// <exception cref="KeyNotFoundException">Thrown when the manager with the specified ID is not found.</exception>
	public void Delete(int id)
	{
		const string query1 = """
			DELETE FROM managers
			WHERE manager_id = @id;
		""";

		// Release the connection after use with `using`
		using SqlConnection connection = _dbWorker.Connect();

		using SqlCommand command1 = new SqlCommand(query1, connection);
		command1.Parameters.AddWithValue("@id", id);

		int rowsAffected = command1.ExecuteNonQuery();

		if (rowsAffected != 1)
		{
			throw new KeyNotFoundException($"Manager with ID {id} not found.");
		}
	}

	/// <summary>
	/// Helper method to populate a Manager instance from a SqlDataReader.
	/// </summary>
	/// <param name="reader">The SqlDataReader containing the data to populate the Manager instance.</param>
	/// <returns>A new Manager instance populated with the data from the reader.</returns>
	private Manager PopulateManager(SqlDataReader reader)
	{
		return new Manager(
			employeeId: reader.GetInt32(reader.GetOrdinal("manager_id")),
			name: reader.GetString(reader.GetOrdinal("name")),
			email: reader.GetString(reader.GetOrdinal("email")),
			phoneNumber: reader.GetString(reader.GetOrdinal("phone_number"))
		);
	}
}
