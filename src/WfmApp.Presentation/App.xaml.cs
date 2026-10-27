using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using WfmApp.Presentation.Api;
using WfmApp.Presentation.ViewModels;
using WfmApp.Presentation.Views;

namespace WfmApp.Presentation;

public partial class App : global::System.Windows.Application
{
    private ServiceProvider? _serviceProvider;
    private IServiceScope? _serviceScope;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();

        services.AddHttpClient<IWfmApiClient, WfmApiClient>(client =>
            client.BaseAddress = new Uri(
                Environment.GetEnvironmentVariable("WFM_API_URL") ?? "http://localhost:5080/"));

        services.AddScoped<ShiftDashboardViewModel>();
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
