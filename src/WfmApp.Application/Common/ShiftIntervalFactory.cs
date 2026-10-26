using WfmApp.Domain.ValueObjects;

namespace WfmApp.Application.Common;

public static class ShiftIntervalFactory
{
    public static ShiftInterval Build(
        DateTimeOffset start,
        DateTimeOffset end,
        DateTimeOffset? breakStart,
        TimeSpan? breakDuration)
    {
        if (breakStart is null && breakDuration is null)
            return ShiftInterval.Create(start, end, null);

        if (breakStart is null)
            throw new ArgumentException("Break start is required when a break duration is supplied.", nameof(breakStart));

        if (breakDuration is null)
            throw new ArgumentException("Break duration is required when a break start is supplied.", nameof(breakDuration));

        return ShiftInterval.Create(start, end, ShiftBreak.Create(breakStart.Value, breakDuration.Value));
    }
}
