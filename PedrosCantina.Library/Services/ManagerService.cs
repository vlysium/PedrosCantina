using PedrosCantina.Library.Models;
using PedrosCantina.Library.Repositories;

namespace PedrosCantina.Library.Services;

public class ManagerService
{
	/// <summary>
	/// The manager repository used to perform CRUD operations on managers.
	/// </summary>
	private readonly ICrudOperations<Manager> _managerRepository;

	/// <summary>
	/// Initializes a new instance of the <see cref="ManagerService"/> class with the specified manager repository.
	/// </summary>
	/// <param name="managerRepository">The manager repository used to perform CRUD operations on managers.</param>
	public ManagerService(ManagerRepository managerRepository)
	{
		_managerRepository = managerRepository;
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
	/// Demotes a manager to a regular employee.
	/// </summary>
	/// <param name="id">The ID of the manager to demote.</param>
	/// <returns>The demoted manager, now a regular employee.</returns>
	public Employee DemoteManager(int id)
	{
		return _managerRepository.Delete(id);
	}
}
