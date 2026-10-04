using PedrosCantina.Library.Models;
using PedrosCantina.Library.Repositories;

namespace PedrosCantina.Library.Services;

public class ShiftPeriodService
{
	/// <summary>
	/// The shift period repository used to perform read operations on shift periods.
	/// </summary>
	private readonly IReadOperations<ShiftPeriod, string> _shiftPeriodRepository;

	/// <summary>
	/// Initializes a new instance of the <see cref="ShiftPeriodService"/> class with the specified shift period repository.
	/// </summary>
	/// <param name="shiftPeriodRepository">The shift period repository to use for read operations.</param>
	public ShiftPeriodService(IReadOperations<ShiftPeriod, string> shiftPeriodRepository)
	{
		_shiftPeriodRepository = shiftPeriodRepository;
	}

	/// <summary>
	/// Gets a shift period by its unique identifier.
	/// </summary>
	/// <param name="id">The unique identifier of the shift period to retrieve.</param>
	/// <returns>The shift period if found, otherwise null.</returns>
	public ShiftPeriod? GetShiftPeriodById(string id)
	{
		return _shiftPeriodRepository.ReadById(id);
	}

	/// <summary>
	/// Gets all shift periods from the shift period repository.
	/// </summary>
	/// <returns>A list of all shift periods.</returns>
	public List<ShiftPeriod> GetAllShiftPeriods()
	{
		return _shiftPeriodRepository.ReadAll();
	}
}
