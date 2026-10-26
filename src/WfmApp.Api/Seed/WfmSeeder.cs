using Microsoft.EntityFrameworkCore;
using WfmApp.Domain.Entities;
using WfmApp.Domain.Enums;
using WfmApp.Domain.ValueObjects;
using WfmApp.Infrastructure.Persistence;

namespace WfmApp.Api.Seed;

public static class WfmSeeder
{
    public static async Task SeedAsync(WfmDbContext context)
    {
        if (await context.WarehouseHubs.AnyAsync())
            return;

        var moscow = WarehouseHub.Create(
            "MSK-HUB-01",
            "Хаб Москва",
            HubLocation.Create("Москва", "Новая Рига"),
            "Europe/Moscow");

        var spb = WarehouseHub.Create(
            "SPB-HUB-01",
            "Хаб Санкт-Петербург",
            HubLocation.Create("Санкт-Петербург", "Шушары"),
            "Europe/Moscow");

        await context.WarehouseHubs.AddRangeAsync(moscow, spb);

        var employees = new[]
        {
            WarehouseEmployee.Create("10001", "Иванов Пётр Сергеевич", moscow.Id, Role.Inbound, EmploymentType.Staff),
            WarehouseEmployee.Create("10002", "Смирнова Анна Игоревна", moscow.Id, Role.Picking, EmploymentType.Staff),
            WarehouseEmployee.Create("10003", "Кузнецов Олег Дмитриевич", moscow.Id, Role.Packing, EmploymentType.SelfEmployed),
            WarehouseEmployee.Create("20001", "Соколова Мария Павловна", spb.Id, Role.Outbound, EmploymentType.Staff)
        };

        await context.WarehouseEmployees.AddRangeAsync(employees);

        await context.SaveChangesAsync();
    }
}
