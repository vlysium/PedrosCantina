using PedrosCantina.Library.Models;
using PedrosCantina.Library.Repositories;

namespace PedrosCantina.Library.Services;

public class ShiftService
{
	/// <summary>
	/// The shift repository used to perform CRUD operations on shifts.
	/// </summary>
	private readonly ICrudOperations<Shift, int> _shiftRepository;

	/// <summary>
	/// Initializes a new instance of the <see cref="ShiftService"/> class with the specified shift repository.
	/// </summary>
	/// <param name="shiftRepository">The shift repository to use for CRUD operations.</param>
	public ShiftService(ICrudOperations<Shift, int> shiftRepository)
	{
		_shiftRepository = shiftRepository;
	}

	/// <summary>
	/// Gets a shift by its unique identifier.
	/// </summary>
	/// <param name="id">The unique identifier of the shift to get.</param>
	/// <returns>The shift if found, otherwise null.</returns>
	public Shift? GetShiftById(int id)
	{
		return _shiftRepository.ReadById(id);
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
	/// <param name="id">The unique identifier of the shift to delete.</param>
	public void DeleteShift(int id)
	{
		_shiftRepository.Delete(id);
	}
}
