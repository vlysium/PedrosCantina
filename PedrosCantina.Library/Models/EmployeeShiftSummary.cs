namespace PedrosCantina.Library.Models;

public class EmployeeShiftSummary : Employee
{
	/// <summary>
	/// Gets or sets the number of shifts worked by the employee.
	/// </summary>
	public int ShiftsWorked { get; set; }

	public EmployeeShiftSummary(int employeeId, string name, string email, string phoneNumber, int shiftsWorked): base(employeeId, name, email, phoneNumber)
	{
		ShiftsWorked = shiftsWorked;
	}

	public override string ToString()
	{
		return $"{base.ToString()}, ShiftsWorked: {ShiftsWorked}";
	}
}
