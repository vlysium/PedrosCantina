using PedrosCantina.Library.Models;
using PedrosCantina.Library.Repositories;

namespace PedrosCantina.Library.Services;

public class EmployeeService
{
	/// <summary>
	/// The employee repository used to perform CRUD operations on employees.
	/// </summary>
	private readonly ICrudOperations<Employee> _employeeRepository;

	/// <summary>
	/// Initializes a new instance of the <see cref="EmployeeService"/> class with the specified employee repository and manager repository.
	/// </summary>
	/// <param name="employeeRepository">The employee repository used to perform CRUD operations on employees.</param>
	public EmployeeService(EmployeeRepository employeeRepository)
	{
		_employeeRepository = employeeRepository;
	}

	/// <summary>
	/// Gets an employee by their unique identifier.
	/// </summary>
	/// <param name="id">The unique identifier of the employee to get.</param>
	/// <returns>The employee if found, otherwise null.</returns>
	public Employee? GetEmployeeById(int id)
	{
		return _employeeRepository.ReadById(id);
	}

	/// <summary>
	/// Gets all employees from the employee repository.
	/// </summary>
	/// <returns>A list of all employees in the database.</returns>
	public List<Employee> GetAllEmployees()
	{
		return _employeeRepository.ReadAll();
	}

	/// <summary>
	/// Adds a new employee to the employee repository.
	/// </summary>
	/// <param name="employee">The employee to add.</param>
	public void AddEmployee(Employee employee)
	{
		_employeeRepository.Create(employee);
	}

	/// <summary>
	/// Updates an existing employee in the employee repository.
	/// </summary>
	/// <param name="employee">The employee to update.</param>
	public void UpdateEmployee(Employee employee)
	{
		_employeeRepository.Update(employee);
	}

	/// <summary>
	/// Deletes an employee from the employee repository by their unique identifier.
	/// </summary>
	/// <param name="id">The unique identifier of the employee to delete.</param>
	public void DeleteEmployee(int id)
	{
		_employeeRepository.Delete(id);
	}
}
