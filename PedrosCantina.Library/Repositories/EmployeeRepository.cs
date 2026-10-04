using Microsoft.Data.SqlClient;
using PedrosCantina.Library.Models;

namespace PedrosCantina.Library.Repositories;

public class EmployeeRepository : ICrudOperations<Employee, int>
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
			FROM employees
			WHERE employee_id = @Id;
		""";

		// Release the connection after use with `using`
		using SqlConnection connection = _dbWorker.Connect();

		using SqlCommand command = new SqlCommand(query, connection);
		command.Parameters.AddWithValue("@Id", id);

		using SqlDataReader reader = command.ExecuteReader();

		if (!reader.Read())
		{
			return null;
		}

		return PopulateEmployee(reader);
	}

	/// <summary>
	/// Reads all employees from the database.
	/// </summary>
	/// <returns>A list of all employees in the database.</returns>
	public List<Employee> ReadAll()
	{
		const string query = """
			SELECT employee_id, name, email, phone_number
			FROM employees;
		""";

		List<Employee> employees = new List<Employee>();

		// Release the connection after use with `using`
		using SqlConnection connection = _dbWorker.Connect();

		using SqlCommand command = new SqlCommand(query, connection);
		using SqlDataReader reader = command.ExecuteReader();

		while (reader.Read())
		{
			employees.Add(PopulateEmployee(reader));
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
			INSERT INTO employees (name, email, phone_number)
			OUTPUT INSERTED.employee_id
			VALUES (@Name, @Email, @PhoneNumber);
		""";

		// Release the connection after use with `using`
		using SqlConnection connection = _dbWorker.Connect();

		using SqlCommand command = new SqlCommand(query, connection);
		command.Parameters.AddWithValue("@Name", employee.Name);
		command.Parameters.AddWithValue("@Email", employee.Email);
		command.Parameters.AddWithValue("@PhoneNumber", employee.PhoneNumber);

		object newEmployeeId = command.ExecuteScalar();

		if (newEmployeeId == null || newEmployeeId == DBNull.Value)
		{
			throw new InvalidOperationException("Failed to create employee.");
		}

		return new Employee(
			employeeId: Convert.ToInt32(newEmployeeId),
			name: employee.Name,
			email: employee.Email,
			phoneNumber: employee.PhoneNumber
		);
	}

	/// <summary>
	/// Updates an existing employee in the database.
	/// </summary>
	/// <param name="employee">The employee to update.</param>
	/// <returns>The updated employee.</returns>
	/// <exception cref="KeyNotFoundException">Thrown when the employee with the specified unique identifier is not found.</exception>
	public void Update(Employee employee)
	{
		const string query = """
			UPDATE employees
			SET name = @Name, email = @Email, phone_number = @PhoneNumber
			WHERE employee_id = @Id;
		""";

		// Release the connection after use with `using`
		using SqlConnection connection = _dbWorker.Connect();

		using SqlCommand command = new SqlCommand(query, connection);
		command.Parameters.AddWithValue("@Id", employee.EmployeeId);
		command.Parameters.AddWithValue("@Name", employee.Name);
		command.Parameters.AddWithValue("@Email", employee.Email);
		command.Parameters.AddWithValue("@PhoneNumber", employee.PhoneNumber);

		int rowsAffected = command.ExecuteNonQuery();

		if (rowsAffected != 1)
		{
			throw new KeyNotFoundException($"Employee with ID {employee.EmployeeId} not found.");
		}
	}

	/// <summary>
	/// Deletes an employee from the database by their unique identifier.
	/// </summary>
	/// <param name="id">The unique identifier of the employee to delete.</param>
	/// <exception cref="KeyNotFoundException">Thrown when the employee with the specified unique identifier is not found.</exception>
	public void Delete(int id)
	{
		const string query = """
			DELETE FROM employees
			WHERE employee_id = @Id;
		""";

		// Release the connection after use with `using`
		using SqlConnection connection = _dbWorker.Connect();

		using SqlCommand command = new SqlCommand(query, connection);
		command.Parameters.AddWithValue("@Id", id);

		int rowsAffected = command.ExecuteNonQuery();

		if (rowsAffected != 1)
		{
			throw new KeyNotFoundException($"Employee with ID {id} not found.");
		}
	}

	/// <summary>
	/// Populates an Employee instance from a SqlDataReader.
	/// </summary>
	/// <param name="reader">The SqlDataReader to read from.</param>
	/// <returns>The populated Employee instance.</returns>
	private Employee PopulateEmployee(SqlDataReader reader)
	{
		return new Employee(
			employeeId: reader.GetInt32(reader.GetOrdinal("employee_id")),
			name: reader.GetString(reader.GetOrdinal("name")),
			email: reader.GetString(reader.GetOrdinal("email")),
			phoneNumber: reader.GetString(reader.GetOrdinal("phone_number"))
		);
	}
}
