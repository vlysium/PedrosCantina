using Microsoft.Data.SqlClient;
using PedrosCantina.Library.Models;

namespace PedrosCantina.Library.Repositories;

public class ShiftPeriodRepository : IReadOperations<ShiftPeriod, string>
{
	/// <summary>
	/// The database worker used to interact with the database.
	/// </summary>
	private readonly DBWorker _dbWorker;
	
	/// <summary>
	/// Initializes a new instance of the <see cref="ShiftPeriodRepository"/> class with the specified database worker.
	/// </summary>
	/// <param name="dbWorker">The database worker to use.</param>
	public ShiftPeriodRepository(DBWorker dbWorker)
	{
		_dbWorker = dbWorker;
	}

	/// <summary>
	/// Reads a shift period by its ID.
	/// </summary>
	/// <param name="period">The ID of the shift period to read.</param>
	/// <returns>The shift period if found, otherwise null.</returns>
	public ShiftPeriod? ReadById(string period)
	{
		const string query = """
			SELECT period, start_time, end_time
			FROM shift_periods
			WHERE period = @period;
		""";

		// Release the connection after use with `using`
		using SqlConnection connection = _dbWorker.Connect();

		using SqlCommand command = new SqlCommand(query, connection);
		command.Parameters.AddWithValue("@period", period);

		using SqlDataReader reader = command.ExecuteReader();

		if (!reader.Read())
		{
			return null;
		}

		return new ShiftPeriod(
			code: reader.GetString(reader.GetOrdinal("period")),
			startTime: TimeOnly.FromTimeSpan(reader.GetTimeSpan(reader.GetOrdinal("start_time"))),
			endTime: TimeOnly.FromTimeSpan(reader.GetTimeSpan(reader.GetOrdinal("end_time")))
		);
	}

	/// <summary>
	/// Reads all shift periods from the database.
	/// </summary>
	/// <returns>A list of all shift periods.</returns>
	public List<ShiftPeriod> ReadAll()
	{
		const string query = """
			SELECT period, start_time, end_time
			FROM shift_periods;
		""";

		// Release the connection after use with `using`
		using SqlConnection connection = _dbWorker.Connect();

		using SqlCommand command = new SqlCommand(query, connection);
		using SqlDataReader reader = command.ExecuteReader();

		List<ShiftPeriod> shiftPeriods = new List<ShiftPeriod>();

		while (reader.Read())
		{
			ShiftPeriod shiftPeriod = new ShiftPeriod(
				code: reader.GetString(reader.GetOrdinal("period")),
				startTime: TimeOnly.FromTimeSpan(reader.GetTimeSpan(reader.GetOrdinal("start_time"))),
				endTime: TimeOnly.FromTimeSpan(reader.GetTimeSpan(reader.GetOrdinal("end_time")))
			);

			shiftPeriods.Add(shiftPeriod);
		}

		return shiftPeriods;
	}
}
