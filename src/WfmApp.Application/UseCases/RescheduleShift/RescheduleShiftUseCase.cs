using WfmApp.Application.Common;
using WfmApp.Application.Interfaces;

namespace WfmApp.Application.UseCases.RescheduleShift;

public sealed class RescheduleShiftUseCase
{
    private readonly IShiftRepository _shifts;
    private readonly IShiftOverlapChecker _overlapChecker;
    private readonly IUnitOfWork _unitOfWork;

    public RescheduleShiftUseCase(
        IShiftRepository shifts,
        IShiftOverlapChecker overlapChecker,
        IUnitOfWork unitOfWork)
    {
        _shifts = shifts;
        _overlapChecker = overlapChecker;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(RescheduleShiftCommand command, CancellationToken ct = default)
    {
        var shift = await _shifts.GetByIdAsync(command.ShiftId, ct);

        if (shift is null)
            throw new EntityNotFoundException($"Shift {command.ShiftId.Value} was not found.");

        var interval = ShiftIntervalFactory.Build(
            command.Start, command.End, command.BreakStart, command.BreakDuration);

        if (await _overlapChecker.HasOverlapAsync(shift.EmployeeId, interval, shift.Id, ct))
            throw new InvalidOperationException("The employee already has another shift overlapping this interval.");

        shift.Reschedule(interval);

        await _unitOfWork.SaveChangesAsync(ct);
    }
}
