using Microsoft.Data.SqlClient;
using PedrosCantina.Library.Models;
using PedrosCantina.Library.Services;

namespace PedrosCantina.Library.Data;

public class DatabaseSeeder
{
	/// <summary>
	/// The employee service used to perform CRUD operations on employees.
	/// </summary>
	private readonly EmployeeService _employeeService;

	/// <summary>
	/// The manager service used to perform CRUD operations on managers.
	/// </summary>
	private readonly ManagerService _managerService;

	/// <summary>
	/// The shift period service used to perform CRUD operations on shift periods.
	/// </summary>
	private readonly ShiftPeriodService _shiftPeriodService;

	/// <summary>
	/// The shift service used to perform CRUD operations on shifts.
	/// </summary>
	private readonly ShiftService _shiftService;

	/// <summary>
	/// The database worker used to connect to the database and execute SQL commands.
	/// </summary>
	private readonly DBWorker _dbWorker;

	/// <summary>
	/// The random number generator used to select random employees and managers for shifts.
	/// </summary>
	private readonly Random _random = new Random();

	/// <summary>
	/// Initializes a new instance of the <see cref="DatabaseSeeder"/> class with the specified services and database worker.
	/// </summary>
	/// <param name="employeeService">The employee service used to perform CRUD operations on employees.</param>
	/// <param name="managerService">The manager service used to perform CRUD operations on managers.</param>
	/// <param name="shiftPeriodService">The shift period service used to perform CRUD operations on shift periods.</param>
	/// <param name="shiftService">The shift service used to perform CRUD operations on shifts.</param>
	/// <param name="dbWorker">The database worker used to connect to the database and execute SQL commands.</param>
	public DatabaseSeeder(EmployeeService employeeService, ManagerService managerService, ShiftPeriodService shiftPeriodService, ShiftService shiftService, DBWorker dbWorker)
	{
		_employeeService = employeeService;
		_managerService = managerService;
		_shiftPeriodService = shiftPeriodService;
		_shiftService = shiftService;
		_dbWorker = dbWorker;
	}

	/// <summary>
	/// Seeds the database with the specified employees, shift periods, and shifts.
	/// </summary>
	/// <param name="employees">The list of employees to seed the database with.</param>
	public void SeedEmployees(List<Employee> employees)
	{
		ArgumentNullException.ThrowIfNull(employees);

		foreach (Employee employee in employees)
		{
			if (employee is Manager manager)
			{
				_managerService.AddManager(manager);
			}
			else
			{
				_employeeService.AddEmployee(employee);
			}
		}
	}

	/// <summary>
	/// Seeds the database with the specified shift periods.
	/// </summary>
	public void SeedShiftPeriods()
	{
		ShiftPeriod[] periods =
		[
			new ShiftPeriod("Morgen", new TimeOnly(9, 0), new TimeOnly(14, 0)),
			new ShiftPeriod("Aften", new TimeOnly(14, 0), new TimeOnly(19, 0))
		];

		// Release the connection after use with `using`
		using SqlConnection connection = _dbWorker.Connect();

		foreach (ShiftPeriod period in periods)
		{
			const string query = """
				INSERT INTO shift_periods (period, start_time, end_time)
				VALUES (@Period, @StartTime, @EndTime)
			""";

			using SqlCommand command = new(query, connection);
			command.Parameters.AddWithValue("@Period", period.Code);
			command.Parameters.AddWithValue("@StartTime", period.StartTime);
			command.Parameters.AddWithValue("@EndTime", period.EndTime);

			command.ExecuteNonQuery();
		}
	}

	/// <summary>
	/// Seeds the database with shifts for the specified date range and density.
	/// </summary>
	/// <param name="startDate">The start date of the range to seed shifts for.</param>
	/// <param name="endDate">The end date of the range to seed shifts for.</param>
	/// <param name="density">The density of shifts to create (0-100).</param>
	/// <returns>The result of the seeding operation.</returns>
	public SeedResult SeedShifts(DateOnly startDate, DateOnly endDate, int density = 100)
	{
		ValidateShiftParameters(startDate, endDate, density);

		List<Employee> employees = _employeeService.GetAllEmployees();

		List<Manager> managers = _managerService.GetAllManagers();

		List<ShiftPeriod> periods = _shiftPeriodService.GetAllShiftPeriods();

		ValidateShiftDatabaseState(employees, managers, periods);

		int created = 0;

		// Loop through each date in the specified range and create shifts for each shift period
		for (DateOnly date = startDate; date <= endDate; date = date.AddDays(1))
		{
			foreach (ShiftPeriod period in periods)
			{
				if (!ShouldCreateShift(density))
				{
					continue;
				}

				Manager manager = GetRandom(managers);

				Shift shift = new Shift(date, period, manager);

				int additionalEmployeeCount = _random.Next(1, 3);

				List<Employee> additionalEmployees = GetRandomEmployees(employees, manager, additionalEmployeeCount);

				foreach (Employee employee in additionalEmployees)
				{
					shift.AddEmployee(employee);
				}

				_shiftService.AddShift(shift);

				created++;
			}
		}

		return new SeedResult(created);
	}

