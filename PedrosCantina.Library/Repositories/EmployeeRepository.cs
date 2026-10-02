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

	/// <summary>
	/// Reads all employees from the database.
	/// </summary>
	/// <returns>A list of all employees in the database.</returns>
	public List<Employee> ReadAll()
	{
		const string query = """
			SELECT employee_id, name, email, phone_number
			FROM Employees;
		""";

		List<Employee> employees = new List<Employee>();

		// Release the connection after use with `using`
		using SqlConnection connection = _dbWorker.Connect();

		SqlCommand command = new SqlCommand(query, connection);
		SqlDataReader reader = command.ExecuteReader();

		while (reader.Read())
		{
			employees.Add(new Employee
			{
				EmployeeId = reader.GetInt32(reader.GetOrdinal("employee_id")),
				Name = reader.GetString(reader.GetOrdinal("name")),
				Email = reader.GetString(reader.GetOrdinal("email")),
				PhoneNumber = reader.GetString(reader.GetOrdinal("phone_number"))
			});
		}

		return employees;
	}

	/// <summary>
	/// Inserts a new employee into the database.
	/// </summary>
	/// <param name="employee">The employee to insert into the database.</param>
	/// <returns>The created employee.</returns>
	/// <exception cref="InvalidOperationException">Thrown when the employee cannot be created.</exception>
	public Employee Create(Employee employee)
	{
		const string query = """
			INSERT INTO Employees (name, email, phone_number)
			OUTPUT INSERTED.employee_id
			VALUES (@Name, @Email, @PhoneNumber);
		""";

		// Release the connection after use with `using`
		using SqlConnection connection = _dbWorker.Connect();

		SqlCommand command = new SqlCommand(query, connection);
		command.Parameters.AddWithValue("@Name", employee.Name);
		command.Parameters.AddWithValue("@Email", employee.Email);
		command.Parameters.AddWithValue("@PhoneNumber", employee.PhoneNumber);

		object result = command.ExecuteScalar();

		if (result == null || result == DBNull.Value)
		{
			throw new InvalidOperationException("Failed to create employee.");
		}

		return new Employee
		{
			EmployeeId = Convert.ToInt32(result),
			Name = employee.Name,
			Email = employee.Email,
			PhoneNumber = employee.PhoneNumber
		};
	}

	/// <summary>
	/// Updates an existing employee in the database.
	/// </summary>
	/// <param name="employee">The employee to update.</param>
	/// <returns>The updated employee.</returns>
	/// <exception cref="KeyNotFoundException">Thrown when the employee with the specified unique identifier is not found.</exception>
	public Employee Update(Employee employee)
	{
		const string query = """
			UPDATE Employees
			SET name = @Name, email = @Email, phone_number = @PhoneNumber
			WHERE employee_id = @Id;
		""";

		// Release the connection after use with `using`
		using SqlConnection connection = _dbWorker.Connect();

		SqlCommand command = new SqlCommand(query, connection);
		command.Parameters.AddWithValue("@Id", employee.EmployeeId);
		command.Parameters.AddWithValue("@Name", employee.Name);
		command.Parameters.AddWithValue("@Email", employee.Email);
		command.Parameters.AddWithValue("@PhoneNumber", employee.PhoneNumber);

		int rowsAffected = command.ExecuteNonQuery();

		if (rowsAffected == 0)
		{
			throw new KeyNotFoundException($"Employee with ID {employee.EmployeeId} not found.");
		}

		return employee;
	}

	/// <summary>
	/// Deletes an employee from the database by their unique identifier.
	/// </summary>
	/// <param name="id">The unique identifier of the employee to delete.</param>
	/// <returns>The deleted employee.</returns>
	/// <exception cref="KeyNotFoundException">Thrown when the employee with the specified unique identifier is not found.</exception>
	public Employee Delete(int id)
	{
		const string query = """
			DELETE FROM Employees
			OUTPUT DELETED.employee_id, DELETED.name, DELETED.email, DELETED.phone_number
			WHERE employee_id = @Id;
		""";

		// Release the connection after use with `using`
		using SqlConnection connection = _dbWorker.Connect();

		SqlCommand command = new SqlCommand(query, connection);
		command.Parameters.AddWithValue("@Id", id);

		SqlDataReader reader = command.ExecuteReader();

		if (!reader.Read())
		{
			throw new KeyNotFoundException($"Employee with ID {id} not found.");
		}

		return new Employee
		{
			EmployeeId = reader.GetInt32(reader.GetOrdinal("employee_id")),
			Name = reader.GetString(reader.GetOrdinal("name")),
			Email = reader.GetString(reader.GetOrdinal("email")),
			PhoneNumber = reader.GetString(reader.GetOrdinal("phone_number"))
		};
	}
}
