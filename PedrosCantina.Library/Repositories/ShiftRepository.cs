using Microsoft.Data.SqlClient;
using PedrosCantina.Library.Models;

namespace PedrosCantina.Library.Repositories;

public class ShiftRepository : IReadOperations<Shift, ShiftKey>, IWriteOperations<Shift, ShiftKey>
{
	/// <summary>
	/// The database worker used to interact with the database.
	/// </summary>
	private readonly DBWorker _dbWorker;

	/// <summary>
	/// Initializes a new instance of the <see cref="ShiftRepository"/> class with the specified database worker.
	/// </summary>
	/// <param name="dbWorker">The database worker to use for interacting with the database.</param>
	public ShiftRepository(DBWorker dbWorker)
	{
		_dbWorker = dbWorker;
	}
	
	/// <summary>
	/// Reads a shift from the database by its ID, including its associated shift period, manager, and employees.
	/// </summary>
	/// <param name="key">The key of the shift to read.</param>
	/// <returns>The shift with the specified key, or null if not found.</returns>
	public Shift? ReadById(ShiftKey key)
	{
		const string query = """
			SELECT
				shift_date, period, start_time, end_time,
				manager_id, manager_name, manager_email, manager_phone_number,
				employee_id, employee_name, employee_email, employee_phone_number
			FROM vw_shift_details
			WHERE shift_date = @date AND period = @period
			ORDER BY shift_date, start_time;
		""";

		Shift? shift = null;

		// Release the connection after use with `using`
		using SqlConnection connection = _dbWorker.Connect();

		using SqlCommand command = new SqlCommand(query, connection);
		command.Parameters.AddWithValue("@date", key.Date);
    	command.Parameters.AddWithValue("@period", key.Period);

		using SqlDataReader reader = command.ExecuteReader();

		// Read the data from the database and populate one Shift instance with its associated ShiftPeriod, Manager, and Employees
		while (reader.Read())
		{
			if (shift == null)
			{
				DateOnly date = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("shift_date")));

				// Populate the ShiftPeriod instance
				ShiftPeriod period = PopulateShiftPeriod(reader);

				// Populate the Manager instance
				Manager manager = PopulateManager(reader);

				shift = new Shift(date, period, manager);
			}

			// Populate the Employee instance and add it to the shift's employee list
			Employee employee = PopulateEmployee(reader);

