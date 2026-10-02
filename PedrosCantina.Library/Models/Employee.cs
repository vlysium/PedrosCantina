namespace PedrosCantina.Library;

public class Employee
{
	/// <summary>
	/// Gets or sets the unique identifier for the employee.
	/// </summary>
	public int EmployeeId { get; set; }

	/// <summary>
	/// Gets or sets the name of the employee.
	/// </summary>
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the email address of the employee.
	/// </summary>
	public string Email { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the phone number of the employee.
	/// </summary>
	public string PhoneNumber { get; set; } = string.Empty;

	/// <summary>
	/// Initializes a new instance of the <see cref="Employee"/> class.
	/// </summary>
	public Employee() { }

	/// <summary>
	/// Initializes a new instance of the <see cref="Employee"/> class with the specified properties.
	/// </summary>
	/// <param name="name">The name of the employee.</param>
	/// <param name="email">The email address of the employee.</param>
	/// <param name="phoneNumber">The phone number of the employee.</param>
	public Employee(string name, string email, string phoneNumber): this()
	{
		Name = name;
		Email = email;
		PhoneNumber = phoneNumber;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Employee"/> class with the specified properties, including the employee ID.
	/// </summary>
	/// <param name="employeeId">The unique identifier for the employee.</param>
	/// <param name="name">The name of the employee.</param>
	/// <param name="email">The email address of the employee.</param>
	/// <param name="phoneNumber">The phone number of the employee.</param>
	public Employee(int employeeId, string name, string email, string phoneNumber): this(name, email, phoneNumber)
	{
		EmployeeId = employeeId;
	}

	public override string ToString()
	{
		return $"EmployeeId: {EmployeeId}, Name: {Name}, Email: {Email}, PhoneNumber: {PhoneNumber}";
	}
}
