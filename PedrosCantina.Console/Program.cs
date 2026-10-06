using System.Globalization;
using PedrosCantina.Console;
using PedrosCantina.Library;
using PedrosCantina.Library.Data;
using PedrosCantina.Library.Models;
using PedrosCantina.Library.Repositories;
using PedrosCantina.Library.Services;

internal class Program
{
	private static void Main()
	{
		Console.WriteLine("=== Database Seeder ===");
		Console.WriteLine();

		try
		{
			SeedMode mode = ReadSeedMode();

			DBWorker dbWorker = new DBWorker();

			EmployeeRepository employeeRepository = new(dbWorker);

			ManagerRepository managerRepository = new(dbWorker);

			ShiftPeriodRepository shiftPeriodRepository = new(dbWorker);

			ShiftRepository shiftRepository = new(dbWorker);

			EmployeeService employeeService = new(employeeRepository);

			ManagerService managerService = new(managerRepository);

			ShiftPeriodService shiftPeriodService = new(shiftPeriodRepository);

			ShiftService shiftService = new(shiftRepository);

			DatabaseSeeder seeder = new DatabaseSeeder(employeeService, managerService, shiftPeriodService, shiftService, dbWorker);

			if (mode is SeedMode.EmployeesOnly or SeedMode.EmployeesAndShiftPeriods or SeedMode.All)
			{
				List<Employee> employees = new EmployeesToSeed().Employees;

				Console.WriteLine();
				Console.WriteLine($"Seeding {employees.Count} employees...");

				seeder.SeedEmployees(employees);

				Console.WriteLine("Employees seeded successfully.");
			}

			if (mode is SeedMode.EmployeesAndShiftPeriods or SeedMode.All)
			{
				Console.WriteLine();
				Console.WriteLine("Seeding shift periods...");

				seeder.SeedShiftPeriods();

				Console.WriteLine("Shift periods seeded successfully.");
			}

			if (mode is SeedMode.ShiftsOnly or SeedMode.All)
			{
				Console.WriteLine();

				DateOnly startDate = ReadDate("Start date (yyyy-MM-dd): ");

				DateOnly endDate = ReadDate("End date (yyyy-MM-dd): ");

				int density = ReadDensity();

				Console.WriteLine();
				Console.WriteLine("Seeding shifts...");

				SeedResult result = seeder.SeedShifts(startDate, endDate, density);

				Console.WriteLine();
				Console.WriteLine($"Shifts created: {result.Created}");
			}

			Console.WriteLine();
			Console.WriteLine("Seeding complete.");
		}
		catch (Exception ex)
		{
			Console.WriteLine();
			Console.WriteLine("ERROR:");
			Console.WriteLine(ex.Message);
		}
	}

	/// <summary>
	/// Reads the seed mode from the user input and returns the corresponding <see cref="SeedMode"/> value.
	/// If the input is invalid, the user will be prompted to enter a valid option until a valid input is received.
	/// </summary>
	/// <returns>The selected seed mode.</returns>
	private static SeedMode ReadSeedMode()
	{
		while (true)
		{
			Console.WriteLine("Select what you want to seed:");

			Console.WriteLine("1. Shifts only");

			Console.WriteLine("2. Employees only");

			Console.WriteLine("3. Employees + ShiftPeriods");

			Console.WriteLine("4. Everything");

			Console.WriteLine();

			Console.Write("Selection: ");

			string? input = Console.ReadLine();

			if (int.TryParse(input, out int selection) && Enum.IsDefined(typeof(SeedMode), selection))
			{
				return (SeedMode)selection;
			}

			Console.WriteLine("Please select an option from 1 to 4.");

			Console.WriteLine();
		}
	}

	/// <summary>
	/// Reads a date from the user input in the format "yyyy-MM-dd" and returns it as a <see cref="DateOnly"/> value.
	/// </summary>
	/// <param name="prompt">The prompt to display to the user.</param>
	/// <returns>The parsed date.</returns>
	private static DateOnly ReadDate(string prompt)
	{
		while (true)
		{
			Console.Write(prompt);

			string? input = Console.ReadLine();

			if (DateOnly.TryParseExact(input, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly date))
			{
				return date;
			}

			Console.WriteLine("Invalid date. Please use yyyy-MM-dd.");
		}
	}

	/// <summary>
	/// Reads the density value from the user input, which should be an integer between 0 and 100.
	/// </summary>
	/// <returns>The parsed density value.</returns>
	private static int ReadDensity()
	{
		while (true)
		{
			Console.Write("Density (0-100, default 100): ");

			string? input = Console.ReadLine();

			if (string.IsNullOrWhiteSpace(input))
			{
				return 100;
			}

			if (int.TryParse(input, out int density) && density >= 0 && density <= 100)
			{
				return density;
			}

			Console.WriteLine("Density must be an integer between 0 and 100.");
		}
	}
}
