using Microsoft.Data.SqlClient;
using PedrosCantina.Library.Models;

namespace PedrosCantina.Library.Repositories;

public class ShiftRepository : ICrudOperations<Shift>
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
	/// <param name="id">The ID of the shift to read.</param>
	/// <returns>The shift with the specified ID, or null if not found.</returns>
	public Shift? ReadById(int id)
	{
		const string query = """
			SELECT
				s.shift_id, s.[date] AS shift_date,
				sp.period, sp.start_time, sp.end_time,
				m.manager_id, manager.name AS manager_name, manager.email AS manager_email, manager.phone_number AS manager_phone_number,
				e.employee_id, e.name AS employee_name, e.email AS employee_email, e.phone_number AS employee_phone_number
			FROM shifts AS s
			JOIN shift_periods AS sp ON s.period = sp.period
			JOIN managers AS m ON s.manager_id = m.manager_id
			JOIN employees AS manager ON m.manager_id = manager.employee_id
			JOIN employee_shifts AS es ON s.shift_id = es.shift_id
			JOIN employees AS e ON es.employee_id = e.employee_id
			WHERE s.shift_id = @ShiftId
			ORDER BY shift_date, sp.start_time;
		""";

		Shift? shift = null;

		// Release the connection after use with `using`
		using SqlConnection connection = _dbWorker.Connect();

		using SqlCommand command = new SqlCommand(query, connection);
		command.Parameters.AddWithValue("@ShiftId", id);

		using SqlDataReader reader = command.ExecuteReader();

		// Read the data from the database and populate one Shift instance with its associated ShiftPeriod, Manager, and Employees
		while (reader.Read())
		{
			if (shift == null)
			{
				int shiftId = reader.GetInt32(reader.GetOrdinal("shift_id"));
				DateOnly date = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("shift_date")));

				// Populate the ShiftPeriod instance
				ShiftPeriod period = PopulateShiftPeriod(reader);

				// Populate the Manager instance
				Manager manager = PopulateManager(reader);

				shift = new Shift(shiftId, date, period, manager);
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
				s.shift_id, s.[date] AS shift_date,
				sp.period, sp.start_time, sp.end_time,
				m.manager_id, manager.name AS manager_name, manager.email AS manager_email, manager.phone_number AS manager_phone_number,
				e.employee_id, e.name AS employee_name, e.email AS employee_email, e.phone_number AS employee_phone_number
			FROM shifts AS s
			JOIN shift_periods AS sp ON s.period = sp.period
			JOIN managers AS m ON s.manager_id = m.manager_id
			JOIN employees AS manager ON m.manager_id = manager.employee_id
			JOIN employee_shifts AS es ON s.shift_id = es.shift_id
			JOIN employees AS e ON es.employee_id = e.employee_id
			ORDER BY shift_date, sp.start_time;
		""";

		// Using a dictionary for faster lookups compared to a list
		Dictionary<int, Shift> shifts = new Dictionary<int, Shift>();

		// Release the connection after use with `using`
		using SqlConnection connection = _dbWorker.Connect();

		using SqlCommand command = new SqlCommand(query, connection);
		using SqlDataReader reader = command.ExecuteReader();

		while (reader.Read())
		{
			int shiftId = reader.GetInt32(reader.GetOrdinal("shift_id"));
			DateOnly date = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("shift_date")));

			// Populate the ShiftPeriod instance
			ShiftPeriod period = PopulateShiftPeriod(reader);

			// Populate the Manager instance
			Manager manager = PopulateManager(reader);

			// Check if the shift already exists in the dictionary; if not, create a new Shift instance
			if (!shifts.TryGetValue(shiftId, out Shift? existingShift))
			{
				existingShift = new Shift(shiftId, date, period, manager);
				shifts.Add(shiftId, existingShift);
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
			OUTPUT INSERTED.shift_id
			VALUES (@Date, @Period, @ManagerId);
		""";

		const string query2 = """
			INSERT INTO employee_shifts (employee_id, shift_id)
			VALUES (@EmployeeId, @ShiftId);
		""";

		// Release the connection after use with `using`
		using SqlConnection connection = _dbWorker.Connect();
		using SqlTransaction transaction = connection.BeginTransaction();

		try
		{
			using SqlCommand command = new SqlCommand(query1, connection, transaction);
			command.Parameters.AddWithValue("@Date", shift.Date);
			command.Parameters.AddWithValue("@Period", shift.Period.Code);
			command.Parameters.AddWithValue("@ManagerId", shift.Manager.EmployeeId);

			int shiftId = (int)command.ExecuteScalar()!;

			foreach (Employee employee in shift.Employees)
			{
				// Don't insert into employee_shifts if the employee is the manager of the shift
				if (employee.EmployeeId != shift.Manager.EmployeeId)
				{
					using SqlCommand command2 = new SqlCommand(query2, connection, transaction);
					command2.Parameters.AddWithValue("@EmployeeId", employee.EmployeeId);
					command2.Parameters.AddWithValue("@ShiftId", shiftId);

					command2.ExecuteNonQuery();
				}
			}

			transaction.Commit();
			
			// Create a new Shift instance to return, which includes the newly generated shift ID from the database
			Shift createdShift = new Shift(shiftId, shift.Date, shift.Period, shift.Manager);
			foreach (Employee employee in shift.Employees)
			{
				// Don't add the manager to the employee list again, as they are already part of the shift
				if (employee.EmployeeId != shift.Manager.EmployeeId)
				{
					createdShift.AddEmployee(employee);
				}
			}

			return createdShift;
		}
		catch (Exception)
		{
			transaction.Rollback();
			throw;
		}
	}

	public Shift Update(Shift entity)
	{
		throw new NotImplementedException();
	}

	public Shift Delete(int id)
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// Helper method to populate a ShiftPeriod instance from a SqlDataReader.
	/// </summary>
	/// <param name="reader">The SqlDataReader containing the data.</param>
	/// <returns>The populated ShiftPeriod instance.</returns>
	private ShiftPeriod PopulateShiftPeriod(SqlDataReader reader)
	{
		return new ShiftPeriod
		{
			Code = reader.GetString(reader.GetOrdinal("period")),
			StartTime = TimeOnly.FromTimeSpan(reader.GetTimeSpan(reader.GetOrdinal("start_time"))),
			EndTime = TimeOnly.FromTimeSpan(reader.GetTimeSpan(reader.GetOrdinal("end_time")))
		};
	}

	/// <summary>
	/// Helper method to populate a Manager instance from a SqlDataReader.
	/// </summary>
	/// <param name="reader">The SqlDataReader containing the data.</param>
	/// <returns>The populated Manager instance.</returns>
	private Manager PopulateManager(SqlDataReader reader)
	{
		return new Manager
		{
			EmployeeId = reader.GetInt32(reader.GetOrdinal("manager_id")),
			Name = reader.GetString(reader.GetOrdinal("manager_name")),
			Email = reader.GetString(reader.GetOrdinal("manager_email")),
			PhoneNumber = reader.GetString(reader.GetOrdinal("manager_phone_number"))
		};
	}

	/// <summary>
	/// Helper method to populate an Employee instance from a SqlDataReader.
	/// </summary>
	/// <param name="reader">The SqlDataReader containing the data.</param>
	/// <returns>The populated Employee instance.</returns>
	private Employee PopulateEmployee(SqlDataReader reader)
	{
		return new Employee
		{
			EmployeeId = reader.GetInt32(reader.GetOrdinal("employee_id")),
			Name = reader.GetString(reader.GetOrdinal("employee_name")),
			Email = reader.GetString(reader.GetOrdinal("employee_email")),
			PhoneNumber = reader.GetString(reader.GetOrdinal("employee_phone_number"))
		};
	}
}
