using BusinessCalendar;

namespace BusinessCalendar.Tests;

public class WeekendRuleTests
{
    [Theory]
    [InlineData(DayOfWeek.Saturday, true)]
    [InlineData(DayOfWeek.Sunday, true)]
    [InlineData(DayOfWeek.Monday, false)]
    [InlineData(DayOfWeek.Tuesday, false)]
    [InlineData(DayOfWeek.Wednesday, false)]
    [InlineData(DayOfWeek.Thursday, false)]
    [InlineData(DayOfWeek.Friday, false)]
    public void SaturdaySunday_IsWeekend_MatchesDefaultConvention(DayOfWeek dayOfWeek, bool expected)
    {
        Assert.Equal(expected, WeekendRule.SaturdaySunday.IsWeekend(dayOfWeek));
    }

    [Theory]
    [InlineData(DayOfWeek.Friday, true)]
    [InlineData(DayOfWeek.Saturday, true)]
    [InlineData(DayOfWeek.Sunday, false)]
    [InlineData(DayOfWeek.Monday, false)]
    public void FridaySaturday_IsWeekend_MatchesMiddleEastConvention(DayOfWeek dayOfWeek, bool expected)
    {
        Assert.Equal(expected, WeekendRule.FridaySaturday.IsWeekend(dayOfWeek));
    }

    [Fact]
    public void Constructor_NullDays_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new WeekendRule(null!));
    }

    [Fact]
    public void Constructor_EmptySet_MeansNoWeekendDays()
    {
        var rule = new WeekendRule();

        foreach (DayOfWeek day in Enum.GetValues<DayOfWeek>())
        {
            Assert.False(rule.IsWeekend(day));
        }
    }

    [Fact]
    public void Constructor_DuplicateDays_AreDeduplicated()
    {
        var rule = new WeekendRule(DayOfWeek.Sunday, DayOfWeek.Sunday, DayOfWeek.Sunday);

        Assert.Single(rule.Days);
        Assert.Contains(DayOfWeek.Sunday, rule.Days);
    }

    [Fact]
    public void Constructor_AllSevenDays_Throws()
    {
        var allDays = Enum.GetValues<DayOfWeek>();

        var exception = Assert.Throws<ArgumentException>(() => new WeekendRule(allDays));
        Assert.Equal("weekendDays", exception.ParamName);
    }

    [Fact]
    public void Days_ReflectsConstructorArguments()
    {
        var rule = new WeekendRule(DayOfWeek.Friday, DayOfWeek.Saturday);

        Assert.Equal(2, rule.Days.Count);
        Assert.Contains(DayOfWeek.Friday, rule.Days);
        Assert.Contains(DayOfWeek.Saturday, rule.Days);
    }
}
