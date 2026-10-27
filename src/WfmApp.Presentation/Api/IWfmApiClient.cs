using WfmApp.Contracts;

namespace WfmApp.Presentation.Api;

public interface IWfmApiClient
{
    Task<IReadOnlyList<HubDto>> GetHubsAsync(CancellationToken ct);
    Task<IReadOnlyList<EmployeeDto>> GetEmployeesAsync(Guid hubId, CancellationToken ct);
    Task<IReadOnlyList<ShiftDto>> GetShiftsAsync(Guid hubId, DateTimeOffset date, CancellationToken ct);
    Task<ShiftDto> CreateShiftAsync(CreateShiftRequest request, CancellationToken ct);
    Task<ShiftDto> CheckInAsync(CheckInRequest request, CancellationToken ct);
    Task<CheckoutResultDto> CheckOutAsync(CheckOutRequest request, CancellationToken ct);
}
