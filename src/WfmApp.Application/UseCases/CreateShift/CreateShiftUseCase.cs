using WfmApp.Application.Common;
using WfmApp.Application.Interfaces;
using WfmApp.Domain.Common;
using WfmApp.Domain.Entities;

namespace WfmApp.Application.UseCases.CreateShift;

public sealed class CreateShiftUseCase
{
    private readonly IEmployeeRepository _employees;
    private readonly IShiftRepository _shifts;
    private readonly IShiftOverlapChecker _overlapChecker;
    private readonly IUnitOfWork _unitOfWork;

    public CreateShiftUseCase(
        IEmployeeRepository employees,
        IShiftRepository shifts,
        IShiftOverlapChecker overlapChecker,
        IUnitOfWork unitOfWork)
    {
        _employees = employees;
        _shifts = shifts;
        _overlapChecker = overlapChecker;
        _unitOfWork = unitOfWork;
    }

    public async Task<WarehouseShiftId> ExecuteAsync(CreateShiftCommand command, CancellationToken ct = default)
    {
        var employee = await _employees.GetByIdAsync(command.EmployeeId, ct);

        if (employee is null)
            throw new EntityNotFoundException($"Employee {command.EmployeeId.Value} was not found.");

        var interval = ShiftIntervalFactory.Build(
            command.Start, command.End, command.BreakStart, command.BreakDuration);

        if (await _overlapChecker.HasOverlapAsync(employee.Id, interval, null, ct))
            throw new InvalidOperationException("The employee already has a shift overlapping this interval.");

        var shift = WarehouseShift.Create(employee.Id, employee.HubId, command.Zone, interval);

        await _shifts.AddAsync(shift, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return shift.Id;
    }
}
