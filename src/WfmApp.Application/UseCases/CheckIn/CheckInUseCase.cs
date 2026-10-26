using WfmApp.Application.Common;
using WfmApp.Application.Interfaces;
using WfmApp.Domain.Common;

namespace WfmApp.Application.UseCases.CheckIn;

public sealed class CheckInUseCase
{
    private readonly IEmployeeRepository _employees;
    private readonly IShiftRepository _shifts;
    private readonly IUnitOfWork _unitOfWork;

    public CheckInUseCase(
        IEmployeeRepository employees,
        IShiftRepository shifts,
        IUnitOfWork unitOfWork)
    {
        _employees = employees;
        _shifts = shifts;
        _unitOfWork = unitOfWork;
    }

    public async Task<WarehouseShiftId> ExecuteAsync(CheckInCommand command, CancellationToken ct = default)
    {
        var replayed = await _shifts.GetByAccessControlEventIdAsync(command.AccessControlEventId, ct);

        if (replayed is not null)
            return replayed.Id;

        var employee = await _employees.GetByIdAsync(command.EmployeeId, ct);

        if (employee is null)
            throw new EntityNotFoundException($"Employee {command.EmployeeId.Value} was not found.");

        if (employee.HubId != command.HubId)
            throw new InvalidOperationException("The employee is not assigned to the requested hub.");

        var activeShift = await _shifts.GetActiveByEmployeeIdAsync(command.EmployeeId, ct);

        if (activeShift is not null)
            throw new InvalidOperationException("The employee already has an active shift.");

        var shift = await _shifts.GetPlannedCoveringAsync(command.EmployeeId, command.OccurredAt, ct);

        if (shift is null)
            throw new EntityNotFoundException("No planned shift covers the requested check-in time.");

        shift.CheckIn(command.OccurredAt, command.AccessControlEventId);

        await _unitOfWork.SaveChangesAsync(ct);

        return shift.Id;
    }
}