			shift.AddEmployee(employee);
		}

		return shift;
	}

	/// <summary>
	/// Reads all shifts from the database, including their associated shift periods, managers, and employees.
	/// </summary>
	/// <returns>A list of all shifts in the database.</returns>
	public List<Shift> ReadAll()
	{
		const string query = """
			SELECT
				shift_date, period, start_time, end_time,
				manager_id, manager_name, manager_email, manager_phone_number,
				employee_id, employee_name, employee_email, employee_phone_number
			FROM vw_shift_details
			ORDER BY shift_date, start_time;
		""";

		// Using a dictionary for faster lookups compared to a list
		Dictionary<ShiftKey, Shift> shifts = new Dictionary<ShiftKey, Shift>();

		// Release the connection after use with `using`
		using SqlConnection connection = _dbWorker.Connect();

		using SqlCommand command = new SqlCommand(query, connection);
		using SqlDataReader reader = command.ExecuteReader();

		while (reader.Read())
		{
			DateOnly date = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("shift_date")));

			// Populate the ShiftPeriod instance
			ShiftPeriod period = PopulateShiftPeriod(reader);

			// Populate the Manager instance
			Manager manager = PopulateManager(reader);

			ShiftKey key = new ShiftKey(date, period);
			
			// Check if the shift already exists in the dictionary; if not, create a new Shift instance
			if (!shifts.TryGetValue(key, out Shift? existingShift))
			{
				existingShift = new Shift(date, period, manager);

				shifts.Add(key, existingShift);
			}

			// Populate the Employee instance and add it to the existing shift's employee list
			Employee employee = PopulateEmployee(reader);

			existingShift.AddEmployee(employee);
		}

		return shifts.Values.ToList();
	}

	/// <summary>
	/// Creates a new shift in the database, including its associated shift period, manager, and employees.
	/// </summary>
	/// <param name="shift">The shift to create.</param>
	/// <returns>The created shift with a generated ID.</returns>
	public Shift Create(Shift shift)
	{
		const string query1 = """
			INSERT INTO shifts ([date], period, manager_id)
			VALUES (@date, @period, @manager_id);
		""";

		const string query2 = """
			INSERT INTO employee_shifts (employee_id, shift_date, shift_period)
			VALUES (@employee_id, @date, @period);
		""";

		// Release the connection after use with `using`
		using SqlConnection connection = _dbWorker.Connect();
		using SqlTransaction transaction = connection.BeginTransaction();

		try
		{
			using SqlCommand command = new SqlCommand(query1, connection, transaction);
			command.Parameters.AddWithValue("@date", shift.Date);
			command.Parameters.AddWithValue("@period", shift.Period.Code);
			command.Parameters.AddWithValue("@manager_id", shift.Manager.EmployeeId);

			int rowsAffected = command.ExecuteNonQuery();

			if (rowsAffected != 1)
			{
				throw new InvalidOperationException("Failed to create the shift.");
			}

			foreach (Employee employee in shift.Employees.Values)
			{
				using SqlCommand command2 = new SqlCommand(query2, connection, transaction);
				command2.Parameters.AddWithValue("@employee_id", employee.EmployeeId);
				command2.Parameters.AddWithValue("@date", shift.Date);
				command2.Parameters.AddWithValue("@period", shift.Period.Code);

				command2.ExecuteNonQuery();
			}
			
			transaction.Commit();

			return shift;
		}
		catch (Exception)
		{
			transaction.Rollback();
			throw;
		}
	}

	/// <summary>
	/// Updates an existing shift in the database, including its associated shift period, manager, and employees.
	/// </summary>
	/// <param name="shift">The shift to update.</param>
	public void Update(Shift shift)
	{
		const string query1 = """
			UPDATE shifts
			SET [date] = @date, period = @period, manager_id = @manager_id
			WHERE [date] = @date AND period = @period;
		""";

		const string query2 = """
			DELETE FROM employee_shifts
			WHERE shift_date = @date AND shift_period = @period;
		""";

		const string query3 = """
			INSERT INTO employee_shifts (employee_id, shift_date, shift_period)
			VALUES (@employee_id, @date, @period);
		""";

		// Release the connection after use with `using`
		using SqlConnection connection = _dbWorker.Connect();
		using SqlTransaction transaction = connection.BeginTransaction();

		try
		{
			// Update the shift details in the shifts table
			using SqlCommand command1 = new SqlCommand(query1, connection, transaction);
			command1.Parameters.AddWithValue("@date", shift.Date);
			command1.Parameters.AddWithValue("@period", shift.Period.Code);
			command1.Parameters.AddWithValue("@manager_id", shift.Manager.EmployeeId);

			int rowsAffected1 = command1.ExecuteNonQuery();

			if (rowsAffected1 != 1)
			{
				throw new KeyNotFoundException($"Shift with Date {shift.Date} and Period {shift.Period} not found.");
			}

			// Clear existing employee associations for the shift in the employee_shifts junction table,
			// as the number of employees assigned to the shift may have changed
			using SqlCommand command2 = new SqlCommand(query2, connection, transaction);
			command2.Parameters.AddWithValue("@date", shift.Date);
			command2.Parameters.AddWithValue("@period", shift.Period.Code);

			command2.ExecuteNonQuery();

			// Re-insert the updated employee associations for the shift in the employee_shifts junction table
			foreach (Employee employee in shift.Employees.Values)
			{
				using SqlCommand command3 = new SqlCommand(query3, connection, transaction);
				command3.Parameters.AddWithValue("@employee_id", employee.EmployeeId);
				command3.Parameters.AddWithValue("@date", shift.Date);
				command3.Parameters.AddWithValue("@period", shift.Period.Code);

				command3.ExecuteNonQuery();
			}

			transaction.Commit();
		}
		catch (Exception)
		{
			transaction.Rollback();
			throw;
		}
	}

	/// <summary>
	/// Deletes a shift from the database by its unique identifier.
	/// </summary>
	/// <param name="key">The key of the shift to delete.</param>
	/// <exception cref="KeyNotFoundException">Thrown when the shift with the specified unique identifier is not found.</exception>
	public void Delete(ShiftKey key)
	{
		const string query = """
			DELETE FROM shifts
			WHERE [date] = @date AND period = @period;
		""";

		// Release the connection after use with `using`
		using SqlConnection connection = _dbWorker.Connect();

		using SqlCommand command = new SqlCommand(query, connection);
		command.Parameters.AddWithValue("@date", key.Date);
		command.Parameters.AddWithValue("@period", key.Period);

		int rowsAffected = command.ExecuteNonQuery();
		
		if (rowsAffected != 1)
		{
			throw new KeyNotFoundException($"Shift with Date {key.Date} and Period {key.Period} not found.");
		}
	}

	/// <summary>
	/// Reads all shifts for a specific month and year from the database, including their associated shift periods, managers, and employees.
	/// </summary>
	/// <param name="year">The year for which to read shifts.</param>
	/// <param name="month">The month for which to read shifts.</param>
	/// <returns>A list of shifts for the specified month and year.</returns>
	public List<Shift> ReadByMonth(int year, int month)
	{
		const string query = """
			SELECT
				shift_date, period, start_time, end_time,
				manager_id, manager_name, manager_email, manager_phone_number,
				employee_id, employee_name, employee_email, employee_phone_number
			FROM vw_shift_details
			WHERE shift_date >= DATEFROMPARTS(@year, @month, 1) AND shift_date < DATEADD(MONTH, 1, DATEFROMPARTS(@year, @month, 1))
			ORDER BY shift_date, start_time;
		""";

		Dictionary<ShiftKey, Shift> shifts = new Dictionary<ShiftKey, Shift>();

		// Release the connection after use with `using`
		using SqlConnection connection = _dbWorker.Connect();

		using SqlCommand command = new SqlCommand(query, connection);
		command.Parameters.AddWithValue("@year", year);
		command.Parameters.AddWithValue("@month", month);

		using SqlDataReader reader = command.ExecuteReader();

		while (reader.Read())
		{
			DateOnly date = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("shift_date")));

			ShiftPeriod period = PopulateShiftPeriod(reader);
			Manager manager = PopulateManager(reader);

			ShiftKey key = new ShiftKey(date, period);
			
			// Check if the shift already exists in the dictionary; if not, create a new Shift instance
			if (!shifts.TryGetValue(key, out Shift? existingShift))
			{
				existingShift = new Shift(date, period, manager);
				shifts.Add(key, existingShift);
			}

			Employee employee = PopulateEmployee(reader);
			existingShift.AddEmployee(employee);
		}

		return shifts.Values.ToList();
	}

	/// <summary>
	/// Reads the number of shifts worked by a specific employee in a given month and year from the database.
	/// </summary>
	/// <param name="date">The year and month for which to read shifts.</param>
	/// <param name="employeeId">The ID of the employee for whom to read shifts.</param>
	/// <returns>The number of shifts worked by the specified employee in the given month and year.</returns>
	public int ReadMonthlyShiftsByEmployeeId(DateOnly date, int employeeId)
	{
		const string query = """
			SELECT COUNT(*) AS shifts_worked
			FROM vw_shift_details
			WHERE shift_date >= DATEFROMPARTS(@year, @month, 1) AND shift_date < DATEADD(MONTH, 1, DATEFROMPARTS(@year, @month, 1)) 
				AND employee_id = @employee_id
			GROUP BY employee_id, employee_name, employee_email, employee_phone_number
			ORDER BY shifts_worked DESC;
		""";

		// Release the connection after use with `using`
		using SqlConnection connection = _dbWorker.Connect();

		using SqlCommand command = new SqlCommand(query, connection);
		command.Parameters.AddWithValue("@year", date.Year);
		command.Parameters.AddWithValue("@month", date.Month);
		command.Parameters.AddWithValue("@employee_id", employeeId);

		using SqlDataReader reader = command.ExecuteReader();

		if (!reader.Read())
		{
			return 0; // No shifts found for the specified employee in the given month and year
		}

		return reader.GetInt32(reader.GetOrdinal("shifts_worked"));
	}

	/// <summary>
	/// Helper method to populate a ShiftPeriod instance from a SqlDataReader.
	/// </summary>
	/// <param name="reader">The SqlDataReader containing the data.</param>
	/// <returns>The populated ShiftPeriod instance.</returns>
	private ShiftPeriod PopulateShiftPeriod(SqlDataReader reader)
	{
		return new ShiftPeriod(
			code: reader.GetString(reader.GetOrdinal("period")),
			startTime: TimeOnly.FromTimeSpan(reader.GetTimeSpan(reader.GetOrdinal("start_time"))),
			endTime: TimeOnly.FromTimeSpan(reader.GetTimeSpan(reader.GetOrdinal("end_time")))
		);
	}

	/// <summary>
	/// Helper method to populate a Manager instance from a SqlDataReader.
	/// </summary>
	/// <param name="reader">The SqlDataReader containing the data.</param>
	/// <returns>The populated Manager instance.</returns>
	private Manager PopulateManager(SqlDataReader reader)
	{
		return new Manager(
			employeeId: reader.GetInt32(reader.GetOrdinal("manager_id")),
			name: reader.GetString(reader.GetOrdinal("manager_name")),
			email: reader.GetString(reader.GetOrdinal("manager_email")),
			phoneNumber: reader.GetString(reader.GetOrdinal("manager_phone_number"))
		);
	}

	/// <summary>
	/// Helper method to populate an Employee instance from a SqlDataReader.
	/// </summary>
	/// <param name="reader">The SqlDataReader containing the data.</param>
	/// <returns>The populated Employee instance.</returns>
	private Employee PopulateEmployee(SqlDataReader reader)
	{
		return new Employee(
			employeeId: reader.GetInt32(reader.GetOrdinal("employee_id")),
			name: reader.GetString(reader.GetOrdinal("employee_name")),
			email: reader.GetString(reader.GetOrdinal("employee_email")),
			phoneNumber: reader.GetString(reader.GetOrdinal("employee_phone_number"))
		);
	}
}
