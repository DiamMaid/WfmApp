using Microsoft.AspNetCore.Mvc;
using WfmApp.Api.Contracts;
using WfmApp.Application.Common;
using WfmApp.Application.Interfaces;
using WfmApp.Application.UseCases.CheckIn;
using WfmApp.Application.UseCases.CheckOut;
using WfmApp.Application.UseCases.ConfirmShift;
using WfmApp.Application.UseCases.CreateShift;
using WfmApp.Application.UseCases.RescheduleShift;
using WfmApp.Domain.Common;
using WfmApp.Domain.Entities;

namespace WfmApp.Api.Controllers;

[ApiController]
[Route("api/v1/shifts")]
public sealed class ShiftsController : ControllerBase
{
    private readonly CreateShiftUseCase _createShift;
    private readonly RescheduleShiftUseCase _rescheduleShift;
    private readonly ConfirmShiftUseCase _confirmShift;
    private readonly CheckInUseCase _checkIn;
    private readonly CheckOutUseCase _checkOut;
    private readonly IShiftRepository _shifts;

    public ShiftsController(
        CreateShiftUseCase createShift,
        RescheduleShiftUseCase rescheduleShift,
        ConfirmShiftUseCase confirmShift,
        CheckInUseCase checkIn,
        CheckOutUseCase checkOut,
        IShiftRepository shifts)
    {
        _createShift = createShift;
        _rescheduleShift = rescheduleShift;
        _confirmShift = confirmShift;
        _checkIn = checkIn;
        _checkOut = checkOut;
        _shifts = shifts;
    }

    [HttpPost]
    public async Task<ActionResult<ShiftDto>> Create(
        [FromBody] CreateShiftRequest request,
        CancellationToken ct)
    {
        var command = new CreateShiftCommand
        {
            EmployeeId = new EmployeeId(request.EmployeeId),
            Zone = request.Zone,
            Start = request.Start,
            End = request.End,
            BreakStart = request.BreakStart,
            BreakDuration = ToDuration(request.BreakMinutes)
        };

        var id = await _createShift.ExecuteAsync(command, ct);

        return Created($"/api/v1/shifts/{id.Value}", ToDto(await LoadAsync(id, ct)));
    }

    [HttpGet("{shiftId:guid}")]
    public async Task<ActionResult<ShiftDto>> Get(Guid shiftId, CancellationToken ct)
    {
        return Ok(ToDto(await LoadAsync(new WarehouseShiftId(shiftId), ct)));
    }

    [HttpPost("check-in")]
    public async Task<ActionResult<ShiftDto>> CheckIn(
        [FromBody] CheckInRequest request,
        CancellationToken ct)
    {
        var command = new CheckInCommand
        {
            EmployeeId = new EmployeeId(request.EmployeeId),
            HubId = new HubId(request.HubId),
            OccurredAt = request.OccurredAt,
            AccessControlEventId = request.AccessControlEventId
        };

        var id = await _checkIn.ExecuteAsync(command, ct);

        return Ok(ToDto(await LoadAsync(id, ct)));
    }

    [HttpPost("check-out")]
    public async Task<ActionResult<CheckoutResultDto>> CheckOut(
        [FromBody] CheckOutRequest request,
        CancellationToken ct)
    {
        var command = new CheckOutCommand
        {
            ShiftId = new WarehouseShiftId(request.ShiftId),
            OccurredAt = request.OccurredAt,
            UnitsProduced = request.UnitsProduced
        };

        var result = await _checkOut.ExecuteAsync(command, ct);

        return Ok(new CheckoutResultDto(
            result.ShiftId.Value,
            result.WorkedMinutes,
            result.BreakMinutes,
            result.UnitsPerHour,
            result.OutputBasis));
    }

    [HttpPatch("{shiftId:guid}/schedule")]
    public async Task<ActionResult<ShiftDto>> Reschedule(
        Guid shiftId,
        [FromBody] RescheduleShiftRequest request,
        CancellationToken ct)
    {
        var command = new RescheduleShiftCommand
        {
            ShiftId = new WarehouseShiftId(shiftId),
            Start = request.Start,
            End = request.End,
            BreakStart = request.BreakStart,
            BreakDuration = ToDuration(request.BreakMinutes)
        };

        await _rescheduleShift.ExecuteAsync(command, ct);

        return Ok(ToDto(await LoadAsync(command.ShiftId, ct)));
    }

    [HttpPost("{shiftId:guid}/confirm")]
    public async Task<ActionResult<ShiftDto>> Confirm(Guid shiftId, CancellationToken ct)
    {
        var command = new ConfirmShiftCommand { ShiftId = new WarehouseShiftId(shiftId) };

        await _confirmShift.ExecuteAsync(command, ct);

        return Ok(ToDto(await LoadAsync(command.ShiftId, ct)));
    }

    private async Task<WarehouseShift> LoadAsync(WarehouseShiftId id, CancellationToken ct)
    {
        var shift = await _shifts.GetByIdAsync(id, ct);

        if (shift is null)
            throw new EntityNotFoundException($"Shift {id.Value} was not found.");

        return shift;
    }

    private static TimeSpan? ToDuration(int? minutes) =>
        minutes is { } value ? TimeSpan.FromMinutes(value) : null;

    private static ShiftDto ToDto(WarehouseShift shift) => new(
        shift.Id.Value,
        shift.EmployeeId.Value,
        shift.HubId.Value,
        shift.Zone,
        shift.Planned.Start,
        shift.Planned.End,
        shift.Planned.Break?.Start,
        shift.Planned.Break is { } shiftBreak
            ? (int)Math.Round(shiftBreak.Duration.TotalMinutes, MidpointRounding.AwayFromZero)
            : null,
        shift.CheckedInAt,
        shift.CheckedOutAt,
        shift.Status,
        shift.IsConfirmed,
        (int)Math.Round(shift.WorkedDuration.TotalMinutes, MidpointRounding.AwayFromZero));
}
