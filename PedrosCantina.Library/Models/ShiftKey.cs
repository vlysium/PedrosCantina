namespace PedrosCantina.Library.Models;

public readonly record struct ShiftKey
{
	/// <summary>
	/// Gets or sets the date of the shift.
	/// </summary>
	public DateOnly Date { get; init; }

	/// <summary>
	/// Gets or sets the period of the shift.
	/// </summary>
	public string Period { get; init; }

	/// <summary>
	/// Initializes a new instance of the <see cref="ShiftKey"/> struct with the specified date and period.
	/// </summary>
	/// <param name="date">The date of the shift.</param>
	/// <param name="period">The period of the shift.</param>
	public ShiftKey(DateOnly date, ShiftPeriod period)
	{
		Date = date;
		Period = period.Code;
	}

	public override string ToString()
	{
		return $"{Date:yyyy-MM-dd} - {Period}";
	}
}
