namespace BusinessCalendar;

/// <summary>
/// Computes the day count and year fraction between two dates under a
/// specific day-count convention.
/// </summary>
/// <remarks>
/// Day-count conventions are independent of any business calendar: they
/// operate purely on calendar dates and do not consider weekends or
/// holidays. Implement this interface to plug in a custom convention beyond
/// the ones supplied by <see cref="DayCounters"/>.
/// </remarks>
public interface IDayCounter
{
    /// <summary>
    /// Gets the human-readable name of the convention, for example
    /// <c>"Actual/360"</c>.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Computes the convention-specific day count between two dates.
    /// </summary>
    /// <param name="start">The start date of the period.</param>
    /// <param name="end">The end date of the period.</param>
    /// <returns>
    /// The number of days between <paramref name="start"/> and
    /// <paramref name="end"/> as defined by the convention. The result is
    /// negative when <paramref name="end"/> precedes <paramref name="start"/>.
    /// </returns>
    int DayCount(DateOnly start, DateOnly end);

    /// <summary>
    /// Computes the year fraction between two dates under the convention.
    /// </summary>
    /// <param name="start">The start date of the period.</param>
    /// <param name="end">The end date of the period.</param>
    /// <returns>
    /// The signed year fraction between <paramref name="start"/> and
    /// <paramref name="end"/>. The result is negative when
    /// <paramref name="end"/> precedes <paramref name="start"/>.
    /// </returns>
    double YearFraction(DateOnly start, DateOnly end);
}
