namespace BusinessCalendar;

/// <summary>
/// Provides the built-in <see cref="IDayCounter"/> implementations and a
/// factory to resolve one from a <see cref="DayCountConvention"/>.
/// </summary>
public static class DayCounters
{
    /// <summary>
    /// Gets the Actual/360 day counter.
    /// </summary>
    public static IDayCounter Actual360 { get; } = new Actual360DayCounter();

    /// <summary>
    /// Gets the Actual/365 Fixed day counter.
    /// </summary>
    public static IDayCounter Actual365Fixed { get; } = new Actual365FixedDayCounter();

    /// <summary>
    /// Gets the Actual/Actual (ISDA) day counter.
    /// </summary>
    public static IDayCounter ActualActualIsda { get; } = new ActualActualIsdaDayCounter();

    /// <summary>
    /// Gets the 30/360 US (NASD) day counter. See
    /// <see cref="DayCountConvention.Thirty360Us"/> for how this differs
    /// from the ISDA "30/360, Bond Basis" convention.
    /// </summary>
    public static IDayCounter Thirty360Us { get; } = new Thirty360UsDayCounter();

    /// <summary>
    /// Gets the 30E/360 (Eurobond Basis) day counter.
    /// </summary>
    public static IDayCounter ThirtyE360Eurobond { get; } = new ThirtyE360DayCounter();

    /// <summary>
    /// Resolves the <see cref="IDayCounter"/> for a given
    /// <see cref="DayCountConvention"/>.
    /// </summary>
    /// <param name="convention">The convention to resolve.</param>
    /// <returns>The day counter implementing <paramref name="convention"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="convention"/> is not a recognized value.
    /// </exception>
    public static IDayCounter Get(DayCountConvention convention) => convention switch
    {
        DayCountConvention.Actual360 => Actual360,
        DayCountConvention.Actual365Fixed => Actual365Fixed,
        DayCountConvention.ActualActualIsda => ActualActualIsda,
        DayCountConvention.Thirty360Us => Thirty360Us,
        DayCountConvention.ThirtyE360Eurobond => ThirtyE360Eurobond,
        _ => throw new ArgumentOutOfRangeException(nameof(convention), convention, "Unsupported day count convention."),
    };

    /// <summary>
    /// Returns whether <paramref name="date"/> is the last calendar day of
    /// February. Used unconditionally by <see cref="Thirty360UsDayCounter"/>
    /// to match QuantLib's <c>Thirty360::USA</c>; some descriptions of
    /// 30/360 US instead apply this rule only when the underlying
    /// investment is end-of-month, which this library does not model.
    /// </summary>
    private static bool IsLastDayOfFebruary(DateOnly date) =>
        date.Month == FebruaryMonth && date.Day == DateTime.DaysInMonth(date.Year, FebruaryMonth);

    private const int FebruaryMonth = 2;
    private const int DaysPerThirtyDayYear = 360;
    private const int DaysPerThirtyDayMonth = 30;
    private const int MaxDayOfMonth = 31;

    private sealed class Actual360DayCounter : IDayCounter
    {
        private const double DaysPerYear = 360.0;

        public string Name => "Actual/360";

        public int DayCount(DateOnly start, DateOnly end) => end.DayNumber - start.DayNumber;

        public double YearFraction(DateOnly start, DateOnly end) => DayCount(start, end) / DaysPerYear;
    }

    private sealed class Actual365FixedDayCounter : IDayCounter
    {
        private const double DaysPerYear = 365.0;

        public string Name => "Actual/365 (Fixed)";

        public int DayCount(DateOnly start, DateOnly end) => end.DayNumber - start.DayNumber;

        public double YearFraction(DateOnly start, DateOnly end) => DayCount(start, end) / DaysPerYear;
    }

    private sealed class ActualActualIsdaDayCounter : IDayCounter
    {
        private const int DaysPerLeapYear = 366;
        private const int DaysPerCommonYear = 365;

        public string Name => "Actual/Actual (ISDA)";

        public int DayCount(DateOnly start, DateOnly end) => end.DayNumber - start.DayNumber;

        public double YearFraction(DateOnly start, DateOnly end)
        {
            if (start == end)
            {
                return 0.0;
            }

            if (start > end)
            {
                return -YearFraction(end, start);
            }

            var startYear = start.Year;
            var endYear = end.Year;

            var startOfYearAfterStart = new DateOnly(startYear + 1, 1, 1);
            var startOfEndYear = new DateOnly(endYear, 1, 1);

            var daysInStartYear = DaysInYear(startYear);
            var daysInEndYear = DaysInYear(endYear);

            var sum = (double)(endYear - startYear - 1);
            sum += (startOfYearAfterStart.DayNumber - start.DayNumber) / (double)daysInStartYear;
            sum += (end.DayNumber - startOfEndYear.DayNumber) / (double)daysInEndYear;
            return sum;
        }

        private static int DaysInYear(int year) => DateTime.IsLeapYear(year) ? DaysPerLeapYear : DaysPerCommonYear;
    }

    private sealed class Thirty360UsDayCounter : IDayCounter
    {
        public string Name => "30/360 (US)";

        public int DayCount(DateOnly start, DateOnly end)
        {
            var d1 = start.Day;
            var d2 = end.Day;

            if (IsLastDayOfFebruary(start))
            {
                if (IsLastDayOfFebruary(end))
                {
                    d2 = DaysPerThirtyDayMonth;
                }

                d1 = DaysPerThirtyDayMonth;
            }

            if (d2 == MaxDayOfMonth && d1 >= DaysPerThirtyDayMonth)
            {
                d2 = DaysPerThirtyDayMonth;
            }

            if (d1 == MaxDayOfMonth)
            {
                d1 = DaysPerThirtyDayMonth;
            }

            return (DaysPerThirtyDayYear * (end.Year - start.Year))
                + (DaysPerThirtyDayMonth * (end.Month - start.Month))
                + (d2 - d1);
        }

        public double YearFraction(DateOnly start, DateOnly end) => DayCount(start, end) / (double)DaysPerThirtyDayYear;
    }

    private sealed class ThirtyE360DayCounter : IDayCounter
    {
        public string Name => "30E/360 (Eurobond Basis)";

        public int DayCount(DateOnly start, DateOnly end)
        {
            var d1 = start.Day == MaxDayOfMonth ? DaysPerThirtyDayMonth : start.Day;
            var d2 = end.Day == MaxDayOfMonth ? DaysPerThirtyDayMonth : end.Day;

            return (DaysPerThirtyDayYear * (end.Year - start.Year))
                + (DaysPerThirtyDayMonth * (end.Month - start.Month))
                + (d2 - d1);
        }

        public double YearFraction(DateOnly start, DateOnly end) => DayCount(start, end) / (double)DaysPerThirtyDayYear;
    }
}
