namespace PedrosCantina.Library.Models;

public class ShiftPeriod
{
	/// <summary>
	/// Gets or sets the unique identifier for the shift period.
	/// </summary>
	public string Period { get; set; }

	/// <summary>
	/// Gets or sets the start time of the shift period.
	/// </summary>
	public TimeOnly StartTime { get; set; }

	/// <summary>
	/// Gets or sets the end time of the shift period.
	/// </summary>
	public TimeOnly EndTime { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="ShiftPeriod"/> class.
	/// </summary>
	/// <param name="period">The period of the shift.</param>
	/// <param name="startTime">The start time of the shift.</param>
	/// <param name="endTime">The end time of the shift.</param>
	public ShiftPeriod(string period, TimeOnly startTime, TimeOnly endTime)
	{
		Period = period;
		StartTime = startTime;
		EndTime = endTime;
	}

	public override string ToString()
	{
		return $"{Period}: {StartTime} - {EndTime}";
	}
}
