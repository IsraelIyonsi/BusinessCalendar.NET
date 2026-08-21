namespace BusinessCalendar;

/// <summary>
/// A calendar of business days built from a <see cref="WeekendRule"/> and a
/// caller-supplied set of holidays.
/// </summary>
/// <remarks>
/// This library does not ship holiday data for any jurisdiction. Supply the
/// holidays your application needs, for example from a package dedicated to
/// holiday data, a database table, or a static list.
/// </remarks>
public sealed class BusinessDayCalendar
{
    private readonly HashSet<DateOnly> _holidays;

    /// <summary>
    /// Initializes a new instance of the <see cref="BusinessDayCalendar"/>
    /// class.
    /// </summary>
    /// <param name="holidays">
    /// The dates treated as holidays, in addition to weekend days. Duplicate
    /// dates are ignored.
    /// </param>
    /// <param name="weekendRule">
    /// The rule that determines which days of the week are weekend days.
    /// Defaults to <see cref="WeekendRule.SaturdaySunday"/> when omitted.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="holidays"/> is <see langword="null"/>.
    /// </exception>
    public BusinessDayCalendar(IEnumerable<DateOnly> holidays, WeekendRule? weekendRule = null)
    {
        ArgumentNullException.ThrowIfNull(holidays);

        _holidays = new HashSet<DateOnly>(holidays);
        WeekendRule = weekendRule ?? WeekendRule.SaturdaySunday;
    }

    /// <summary>
    /// Gets the weekend rule this calendar was built with.
    /// </summary>
    public WeekendRule WeekendRule { get; }

    /// <summary>
    /// Gets the set of holiday dates this calendar was built with.
    /// </summary>
    public IReadOnlySet<DateOnly> Holidays => _holidays;

    /// <summary>
    /// Determines whether a date falls on a weekend day under this
    /// calendar's <see cref="WeekendRule"/>.
    /// </summary>
    /// <param name="date">The date to test.</param>
    /// <returns><see langword="true"/> if <paramref name="date"/> is a weekend day.</returns>
    public bool IsWeekend(DateOnly date) => WeekendRule.IsWeekend(date.DayOfWeek);

    /// <summary>
    /// Determines whether a date is one of this calendar's holidays.
    /// </summary>
    /// <param name="date">The date to test.</param>
    /// <returns><see langword="true"/> if <paramref name="date"/> is a holiday.</returns>
    public bool IsHoliday(DateOnly date) => _holidays.Contains(date);

    /// <summary>
    /// Determines whether a date is a business day: neither a weekend day
    /// nor a holiday.
    /// </summary>
    /// <param name="date">The date to test.</param>
    /// <returns><see langword="true"/> if <paramref name="date"/> is a business day.</returns>
    public bool IsBusinessDay(DateOnly date) => !IsWeekend(date) && !IsHoliday(date);

    /// <summary>
    /// Finds the earliest business day strictly after the given date.
    /// </summary>
    /// <param name="date">The date to search forward from, exclusive.</param>
    /// <returns>The next business day after <paramref name="date"/>.</returns>
    public DateOnly NextBusinessDay(DateOnly date)
    {
        var candidate = date.AddDays(1);
        while (!IsBusinessDay(candidate))
        {
            candidate = candidate.AddDays(1);
        }

        return candidate;
    }

    /// <summary>
    /// Finds the latest business day strictly before the given date.
    /// </summary>
    /// <param name="date">The date to search backward from, exclusive.</param>
    /// <returns>The previous business day before <paramref name="date"/>.</returns>
    public DateOnly PreviousBusinessDay(DateOnly date)
    {
        var candidate = date.AddDays(-1);
        while (!IsBusinessDay(candidate))
        {
            candidate = candidate.AddDays(-1);
        }

        return candidate;
    }

