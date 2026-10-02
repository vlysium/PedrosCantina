namespace PedrosCantina.Library.Models;

public class Shift
{
	/// <summary>
	/// The list of employees assigned to the shift.
	/// </summary>
	private readonly List<Employee> _employees = new List<Employee>();

	/// <summary>
	/// Gets or sets the unique identifier for the shift.
	/// </summary>
	public int ShiftId { get; init; }

	/// <summary>
	/// Gets or sets the date of the shift.
	/// </summary>
	public DateOnly Date { get; init; }

	/// <summary>
	/// Gets or sets the period of the shift.
	/// </summary>
	public ShiftPeriod Period { get; init; }

	/// <summary>
	/// Gets the list of employees assigned to the shift.
	/// </summary>
	public IReadOnlyList<Employee> Employees { get => _employees; }

	/// <summary>
	/// Gets or sets the manager responsible for the shift.
	/// </summary>
	public Manager Manager { get; init; }

	/// <summary>
	/// Initializes a new instance of the <see cref="Shift"/> class.
	/// </summary>
	/// <param name="manager">The manager responsible for the shift.</param>
	public Shift(Manager manager)
	{
		Manager = manager;
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
	/// Initializes a new instance of the <see cref="Shift"/> class with the specified shift ID, date, period, and manager.
	/// </summary>
	/// <param name="shiftId">The unique identifier for the shift.</param>
	/// <param name="date">The date of the shift.</param>
	/// <param name="period">The period of the shift.</param>
	/// <param name="manager">The manager responsible for the shift.</param>
	public Shift(int shiftId, DateOnly date, ShiftPeriod period, Manager manager): this(date, period, manager)
	{
		ShiftId = shiftId;
	}

	/// <summary>
	/// Adds an employee to the shift. Only up to 3 employees can be assigned to a shift.
	/// </summary>
	/// <param name="employee">The employee to add to the shift.</param>
	/// <exception cref="ArgumentNullException">Thrown when the employee is null.</exception>
	/// <exception cref="ArgumentException">Thrown when the employee is already assigned to the shift.</exception>
	/// <exception cref="InvalidOperationException">Thrown when the maximum number of employees (3) is exceeded.</exception>
	public void AddEmployee(Employee employee)
	{
		ArgumentNullException.ThrowIfNull(employee, nameof(employee));

		if (_employees.Contains(employee))
		{
			throw new ArgumentException("Employee is already assigned to this shift.", nameof(employee));
		}

		if (_employees.Count >= 3)
		{
			throw new InvalidOperationException("Cannot add more than 3 employees to a shift.");
		}

		_employees.Add(employee);
	}

	/// <summary>
	/// Removes an employee from the shift.
	/// </summary>
	/// <param name="employee">The employee to remove from the shift.</param>
	/// <exception cref="ArgumentNullException">Thrown when the employee is null.</exception>
	/// <exception cref="ArgumentException">Thrown when the employee is the manager of the shift.</exception>
	/// <exception cref="InvalidOperationException">Thrown when the employee is not assigned to the shift.</exception>
	public void RemoveEmployee(Employee employee)
	{
		ArgumentNullException.ThrowIfNull(employee, nameof(employee));

		if (employee == Manager)
		{
			throw new ArgumentException("Cannot remove the manager from the shift.", nameof(employee));
		}

		if (!_employees.Contains(employee))
		{
			throw new InvalidOperationException("Employee is not assigned to this shift.");
		}

		_employees.Remove(employee);
	}

	public override string ToString()
	{
		return $"ShiftId: {ShiftId}, Date: {Date}, Period: {Period}, Manager: {Manager.Name}, Employees: [{string.Join(", ", _employees.Select(e => e.Name))}]";
	}
}