	/// <summary>
	/// Helper method to validate the parameters for seeding shifts.
	/// It checks that the start date is before or equal to the end date, and that the density is between 0 and 100.
	/// If any of these conditions are not met, an appropriate exception is thrown.
	/// </summary>
	/// <param name="startDate">The start date of the range to seed shifts for.</param>
	/// <param name="endDate">The end date of the range to seed shifts for.</param>
	/// <param name="density">The density of shifts to create (0-100).</param>
	/// <exception cref="ArgumentException">Thrown when the start date is after the end date.</exception>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when the density is not between 0 and 100.</exception>
	private static void ValidateShiftParameters(DateOnly startDate, DateOnly endDate, int density)
	{
		if (startDate > endDate)
		{
			throw new ArgumentException("Start date must be before or equal to the end date.");
		}

		if (density < 0 || density > 100)
		{
			throw new ArgumentOutOfRangeException(nameof(density), density, "Density must be between 0 and 100.");
		}
	}

	/// <summary>
	/// Helper method to validate the state of the database before seeding shifts.
	/// It checks that there are at least 2 employees, at least 1 manager, and at least 1 shift period in the database.
	/// If any of these conditions are not met, an InvalidOperationException is thrown with a descriptive message.
	/// </summary>
	/// <param name="employees">The list of employees in the database.</param>
	/// <param name="managers">The list of managers in the database.</param>
	/// <param name="periods">The list of shift periods in the database.</param>
	/// <exception cref="InvalidOperationException">Thrown when the database is not in a valid state for seeding shifts.</exception>
	private static void ValidateShiftDatabaseState(List<Employee> employees, List<Manager> managers, List<ShiftPeriod> periods)
	{
		if (employees.Count < 2)
		{
			throw new InvalidOperationException("At least 2 employees are required to seed shifts.");
		}

		if (managers.Count == 0)
		{
			throw new InvalidOperationException("At least 1 manager is required to seed shifts.");
		}

		if (periods.Count == 0)
		{
			throw new InvalidOperationException("No shift periods were found.");
		}
	}

	/// <summary>
	/// Helper method to determine whether to create a shift based on the specified density.
	/// A density of 0 means no shifts will be created, while a density of 100 means all shifts will be created.
	/// For densities between 0 and 100, a random number is generated to determine whether to create a shift.
	/// </summary>
	/// <param name="density">The density of shifts to create (0-100).</param>
	/// <returns>True if a shift should be created, false otherwise.</returns>
	private bool ShouldCreateShift(int density)
	{
		if (density == 0)
		{
			return false;
		}

		if (density == 100)
		{
			return true;
		}

		return _random.Next(0, 100) < density;
	}

	/// <summary>
	/// Helper method to get a list of random employees from the specified list, excluding the specified manager.
	/// </summary>
	/// <param name="employees">The list of employees to choose from.</param>
	/// <param name="manager">The manager to exclude from the list.</param>
	/// <param name="count">The number of employees to select.</param>
	/// <returns>A list of randomly selected employees.</returns>
	/// <exception cref="InvalidOperationException">Thrown when there are not enough employees to select from.</exception>
	private List<Employee> GetRandomEmployees(List<Employee> employees, Manager manager, int count)
	{
		List<Employee> availableEmployees = employees.Where(employee => employee.EmployeeId != manager.EmployeeId).ToList();

		if (availableEmployees.Count < count)
		{
			throw new InvalidOperationException($"Not enough employees to create a shift with " + $"{count + 1} employees.");
		}

		List<Employee> selectedEmployees = new List<Employee>();

		// Randomly select the specified number of employees from the available employees
		for (int i = 0; i < count; i++)
		{
			int index = _random.Next(availableEmployees.Count);

			selectedEmployees.Add(availableEmployees[index]);

			availableEmployees.RemoveAt(index);
		}

		return selectedEmployees;
	}

	/// <summary>
	/// Helper method to get a random item from the specified list.
	/// </summary>
	/// <typeparam name="T">The type of the items in the list.</typeparam>
	/// <param name="items">The list of items to choose from.</param>
	/// <returns>A randomly selected item from the list.</returns>
	private T GetRandom<T>(List<T> items)
	{
		return items[_random.Next(items.Count)];
	}
}

/// <summary>
/// Represents the result of a database seeding operation, including the number of records created.
/// </summary>
/// <param name="Created">The number of records created.</param>
public readonly record struct SeedResult(int Created);
