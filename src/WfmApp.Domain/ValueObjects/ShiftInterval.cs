namespace WfmApp.Domain.ValueObjects;

public sealed record ShiftInterval
{
    private static readonly TimeSpan MaxElapsed = TimeSpan.FromHours(12);
    private static readonly TimeSpan BreakRequiredThreshold = TimeSpan.FromHours(4);
    private static readonly TimeSpan MinBreakDuration = TimeSpan.FromMinutes(45);

    private DateTimeOffset _start;
    private DateTimeOffset _end;
    private ShiftBreak? _break;

    private ShiftInterval()
    {
    }

    private ShiftInterval(DateTimeOffset start, DateTimeOffset end, ShiftBreak? shiftBreak)
    {
        _start = start;
        _end = end;
        _break = shiftBreak;
    }

    public DateTimeOffset Start => _start;
    public DateTimeOffset End => _end;
    public ShiftBreak? Break => _break;

    public TimeSpan Elapsed => _end - _start;
    public TimeSpan Worked => Elapsed - (_break?.Duration ?? TimeSpan.Zero);

    public static ShiftInterval Create(DateTimeOffset start, DateTimeOffset end, ShiftBreak? shiftBreak = null)
    {
        if (start == default)
            throw new ArgumentException("Start time must be set.", nameof(start));

        if (end == default)
            throw new ArgumentException("End time must be set.", nameof(end));

        if (start >= end)
            throw new ArgumentException("Start time must be before end time.", nameof(start));

        var elapsed = end - start;

        if (elapsed > MaxElapsed)
            throw new ArgumentException("Shift window must not exceed 12 hours.", nameof(end));

        if (elapsed > BreakRequiredThreshold && shiftBreak is null)
            throw new ArgumentException(
                $"A break of at least {MinBreakDuration.TotalMinutes:0} minutes is required for a shift longer than {BreakRequiredThreshold.TotalHours:0} hours.",
                nameof(shiftBreak));

        if (shiftBreak is not null)
        {
            if (shiftBreak.Duration < MinBreakDuration)
                throw new ArgumentException(
                    $"Break duration must be at least {MinBreakDuration.TotalMinutes:0} minutes.",
                    nameof(shiftBreak));

            if (shiftBreak.Start < start)
                throw new ArgumentException("Break must start no earlier than the shift start.", nameof(shiftBreak));

            if (shiftBreak.Start + shiftBreak.Duration > end)
                throw new ArgumentException("Break must end no later than the shift end.", nameof(shiftBreak));

            if (shiftBreak.Duration >= elapsed)
                throw new ArgumentException("Break must be shorter than the shift window.", nameof(shiftBreak));
        }

        return new ShiftInterval(start, end, shiftBreak);
    }
}
