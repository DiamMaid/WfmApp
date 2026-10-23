using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WfmApp.Presentation.Application.Interfaces;
using WfmApp.Presentation.Application.UseCases.CreateShift;
using WfmApp.Presentation.Infrastructure.Persistence;
using WfmApp.Presentation.Infrastructure.Persistence.Repositories;
using WfmApp.Presentation.ViewModels;

namespace WfmApp.Presentation;

public partial class App : global::System.Windows.Application
{
    private ServiceProvider? _serviceProvider;
    private IServiceScope? _serviceScope;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();
        services.AddDbContext<WfmDbContext>(options =>
            options.UseNpgsql(WfmDatabase.ConnectionString));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IShiftRepository, ShiftRepository>();
        services.AddScoped<CreateShiftUseCase>();
        services.AddScoped<ShiftPlanningViewModel>();
        services.AddScoped<MainWindow>();

        _serviceProvider = services.BuildServiceProvider();

        _serviceScope = _serviceProvider.CreateScope();
        var window = _serviceScope.ServiceProvider.GetRequiredService<MainWindow>();
        window.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serviceScope?.Dispose();
        _serviceProvider?.Dispose();
        base.OnExit(e);
    }
}