    /// <summary>
    /// Moves a date forward or backward by a number of business days,
    /// without counting the starting date itself.
    /// </summary>
    /// <param name="date">The starting date. It need not be a business day.</param>
    /// <param name="businessDays">
    /// The number of business days to move. A positive value moves forward,
    /// a negative value moves backward, and zero returns
    /// <paramref name="date"/> unchanged.
    /// </param>
    /// <returns>
    /// The date reached after stepping <paramref name="businessDays"/>
    /// business days from <paramref name="date"/>.
    /// </returns>
    public DateOnly AddBusinessDays(DateOnly date, int businessDays)
    {
        if (businessDays == 0)
        {
            return date;
        }

        var step = businessDays > 0 ? 1 : -1;
        var remaining = Math.Abs(businessDays);
        var candidate = date;

        while (remaining > 0)
        {
            candidate = candidate.AddDays(step);
            if (IsBusinessDay(candidate))
            {
                remaining--;
            }
        }

        return candidate;
    }

    /// <summary>
    /// Counts the business days between two dates, inclusive of both
    /// endpoints when they are themselves business days.
    /// </summary>
    /// <param name="start">The start date.</param>
    /// <param name="end">The end date.</param>
    /// <returns>
    /// The count of business days from <paramref name="start"/> to
    /// <paramref name="end"/>, inclusive. The result is negative when
    /// <paramref name="end"/> precedes <paramref name="start"/>.
    /// </returns>
    public int BusinessDaysBetween(DateOnly start, DateOnly end)
    {
        if (start > end)
        {
            return -BusinessDaysBetween(end, start);
        }

        var count = 0;
        for (var date = start; date <= end; date = date.AddDays(1))
        {
            if (IsBusinessDay(date))
            {
                count++;
            }
        }

        return count;
    }

    /// <summary>
    /// Computes the T+n settlement date for a trade, as the date reached by
    /// stepping <paramref name="settlementDays"/> business days forward from
    /// the trade date.
    /// </summary>
    /// <param name="tradeDate">The trade date. It need not be a business day.</param>
    /// <param name="settlementDays">
    /// The settlement period in business days, for example 2 for T+2
    /// equities settlement. Must not be negative.
    /// </param>
    /// <returns>The settlement date.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="settlementDays"/> is negative.
    /// </exception>
    public DateOnly Settle(DateOnly tradeDate, int settlementDays)
    {
        if (settlementDays < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(settlementDays),
                settlementDays,
                "Settlement period cannot be negative.");
        }

        return AddBusinessDays(tradeDate, settlementDays);
    }

    /// <summary>
    /// Adjusts a date onto a business day using the given business-day
    /// convention.
    /// </summary>
    /// <param name="date">The date to adjust.</param>
    /// <param name="convention">The adjustment convention to apply.</param>
    /// <returns>The adjusted, always-business-day date.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="convention"/> is not a recognized value.
    /// </exception>
    public DateOnly Adjust(DateOnly date, BusinessDayConvention convention) => convention switch
    {
        BusinessDayConvention.Unadjusted => date,
        BusinessDayConvention.Following => IsBusinessDay(date) ? date : NextBusinessDay(date),
        BusinessDayConvention.Preceding => IsBusinessDay(date) ? date : PreviousBusinessDay(date),
        BusinessDayConvention.ModifiedFollowing => AdjustModifiedFollowing(date),
        BusinessDayConvention.ModifiedPreceding => AdjustModifiedPreceding(date),
        _ => throw new ArgumentOutOfRangeException(nameof(convention), convention, "Unsupported business day convention."),
    };

    private DateOnly AdjustModifiedFollowing(DateOnly date)
    {
        if (IsBusinessDay(date))
        {
            return date;
        }

        var following = NextBusinessDay(date);
        return IsSameCalendarMonth(following, date) ? following : PreviousBusinessDay(date);
    }

    private DateOnly AdjustModifiedPreceding(DateOnly date)
    {
        if (IsBusinessDay(date))
        {
            return date;
        }

        var preceding = PreviousBusinessDay(date);
        return IsSameCalendarMonth(preceding, date) ? preceding : NextBusinessDay(date);
    }

    private static bool IsSameCalendarMonth(DateOnly first, DateOnly second) =>
        first.Year == second.Year && first.Month == second.Month;
}
