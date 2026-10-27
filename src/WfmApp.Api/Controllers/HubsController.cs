using Microsoft.AspNetCore.Mvc;
using WfmApp.Contracts;
using WfmApp.Application.Common;
using WfmApp.Application.Interfaces;
using WfmApp.Domain.Common;

namespace WfmApp.Api.Controllers;

[ApiController]
[Route("api/v1/hubs")]
public sealed class HubsController : ControllerBase
{
    private readonly IHubRepository _hubs;
    private readonly IEmployeeRepository _employees;
    private readonly IShiftRepository _shifts;

    public HubsController(IHubRepository hubs, IEmployeeRepository employees, IShiftRepository shifts)
    {
        _hubs = hubs;
        _employees = employees;
        _shifts = shifts;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<HubDto>>> GetHubs(
        [FromQuery] string? city,
        CancellationToken ct)
    {
        var hubs = await _hubs.GetActiveAsync(ct);

        var result = hubs
            .Where(h => city is null || string.Equals(h.Location.City, city, StringComparison.OrdinalIgnoreCase))
            .Select(h => new HubDto(h.Id.Value, h.Code, h.Name, h.Location.City, h.Location.Site, h.TimeZone, h.IsActive))
            .ToList();

        return Ok(result);
    }

    [HttpGet("{hubId:guid}/shifts/active")]
    public async Task<ActionResult<IReadOnlyList<ActiveShiftDto>>> GetActiveShifts(
        Guid hubId,
        CancellationToken ct)
    {
        var hub = await _hubs.GetByIdAsync(new HubId(hubId), ct);

        if (hub is null)
            throw new EntityNotFoundException($"Hub {hubId} was not found.");

        var now = DateTimeOffset.UtcNow;
        var shifts = await _shifts.GetActiveByHubAsync(hub.Id, now, ct);
        var employees = await _employees.GetByHubAsync(hub.Id, ct);

        var employeesById = employees.ToDictionary(e => e.Id);
        var result = new List<ActiveShiftDto>();

        foreach (var shift in shifts)
        {
            if (!employeesById.TryGetValue(shift.EmployeeId, out var employee))
                continue;

            var startedAt = shift.CheckedInAt ?? shift.Planned.Start;
            var elapsed = (int)Math.Round((now - startedAt).TotalMinutes, MidpointRounding.AwayFromZero);

            result.Add(new ActiveShiftDto(
                shift.Id.Value,
                employee.Id.Value,
                employee.StaffId,
                employee.FullName,
                employee.Role,
                employee.EmploymentType,
                shift.Zone,
                startedAt,
                shift.Planned.End,
                elapsed));
        }

        return Ok(result);
    }
}
