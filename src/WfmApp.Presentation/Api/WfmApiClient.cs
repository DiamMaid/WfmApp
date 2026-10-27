using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using WfmApp.Contracts;

namespace WfmApp.Presentation.Api;

public sealed class WfmApiClient : IWfmApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    private readonly HttpClient _http;

    public WfmApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<IReadOnlyList<HubDto>> GetHubsAsync(CancellationToken ct)
    {
        return await GetListAsync<HubDto>("api/v1/hubs", ct);
    }

    public async Task<IReadOnlyList<EmployeeDto>> GetEmployeesAsync(Guid hubId, CancellationToken ct)
    {
        return await GetListAsync<EmployeeDto>($"api/v1/employees?hubId={hubId}", ct);
    }

    public async Task<IReadOnlyList<ShiftDto>> GetShiftsAsync(Guid hubId, DateTimeOffset date, CancellationToken ct)
    {
        var url = $"api/v1/hubs/{hubId}/shifts?date={Uri.EscapeDataString(date.ToString("o"))}";
        return await GetListAsync<ShiftDto>(url, ct);
    }

    public async Task<ShiftDto> CreateShiftAsync(CreateShiftRequest request, CancellationToken ct)
    {
        return await PostAsync<CreateShiftRequest, ShiftDto>("api/v1/shifts", request, ct);
    }

    public async Task<ShiftDto> CheckInAsync(CheckInRequest request, CancellationToken ct)
    {
        return await PostAsync<CheckInRequest, ShiftDto>("api/v1/shifts/check-in", request, ct);
    }

    public async Task<CheckoutResultDto> CheckOutAsync(CheckOutRequest request, CancellationToken ct)
    {
        return await PostAsync<CheckOutRequest, CheckoutResultDto>("api/v1/shifts/check-out", request, ct);
    }

    private async Task<IReadOnlyList<T>> GetListAsync<T>(string url, CancellationToken ct)
    {
        using var response = await _http.GetAsync(url, ct);

        if (!response.IsSuccessStatusCode)
            throw await ToFailureAsync(response, ct);

        return await response.Content.ReadFromJsonAsync<List<T>>(JsonOptions, ct) ?? new List<T>();
    }

    private async Task<TOut> PostAsync<TIn, TOut>(string url, TIn body, CancellationToken ct)
    {
        using var response = await _http.PostAsJsonAsync(url, body, JsonOptions, ct);

        if (!response.IsSuccessStatusCode)
            throw await ToFailureAsync(response, ct);

        return (await response.Content.ReadFromJsonAsync<TOut>(JsonOptions, ct))!;
    }

    private static async Task<Exception> ToFailureAsync(HttpResponseMessage response, CancellationToken ct)
    {
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetail>(JsonOptions, ct);

        return new InvalidOperationException(problem?.Detail ?? response.ReasonPhrase ?? "Запрос к API завершился ошибкой.");
    }

    private sealed record ProblemDetail(string? Title, string? Detail, int? Status);
}
