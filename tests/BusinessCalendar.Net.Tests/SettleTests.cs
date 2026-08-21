using BusinessCalendar;

namespace BusinessCalendar.Tests;

public class SettleTests
{
    [Fact]
    public void Settle_TPlusTwo_MatchesUsEquitiesConvention()
    {
        var calendar = new BusinessDayCalendar(Array.Empty<DateOnly>());
        var tradeMonday = new DateOnly(2026, 8, 10);

        var settlement = calendar.Settle(tradeMonday, 2);

        Assert.Equal(new DateOnly(2026, 8, 12), settlement);
    }

    [Fact]
    public void Settle_TPlusZero_ReturnsTradeDateUnchanged()
    {
        var calendar = new BusinessDayCalendar(Array.Empty<DateOnly>());
        var tradeDate = new DateOnly(2026, 8, 10);

        Assert.Equal(tradeDate, calendar.Settle(tradeDate, 0));
    }

    [Fact]
    public void Settle_CrossesWeekendAndHoliday()
    {
        var holiday = new DateOnly(2026, 8, 11);
        var calendar = new BusinessDayCalendar(new[] { holiday });
        var tradeFriday = new DateOnly(2026, 8, 7);

        var settlement = calendar.Settle(tradeFriday, 2);

        Assert.Equal(new DateOnly(2026, 8, 12), settlement);
    }

    [Fact]
    public void Settle_NegativeSettlementDays_Throws()
    {
        var calendar = new BusinessDayCalendar(Array.Empty<DateOnly>());

        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => calendar.Settle(new DateOnly(2026, 8, 10), -1));

        Assert.Equal("settlementDays", exception.ParamName);
    }

    [Fact]
    public void Settle_MatchesAddBusinessDays()
    {
        var calendar = new BusinessDayCalendar(Array.Empty<DateOnly>());
        var tradeDate = new DateOnly(2026, 8, 10);

        Assert.Equal(calendar.AddBusinessDays(tradeDate, 3), calendar.Settle(tradeDate, 3));
    }
}
