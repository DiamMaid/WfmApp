using WfmApp.Application.Interfaces;
using WfmApp.Domain.Common;
using WfmApp.Domain.Entities;
using WfmApp.Domain.ValueObjects;

namespace WfmApp.Application.UseCases.CreateShift;

public sealed class CreateShiftUseCase
{
    private readonly IShiftRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateShiftUseCase(IShiftRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<WorkShiftId> ExecuteAsync(CreateShiftCommand command, CancellationToken ct = default)
    {
        var interval = ShiftInterval.Create(command.Start, command.End);
        var workShift = WorkShift.Create(command.EmployeeId, interval);

        await _repository.AddAsync(workShift, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return workShift.Id;
    }
}