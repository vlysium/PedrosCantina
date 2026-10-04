namespace PedrosCantina.Library.Models;

public class Shift
{
	/// <summary>
	/// Gets the employees assigned to the shift, where the key is the employee's unique identifier.
	/// </summary>
	private readonly Dictionary<int, Employee> _employees = new Dictionary<int, Employee>();

	/// <summary>
	/// Gets the composite key for the shift for identifying it uniquely based on the date and period.
	/// </summary>
	public ShiftKey Key => new ShiftKey(Date, Period);

	/// <summary>
	/// Gets or sets the date of the shift.
	/// </summary>
	public DateOnly Date { get; set; }

	/// <summary>
	/// Gets or sets the period of the shift.
	/// </summary>
	public ShiftPeriod Period { get; set; }

	/// <summary>
	/// Gets the employees assigned to the shift, where the key is the employee's unique identifier.
	/// </summary>
	public IReadOnlyDictionary<int, Employee> Employees => _employees;

	/// <summary>
	/// Gets or sets the manager responsible for the shift.
	/// </summary>
	public Manager Manager { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="Shift"/> class with the specified manager. The manager is automatically added to the shift's employee list.
	/// </summary>
	/// <param name="manager">The manager responsible for the shift.</param>
	public Shift(Manager manager)
	{
		Manager = manager;
		AddEmployee(manager); // Automatically add the manager to the shift's employee list to ensure they are part of the shift.
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Shift"/> class with the specified date, period, and manager.
	/// </summary>
	/// <param name="date">The date of the shift.</param>
	/// <param name="period">The period of the shift.</param>
	/// <param name="manager">The manager responsible for the shift.</param>
	public Shift(DateOnly date, ShiftPeriod period, Manager manager): this(manager)
	{
		Date = date;
		Period = period;
	}

	/// <summary>
	/// Adds an employee to the shift. Only up to 3 employees can be assigned to a shift.
	/// </summary>
	public void AddEmployee(Employee employee)
	{
		ArgumentNullException.ThrowIfNull(employee);

		if (_employees.ContainsKey(employee.EmployeeId))
		{
			throw new ArgumentException($"Employee is already assigned to this shift.", nameof(employee));
		}

		if (_employees.Count >= 3)
		{
			throw new InvalidOperationException("Cannot add more than 3 employees to a shift.");
		}

		_employees.Add(employee.EmployeeId, employee);
	}

	/// <summary>
	/// Removes an employee from the shift.
	/// </summary>
	public void RemoveEmployee(Employee employee)
	{
		ArgumentNullException.ThrowIfNull(employee);

		if (employee.EmployeeId == Manager.EmployeeId)
		{
			throw new ArgumentException("Cannot remove the manager from the shift.", nameof(employee));
		}

		if (!_employees.ContainsKey(employee.EmployeeId))
		{
			throw new InvalidOperationException("Employee is not assigned to this shift.");
		}

		_employees.Remove(employee.EmployeeId);
	}

	public override string ToString()
	{
		return $"Date: {Date}, Period: {Period}, Manager: {Manager.Name}, Employees: [{string.Join(", ", _employees.Values.Select(e => e.Name))}]";
	}
}
