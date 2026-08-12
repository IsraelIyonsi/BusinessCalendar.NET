using BusinessCalendar;

namespace BusinessCalendar.Tests;

/// <summary>
/// Oracle values are the ISDA-convention test vectors from the QuantLib test
/// suite, <c>test-suite/daycounters.cpp</c>, function <c>testActualActual</c>,
/// https://github.com/lballabio/QuantLib/blob/master/test-suite/daycounters.cpp.
/// Each case is annotated below with the scenario name used in that source.
/// </summary>
public class ActualActualIsdaDayCounterTests
{
    private const double Tolerance = 1.0e-10;

    private static readonly IDayCounter DayCounter = DayCounters.ActualActualIsda;

    public static IEnumerable<object[]> QuantLibOracleCases()
    {
        // first example
        yield return new object[] { new DateOnly(2003, 11, 1), new DateOnly(2004, 5, 1), 0.497724380567 };
        // short first calculation period (first period)
        yield return new object[] { new DateOnly(1999, 2, 1), new DateOnly(1999, 7, 1), 0.410958904110 };
        // short first calculation period (second period)
        yield return new object[] { new DateOnly(1999, 7, 1), new DateOnly(2000, 7, 1), 1.001377348600 };
        // long first calculation period (first period)
        yield return new object[] { new DateOnly(2002, 8, 15), new DateOnly(2003, 7, 15), 0.915068493151 };
        // long first calculation period (second period)
        yield return new object[] { new DateOnly(2003, 7, 15), new DateOnly(2004, 1, 15), 0.504004790778 };
        // short final calculation period (penultimate period)
        yield return new object[] { new DateOnly(1999, 7, 30), new DateOnly(2000, 1, 30), 0.503892506924 };
        // short final calculation period (final period)
        yield return new object[] { new DateOnly(2000, 1, 30), new DateOnly(2000, 6, 30), 0.415300546448 };
    }

    [Theory]
    [MemberData(nameof(QuantLibOracleCases))]
    public void YearFraction_MatchesQuantLibOracle(DateOnly start, DateOnly end, double expected)
    {
        var result = DayCounter.YearFraction(start, end);

        Assert.True(
            Math.Abs(result - expected) < Tolerance,
            $"Expected {expected} but got {result} for {start:yyyy-MM-dd} to {end:yyyy-MM-dd}.");
    }

    [Fact]
    public void YearFraction_PeriodWithinSingleCommonYear_EqualsActualOverThreeHundredSixtyFive()
    {
        var start = new DateOnly(2026, 1, 1);
        var end = new DateOnly(2026, 7, 1);

        var result = DayCounter.YearFraction(start, end);

        Assert.Equal((end.DayNumber - start.DayNumber) / 365.0, result, precision: 12);
    }

    [Fact]
    public void YearFraction_PeriodWithinSingleLeapYear_EqualsActualOverThreeHundredSixtySix()
    {
        var start = new DateOnly(2024, 1, 1);
        var end = new DateOnly(2024, 7, 1);

        var result = DayCounter.YearFraction(start, end);

        Assert.Equal((end.DayNumber - start.DayNumber) / 366.0, result, precision: 12);
    }

    [Fact]
    public void Name_IsActualActualIsda()
    {
        Assert.Equal("Actual/Actual (ISDA)", DayCounter.Name);
    }
}
