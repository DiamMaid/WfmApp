using WfmApp.Application.Common;
using WfmApp.Application.Interfaces;

namespace WfmApp.Application.UseCases.ConfirmShift;

public sealed class ConfirmShiftUseCase
{
    private readonly IShiftRepository _shifts;
    private readonly IUnitOfWork _unitOfWork;

    public ConfirmShiftUseCase(IShiftRepository shifts, IUnitOfWork unitOfWork)
    {
        _shifts = shifts;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(ConfirmShiftCommand command, CancellationToken ct = default)
    {
        var shift = await _shifts.GetByIdAsync(command.ShiftId, ct);

        if (shift is null)
            throw new EntityNotFoundException($"Shift {command.ShiftId.Value} was not found.");

        shift.Confirm();

        await _unitOfWork.SaveChangesAsync(ct);
    }
}
