using PedrosCantina.Library.Models;
using PedrosCantina.Library.Repositories;

namespace PedrosCantina.Library.Services;

public class ShiftService
{
	/// <summary>
	/// The shift repository used to perform CRUD operations on shifts.
	/// </summary>
	private readonly ShiftRepository _shiftRepository;

	/// <summary>
	/// Initializes a new instance of the <see cref="ShiftService"/> class with the specified shift repository.
	/// </summary>
	/// <param name="shiftRepository">The shift repository to use for CRUD operations.</param>
	public ShiftService(ShiftRepository shiftRepository)
	{
		_shiftRepository = shiftRepository;
	}

	/// <summary>
	/// Gets a shift by its unique identifier.
	/// </summary>
	/// <param name="key">The unique identifier of the shift to get.</param>
	/// <returns>The shift if found, otherwise null.</returns>
	public Shift? GetShiftById(ShiftKey key)
	{
		return _shiftRepository.ReadById(key);
	}

	/// <summary>
	/// Gets all shifts from the shift repository.
	/// </summary>
	/// <returns>A list of all shifts in the database.</returns>
	public List<Shift> GetAllShifts()
	{
		return _shiftRepository.ReadAll();
	}

	/// <summary>
	/// Adds a new shift to the shift repository.
	/// </summary>
	/// <param name="shift">The shift to add.</param>
	/// <returns>The added shift.</returns>
	public Shift AddShift(Shift shift)
	{
		return _shiftRepository.Create(shift);
	}

	/// <summary>
	/// Updates an existing shift in the shift repository.
	/// </summary>
	/// <param name="shift">The shift to update.</param>
	public void UpdateShift(Shift shift)
	{
		_shiftRepository.Update(shift);
	}

	/// <summary>
	/// Deletes a shift from the shift repository by its unique identifier.
	/// </summary>
	/// <param name="key">The unique identifier of the shift to delete.</param>
	public void DeleteShift(ShiftKey key)
	{
		_shiftRepository.Delete(key);
	}

	/// <summary>
	/// Gets the monthly plan for a specific month and year.
	/// </summary>
	/// <param name="month">The month for which to get the plan.</param>
	/// <param name="year">The year for which to get the plan.</param>
	/// <returns>A list of shifts for the specified month and year sorted by date and start time.</returns>
	public List<Shift> GetMonthlyPlan(int month, int year)
	{
		return _shiftRepository.ReadByMonth(month, year);
	}

	/// <summary>
	/// Gets the number of shifts worked by a specific employee in a given month and year.
	/// </summary>
	/// <param name="date">The year and month for which to get the number of shifts.</param>
	/// <param name="employeeId">The ID of the employee for whom to get the number of shifts.</param>
	/// <returns>The employee shift summary for the specified employee in the given month and year.</returns>
	public EmployeeShiftSummary GetMonthlyShiftsByEmployeeId(DateOnly date, int employeeId)
	{
		return _shiftRepository.ReadMonthlyShiftsByEmployeeId(date, employeeId);
	}

	/// <summary>
	/// Gets the distribution of shifts worked by all employees in a given year.
	/// </summary>
	/// <param name="date">The year for which to get the distribution of shifts.</param>
	/// <returns>A list of employee shift summaries for the specified year ordered by the number of shifts worked in descending order.</returns>
	public List<EmployeeShiftSummary> GetEmployeeShiftDistributionByYear(DateOnly date)
	{
		return _shiftRepository.ReadEmployeeShiftDistributionByYear(date);
	}
}
