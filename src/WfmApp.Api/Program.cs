using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using WfmApp.Api.ErrorHandling;
using WfmApp.Api.Seed;
using WfmApp.Application.Interfaces;
using WfmApp.Application.UseCases.CheckIn;
using WfmApp.Application.UseCases.CheckOut;
using WfmApp.Application.UseCases.ConfirmShift;
using WfmApp.Application.UseCases.CreateShift;
using WfmApp.Application.UseCases.RescheduleShift;
using WfmApp.Infrastructure.Persistence;
using WfmApp.Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<WfmDbContext>(options =>
    options.UseNpgsql(WfmDatabase.ConnectionString));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IHubRepository, HubRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IShiftRepository, ShiftRepository>();
builder.Services.AddScoped<IShiftOverlapChecker, ShiftOverlapChecker>();

builder.Services.AddScoped<CreateShiftUseCase>();
builder.Services.AddScoped<RescheduleShiftUseCase>();
builder.Services.AddScoped<ConfirmShiftUseCase>();
builder.Services.AddScoped<CheckInUseCase>();
builder.Services.AddScoped<CheckOutUseCase>();

builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)));

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<DomainExceptionHandler>();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<WfmDbContext>();
    await context.Database.MigrateAsync();
    await WfmSeeder.SeedAsync(context);
}

app.Run();
