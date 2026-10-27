using Microsoft.AspNetCore.Mvc;
using WfmApp.Contracts;
using WfmApp.Application.Interfaces;
using WfmApp.Domain.Common;

namespace WfmApp.Api.Controllers;

[ApiController]
[Route("api/v1/employees")]
public sealed class EmployeesController : ControllerBase
{
    private readonly IEmployeeRepository _employees;

    public EmployeesController(IEmployeeRepository employees)
    {
        _employees = employees;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EmployeeDto>>> GetByHub(
        [FromQuery] Guid hubId,
        CancellationToken ct)
    {
        var employees = await _employees.GetByHubAsync(new HubId(hubId), ct);

        var result = employees
            .Select(e => new EmployeeDto(
                e.Id.Value,
                e.StaffId,
                e.FullName,
                e.HubId.Value,
                e.Role,
                e.EmploymentType,
                e.Status))
            .ToList();

        return Ok(result);
    }
}
