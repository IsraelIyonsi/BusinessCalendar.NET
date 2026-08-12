namespace BusinessCalendar;

/// <summary>
/// Defines which days of the week are treated as weekend (non-business) days.
/// </summary>
/// <remarks>
/// The default financial-market weekend is Saturday and Sunday. Markets that
/// observe a different weekend, such as Friday and Saturday in several Middle
/// Eastern and North African jurisdictions, can be modeled by supplying a
/// custom set of days.
/// </remarks>
public sealed class WeekendRule
{
    private const int DaysPerWeek = 7;

    private readonly HashSet<DayOfWeek> _weekendDays;

    /// <summary>
    /// Gets the conventional weekend of Saturday and Sunday.
    /// </summary>
    public static WeekendRule SaturdaySunday { get; } = new(DayOfWeek.Saturday, DayOfWeek.Sunday);

    /// <summary>
    /// Gets the Friday and Saturday weekend observed by several Middle
    /// Eastern and North African markets.
    /// </summary>
    public static WeekendRule FridaySaturday { get; } = new(DayOfWeek.Friday, DayOfWeek.Saturday);

    /// <summary>
    /// Initializes a new instance of the <see cref="WeekendRule"/> class with
    /// the given set of weekend days.
    /// </summary>
    /// <param name="weekendDays">
    /// The days of the week treated as weekend. Duplicates are ignored. An
    /// empty set is valid and means no day of the week is a weekend day.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="weekendDays"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Every day of the week was supplied, leaving no day that can ever be a
    /// business day.
    /// </exception>
    public WeekendRule(params DayOfWeek[] weekendDays)
    {
        ArgumentNullException.ThrowIfNull(weekendDays);

        _weekendDays = new HashSet<DayOfWeek>(weekendDays);

        if (_weekendDays.Count >= DaysPerWeek)
        {
            throw new ArgumentException(
                "A weekend rule cannot mark every day of the week as weekend; at least one day must remain a potential business day.",
                nameof(weekendDays));
        }
    }

    /// <summary>
    /// Gets the set of days of the week treated as weekend under this rule.
    /// </summary>
    public IReadOnlySet<DayOfWeek> Days => _weekendDays;

    /// <summary>
    /// Determines whether the given day of the week is a weekend day under
    /// this rule.
    /// </summary>
    /// <param name="dayOfWeek">The day of the week to test.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="dayOfWeek"/> is a weekend
    /// day; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsWeekend(DayOfWeek dayOfWeek) => _weekendDays.Contains(dayOfWeek);
}
