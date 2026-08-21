# BusinessCalendar.NET

Business-day and settlement date math for .NET: working-day add or subtract, T+n settlement, and day-count fractions (ACT/360, ACT/365, 30/360). Bring your own holidays. Zero dependencies.

Every trading, invoicing, or payroll system eventually needs to answer questions like "what's 2 business days after this trade?" or "what fraction of a year is between these two coupon dates?" These sound simple until you hit the edge cases: a settlement date that lands on a public holiday, a 30/360 calculation that straddles the 29th of February, a Modified Following convention that would roll into next month. Getting these wrong produces off-by-one settlement dates and silently wrong interest accruals. There is no small, dependency-free .NET package that owns just this layer: you either pull in a full quant library, hand-roll it (and get the ISDA month-end rule wrong), or copy a StackOverflow answer that doesn't handle 30E/360 correctly. BusinessCalendar.NET is that missing layer: a `DateOnly`-based calendar and a set of day-count conventions, verified against QuantLib and Microsoft Excel's documented WORKDAY/NETWORKDAYS behavior, with zero runtime dependencies.

This library does not ship holiday data for any country or exchange. That is a separate, much larger problem with its own maintenance burden (see the `PublicHoliday` package on NuGet if you need bank holiday calendars for specific countries). BusinessCalendar.NET takes your holiday list and turns it into working, tested date arithmetic.

## Install

```
dotnet add package BusinessCalendar.Net
```

## Quickstart

```csharp
using BusinessCalendar;

var usHolidays = new[]
{
    new DateOnly(2026, 1, 1),   // New Year's Day
    new DateOnly(2026, 7, 4),   // Independence Day
    new DateOnly(2026, 11, 26), // Thanksgiving
};

var calendar = new BusinessDayCalendar(usHolidays);

calendar.IsBusinessDay(new DateOnly(2026, 7, 4));      // false, holiday
calendar.NextBusinessDay(new DateOnly(2026, 11, 25));  // 2026-11-27, skips Thanksgiving
calendar.AddBusinessDays(new DateOnly(2026, 8, 7), 3);  // steps 3 working days forward
```

## T+2 settlement

```csharp
using BusinessCalendar;

var calendar = new BusinessDayCalendar(usHolidays);
var tradeDate = new DateOnly(2026, 8, 10); // Monday

var settlementDate = calendar.Settle(tradeDate, 2); // T+2, standard US equities settlement
// 2026-08-12
```

## Rolling a coupon date and computing accrued interest

```csharp
using BusinessCalendar;

var calendar = new BusinessDayCalendar(usHolidays);
var scheduledCoupon = new DateOnly(2026, 10, 31); // falls on a Saturday

// ISDA Modified Following: roll forward unless that crosses into the next
// month, in which case roll backward instead.
var paymentDate = calendar.Adjust(scheduledCoupon, BusinessDayConvention.ModifiedFollowing);
// 2026-10-30, the last business day of October

var lastCoupon = new DateOnly(2026, 4, 30);
var dayCounter = DayCounters.Thirty360Us;
var accrualFraction = dayCounter.YearFraction(lastCoupon, paymentDate);
```

## What is in the box

**Calendar operations**, all on `System.DateOnly`:

| Member | Purpose |
|---|---|
| `IsBusinessDay(date)` | Not a weekend day and not a holiday |
| `IsWeekend(date)` / `IsHoliday(date)` | The two components of a business day check |
| `NextBusinessDay(date)` | Earliest business day strictly after `date` |
| `PreviousBusinessDay(date)` | Latest business day strictly before `date` |
| `AddBusinessDays(date, n)` | Step `n` business days forward or backward, matching Excel's `WORKDAY` |
| `BusinessDaysBetween(start, end)` | Count business days inclusive of both ends, matching Excel's `NETWORKDAYS` |
| `Settle(tradeDate, n)` | T+n settlement date |
| `Adjust(date, convention)` | Roll onto a business day under a `BusinessDayConvention` |

**Business-day conventions** (`BusinessDayConvention`), matching the 2006 ISDA Definitions section 4.11: `Following`, `ModifiedFollowing`, `Preceding`, `ModifiedPreceding`, `Unadjusted`.

**Day-count conventions** (`DayCounters`, an `IDayCounter` per convention), calendar-agnostic and independent of `BusinessDayCalendar`:

| Convention | Use case |
|---|---|
| `Actual360` | Money markets, short-term lending |
| `Actual365Fixed` | GBP fixed income, some loan markets |
| `ActualActualIsda` | Government bonds, ISDA swap confirmations |
| `Thirty360Us` | US corporate and municipal bonds (30/360 US/NASD, with the end-of-February rule; see [Correctness](#correctness) for how this differs from ISDA "30/360, Bond Basis") |
| `ThirtyE360Eurobond` | Eurobonds, European fixed income |

```csharp
var fraction = DayCounters.Get(DayCountConvention.Actual365Fixed)
    .YearFraction(new DateOnly(2026, 1, 15), new DateOnly(2026, 8, 7));
```

**Weekend rules** (`WeekendRule`): `SaturdaySunday` (default) and `FridaySaturday`, or supply your own set of `DayOfWeek` values, for example a 24/7 market with no weekend at all.

## Bring your own holidays

`BusinessDayCalendar` takes an `IEnumerable<DateOnly>` of holiday dates in its constructor. Wire it up to whatever source fits your application:

```csharp
using BusinessCalendar;
// using PublicHoliday; // a separate package that owns holiday data

IEnumerable<DateOnly> holidays = LoadHolidaysFromWherever();
var calendar = new BusinessDayCalendar(holidays, WeekendRule.FridaySaturday);
```

## Correctness

- `AddBusinessDays` and `BusinessDaysBetween` are verified against Microsoft's official worked examples for `WORKDAY` and `NETWORKDAYS`.
- `ActualActualIsda`, `Thirty360Us`, and `ThirtyE360Eurobond` are verified against the QuantLib test suite's day-count fixtures (`test-suite/daycounters.cpp`). The February month-end edge cases (the 28th vs. 29th vs. treating it as the 30th) are asserted exactly, not approximately.
- `Thirty360Us` implements the 30/360 US (NASD) convention, matching QuantLib's `Thirty360::USA`: the end-of-February rule applies unconditionally, not only for end-of-month investments. It is **not** the same as the ISDA 2006 Definitions section 4.16(f) "30/360, Bond Basis" convention, which has no end-of-February special case at all - a period from 2007-02-28 to 2007-08-31 is 183 days under pure ISDA Bond Basis but 180 days under `Thirty360Us`. If you need literal ISDA Bond Basis semantics, do not use this convention.
- `ModifiedFollowing` and `ModifiedPreceding` are tested against constructed month-boundary scenarios (a month ending on a Saturday, a month starting on a Sunday) that specifically exercise the roll-back and roll-forward branches.

## Zero dependencies, AOT-friendly

No runtime NuGet dependencies. No reflection, no dynamic code generation, no `DateTime`/timezone handling to worry about since everything operates on `DateOnly`. The library trims and compiles cleanly with Native AOT.

## License

MIT. See [LICENSE](LICENSE).
