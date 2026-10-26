using WfmApp.Application.Common;
using WfmApp.Application.Interfaces;

namespace WfmApp.Application.UseCases.CheckOut;

public sealed class CheckOutUseCase
{
    private readonly IShiftRepository _shifts;
    private readonly IUnitOfWork _unitOfWork;

    public CheckOutUseCase(IShiftRepository shifts, IUnitOfWork unitOfWork)
    {
        _shifts = shifts;
        _unitOfWork = unitOfWork;
    }

    public async Task<CheckOutResult> ExecuteAsync(CheckOutCommand command, CancellationToken ct = default)
    {
        var shift = await _shifts.GetByIdAsync(command.ShiftId, ct);

        if (shift is null)
            throw new EntityNotFoundException($"Shift {command.ShiftId.Value} was not found.");

        shift.CheckOut(command.OccurredAt);

        await _unitOfWork.SaveChangesAsync(ct);

        var workedMinutes = (int)Math.Round(shift.WorkedDuration.TotalMinutes, MidpointRounding.AwayFromZero);
        var breakMinutes = (int)Math.Round(
            (shift.Planned.Break?.Duration ?? TimeSpan.Zero).TotalMinutes,
            MidpointRounding.AwayFromZero);

        if (command.UnitsProduced is { } units && workedMinutes > 0)
        {
            var unitsPerHour = units / (workedMinutes / 60.0m);

            return new CheckOutResult(shift.Id, workedMinutes, breakMinutes, unitsPerHour, "uph");
        }

        return new CheckOutResult(shift.Id, workedMinutes, breakMinutes, null, "hours");
    }
}
