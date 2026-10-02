using Microsoft.Data.SqlClient;
using PedrosCantina.Library.Repositories;

namespace PedrosCantina.Library.Data;

public class EmployeeRepository : ICrudOperations<Employee>
{
	/// <summary>
	/// The database worker used to interact with the database.
	/// </summary>
	private readonly DBWorker _dbWorker;

	public EmployeeRepository(DBWorker dbWorker)
	{
		_dbWorker = dbWorker;
	}

	/// <summary>
	/// Reads an employee from the database by their unique identifier.
	/// </summary>
	/// <param name="id">The unique identifier of the employee to read.</param>
	/// <returns>The employee if found, otherwise null.</returns>
	public Employee? ReadById(int id)
	{
		const string query = """
			SELECT employee_id, name, email, phone_number
			FROM Employees
			WHERE employee_id = @Id;
		""";

		// Release the connection after use with `using`
		using SqlConnection connection = _dbWorker.Connect();

		SqlCommand command = new SqlCommand(query, connection);
		command.Parameters.AddWithValue("@Id", id);

		SqlDataReader reader = command.ExecuteReader();

		if (!reader.Read())
		{
			return null;
		}

		return new Employee
		{
			EmployeeId = reader.GetInt32(reader.GetOrdinal("employee_id")),
			Name = reader.GetString(reader.GetOrdinal("name")),
			Email = reader.GetString(reader.GetOrdinal("email")),
			PhoneNumber = reader.GetString(reader.GetOrdinal("phone_number"))
		};
	}

	public List<Employee> ReadAll()
	{
		throw new NotImplementedException();
	}

	public Employee Create(Employee entity)
	{
		throw new NotImplementedException();
	}

	public Employee Update(Employee entity)
	{
		throw new NotImplementedException();
	}

	public Employee Delete(int id)
	{
		throw new NotImplementedException();
	}
}
