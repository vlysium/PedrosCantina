namespace PedrosCantina.Library.Models;

public class Manager : Employee
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Manager"/> class.
	/// </summary>
	public Manager() { }

	/// <summary>
	/// Initializes a new instance of the <see cref="Manager"/> class with the specified properties.
	/// </summary>
	/// <param name="name">The name of the manager.</param>
	/// <param name="email">The email of the manager.</param>
	/// <param name="phoneNumber">The phone number of the manager.</param>
	public Manager(string name, string email, string phoneNumber) : base(name, email, phoneNumber) {}

	/// <summary>
	/// Initializes a new instance of the <see cref="Manager"/> class with the specified properties, including the manager ID.
	/// </summary>
	/// <param name="employeeId">The employee ID of the manager.</param>
	/// <param name="name">The name of the manager.</param>
	/// <param name="email">The email of the manager.</param>
	/// <param name="phoneNumber">The phone number of the manager.</param>
	public Manager(int employeeId, string name, string email, string phoneNumber) : base(employeeId, name, email, phoneNumber) { }

	public override string ToString()
	{
		return $"ManagerId: {EmployeeId}, Name: {Name}, Email: {Email}, PhoneNumber: {PhoneNumber}";
	}
}
