using WfmApp.Presentation.Domain.Common;

namespace WfmApp.Presentation.Application.UseCases.CreateShift;

public sealed class CreateShiftCommand
{
    public required EmployeeId EmployeeId { get; init; }
    public required DateTimeOffset Start { get; init; }
    public required DateTimeOffset End { get; init; }
}