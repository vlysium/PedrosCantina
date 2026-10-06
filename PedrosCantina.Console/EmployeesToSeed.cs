using PedrosCantina.Library.Models;

namespace PedrosCantina.Console;

public class EmployeesToSeed
{
	/// <summary>
	/// The random number generator used to generate random phone numbers for employees.
	/// </summary>
	private readonly Random _random = new Random();

	/// <summary>
	/// Gets the list of employees to seed into the database.
	/// </summary>
	public List<Employee> Employees { get; } = new List<Employee>();

	/// <summary>
	/// Initializes a new instance of the <see cref="EmployeesToSeed"/> class and populates the list of employees to seed into the database.
	/// Modify this list to add or remove employees as needed.
	/// </summary>
	public EmployeesToSeed()
	{
		Employees.Add(new Employee("Alice", "alice@email.com", RandomPhoneNumber()));
		Employees.Add(new Employee("Björn", "bjorn@email.com", RandomPhoneNumber()));
		Employees.Add(new Employee("Cecilie", "cecilie@email.com", RandomPhoneNumber()));
		Employees.Add(new Employee("Diego", "diego@email.com", RandomPhoneNumber()));
		Employees.Add(new Employee("Elena", "elena@email.com", RandomPhoneNumber()));
		Employees.Add(new Employee("Frederik", "frederik@email.com", RandomPhoneNumber()));
		Employees.Add(new Employee("Gustav", "gustav@email.com", RandomPhoneNumber()));
		Employees.Add(new Employee("Hassan", "hassan@email.com", RandomPhoneNumber()));
		Employees.Add(new Employee("Isabella", "isabella@email.com", RandomPhoneNumber()));
		Employees.Add(new Manager("Jacoby", "jacoby@email.com", RandomPhoneNumber()));
		Employees.Add(new Employee("Kerim", "kerim@email.com", RandomPhoneNumber()));
		Employees.Add(new Employee("Laura", "laura@email.com", RandomPhoneNumber()));
		Employees.Add(new Employee("Mei", "mei@email.com", RandomPhoneNumber()));
		Employees.Add(new Employee("Naomi", "naomi@email.com", RandomPhoneNumber()));
		Employees.Add(new Employee("Oliver", "oliver@email.com", RandomPhoneNumber()));
		Employees.Add(new Manager("Pedro", "pedro@email.com", RandomPhoneNumber()));
		Employees.Add(new Employee("Quinn", "quinn@email.com", RandomPhoneNumber()));
		Employees.Add(new Employee("Rosa", "rosa@email.com", RandomPhoneNumber()));
		Employees.Add(new Employee("Søren", "soren@email.com", RandomPhoneNumber()));
		Employees.Add(new Employee("Tomasz", "tomasz@email.com", RandomPhoneNumber()));
		Employees.Add(new Employee("Ulrich", "ulrich@email.com", RandomPhoneNumber()));
		Employees.Add(new Employee("Victor", "victor@email.com", RandomPhoneNumber()));
		Employees.Add(new Employee("Wyatt", "wyatt@email.com", RandomPhoneNumber()));
		Employees.Add(new Employee("Xiaoyu", "xiaoyu@email.com", RandomPhoneNumber()));
		Employees.Add(new Employee("Yasmin", "yasmin@email.com", RandomPhoneNumber()));
		Employees.Add(new Employee("Zara", "zara@email.com", RandomPhoneNumber()));
	}

	/// <summary>
	/// Generates a random phone number in the format of an 8-digit string, padded with leading zeros if necessary.
	/// </summary>
	/// <returns>The generated phone number.</returns>
	private string RandomPhoneNumber()
	{
		return _random.Next(00000000, 99999999).ToString().PadLeft(8, '0');
	}
}
