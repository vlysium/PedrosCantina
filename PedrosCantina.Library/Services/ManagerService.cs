using PedrosCantina.Library.Models;
using PedrosCantina.Library.Repositories;

namespace PedrosCantina.Library.Services;

public class ManagerService
{
	/// <summary>
	/// The manager repository used to perform CRUD operations on managers.
	/// </summary>
	private readonly ManagerRepository _managerRepository;

	/// <summary>
	/// Initializes a new instance of the <see cref="ManagerService"/> class with the specified manager repository.
	/// </summary>
	/// <param name="managerRepository">The manager repository used to perform CRUD operations on managers.</param>
	public ManagerService(ManagerRepository managerRepository)
	{
		_managerRepository = managerRepository;
	}

	/// <summary>
	/// Determines whether the specified employee is a manager.
	/// </summary>
	/// <param name="employeeId">The unique identifier of the employee to check.</param>
	/// <returns>True if the employee is a manager; otherwise, false.</returns>
	public bool isManager(int employeeId)
	{
		return _managerRepository.ReadById(employeeId) != null;
	}

	/// <summary>
	/// Retrieves all managers from the repository.
	/// </summary>
	/// <returns>A list of all managers.</returns>
	public List<Manager> GetAllManagers()
	{
		return _managerRepository.ReadAll();
	}

	/// <summary>
	/// Adds a new manager to the repository.
	/// </summary>
	/// <param name="manager">The manager to add.</param>
	public void AddManager(Manager manager)
	{
		_managerRepository.Create(manager);
	}

	/// <summary>
	/// Promotes an existing employee to a manager by inserting their ID into the managers table.
	/// This method does not create a new employee; it assumes the employee already exists in the employees table.
	/// </summary>
	/// <param name="manager">The manager to promote.</param>
	/// <exception cref="InvalidOperationException">Thrown when the employee is already a manager.</exception>
	public void PromoteToManager(Manager manager)
	{
		if (isManager(manager.EmployeeId))
		{
			throw new InvalidOperationException($"Employee with ID {manager.EmployeeId} is already a manager.");
		}

		_managerRepository.Update(manager);
	}

	/// <summary>
	/// Demotes a manager to a regular employee.
	/// </summary>
	/// <param name="id">The ID of the manager to demote.</param>
	public void DemoteManager(int id)
	{
		_managerRepository.Delete(id);
	}
}
