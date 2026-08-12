using BusinessCalendar;

namespace BusinessCalendar.Tests;

/// <summary>
/// Oracle values are the Eurobond-basis test vectors from the QuantLib test
/// suite, <c>test-suite/daycounters.cpp</c>, function
/// <c>testThirty360_EurobondBasis</c>,
/// https://github.com/lballabio/QuantLib/blob/master/test-suite/daycounters.cpp,
/// citing https://www.isda.org/2008/12/22/30-360-day-count-conventions/.
/// Unlike the US convention, 30E/360 rolls the 31st of any month back to the
/// 30th unconditionally and applies no special end-of-February rule.
/// </summary>
public class ThirtyE360EurobondDayCounterTests
{
    private static readonly IDayCounter DayCounter = DayCounters.ThirtyE360Eurobond;

    public static IEnumerable<object[]> QuantLibOracleCases()
    {
        // Example 1: end dates do not involve the last day of February.
        yield return Case(2006, 8, 20, 2007, 2, 20, 180);
        yield return Case(2007, 2, 20, 2007, 8, 20, 180);
        yield return Case(2007, 8, 20, 2008, 2, 20, 180);
        yield return Case(2008, 2, 20, 2008, 8, 20, 180);
        yield return Case(2008, 8, 20, 2009, 2, 20, 180);
        yield return Case(2009, 2, 20, 2009, 8, 20, 180);

        // Example 2: end dates include some end-of-February dates.
        yield return Case(2006, 2, 28, 2006, 8, 31, 182);
        yield return Case(2006, 8, 31, 2007, 2, 28, 178);
        yield return Case(2007, 2, 28, 2007, 8, 31, 182);
        yield return Case(2007, 8, 31, 2008, 2, 29, 179);
        yield return Case(2008, 2, 29, 2008, 8, 31, 181);
        yield return Case(2008, 8, 31, 2009, 2, 28, 178);
        yield return Case(2009, 2, 28, 2009, 8, 31, 182);
        yield return Case(2009, 8, 31, 2010, 2, 28, 178);
        yield return Case(2010, 2, 28, 2010, 8, 31, 182);
        yield return Case(2010, 8, 31, 2011, 2, 28, 178);
        yield return Case(2011, 2, 28, 2011, 8, 31, 182);
        yield return Case(2011, 8, 31, 2012, 2, 29, 179);

        // Example 3: miscellaneous calculations.
        yield return Case(2006, 1, 31, 2006, 2, 28, 28);
        yield return Case(2006, 1, 30, 2006, 2, 28, 28);
        yield return Case(2006, 2, 28, 2006, 3, 3, 5);
        yield return Case(2006, 2, 14, 2006, 2, 28, 14);
        yield return Case(2006, 9, 30, 2006, 10, 31, 30);
        yield return Case(2006, 10, 31, 2006, 11, 28, 28);
        yield return Case(2007, 8, 31, 2008, 2, 28, 178);
        yield return Case(2008, 2, 28, 2008, 8, 28, 180);
        yield return Case(2008, 2, 28, 2008, 8, 30, 182);
        yield return Case(2008, 2, 28, 2008, 8, 31, 182);
        yield return Case(2007, 2, 26, 2008, 2, 28, 362);
        yield return Case(2007, 2, 26, 2008, 2, 29, 363);
        yield return Case(2008, 2, 29, 2009, 2, 28, 359);
        yield return Case(2008, 2, 28, 2008, 3, 30, 32);
        yield return Case(2008, 2, 28, 2008, 3, 31, 32);
    }

    [Theory]
    [MemberData(nameof(QuantLibOracleCases))]
    public void DayCount_MatchesQuantLibOracle(DateOnly start, DateOnly end, int expected)
    {
        Assert.Equal(expected, DayCounter.DayCount(start, end));
    }

    [Theory]
    [MemberData(nameof(QuantLibOracleCases))]
    public void YearFraction_MatchesExpectedDayCountOverThreeHundredSixty(DateOnly start, DateOnly end, int expectedDays)
    {
        Assert.Equal(expectedDays / 360.0, DayCounter.YearFraction(start, end), precision: 12);
    }

    [Fact]
    public void Name_IsThirtyE360Eurobond()
    {
        Assert.Equal("30E/360 (Eurobond Basis)", DayCounter.Name);
    }

    private static object[] Case(int y1, int m1, int d1, int y2, int m2, int d2, int expectedDays) =>
        new object[] { new DateOnly(y1, m1, d1), new DateOnly(y2, m2, d2), expectedDays };
}
