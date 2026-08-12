# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.1.0] - 2026-08-12

### Added

- `BusinessCalendar` built from a `WeekendRule` and a caller-supplied holiday set: `IsBusinessDay`, `IsWeekend`, `IsHoliday`, `NextBusinessDay`, `PreviousBusinessDay`, `AddBusinessDays`, `BusinessDaysBetween`, `Settle` (T+n settlement), and `Adjust`, all operating on `System.DateOnly`.
- `WeekendRule` with `SaturdaySunday` (default) and `FridaySaturday` presets, plus support for any custom set of weekend days.
- `BusinessDayConvention` enum and `BusinessCalendar.Adjust`: `Following`, `ModifiedFollowing`, `Preceding`, `ModifiedPreceding`, `Unadjusted`, matching the 2006 ISDA Definitions section 4.11.
- `IDayCounter` and the `DayCounters` factory with five built-in conventions: `Actual360`, `Actual365Fixed`, `ActualActualIsda`, `Thirty360Us`, and `ThirtyE360Eurobond`.
- `AddBusinessDays` and `BusinessDaysBetween` verified against Microsoft's official worked examples for Excel's `WORKDAY` and `NETWORKDAYS` functions.
- `ActualActualIsda`, `Thirty360Us`, and `ThirtyE360Eurobond` verified against the QuantLib test suite's day-count fixtures, including the end-of-February edge cases. `Thirty360Us` implements 30/360 US (NASD), matching QuantLib's `Thirty360::USA`; it is distinct from the ISDA "30/360, Bond Basis" convention, which has no end-of-February special case.
- `ModifiedFollowing` and `ModifiedPreceding` verified against month-boundary scenarios that exercise the roll-back and roll-forward branches.
- Zero runtime dependencies; operates entirely on `DateOnly` with no timezone handling. `IsAotCompatible`/`IsTrimmable` enabled and verified by the AOT/trim analyzers on every build.
