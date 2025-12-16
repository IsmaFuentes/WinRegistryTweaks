using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace WinRegistryTweaks
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    private IServiceProvider? _serviceProvider;

    protected override void OnStartup(StartupEventArgs e)
    {
      var serviceCollection = new ServiceCollection();
      configureServices(serviceCollection);

      _serviceProvider = serviceCollection.BuildServiceProvider();
      _serviceProvider.GetRequiredService<MainWindow>().Show();
    }

    private void configureServices(IServiceCollection services)
    {
      services.AddSingleton<Services.IWindowsRegistryKeyService, Services.WindowsRegistryKeyService>();
      services.AddTransient<Services.IDialogService, Services.DialogService>();
      services.AddSingleton<ViewModels.MainViewModel>();
      services.AddSingleton<MainWindow>();
    }

    protected override void OnExit(ExitEventArgs e)
    {
      if (_serviceProvider is IDisposable disposable)
      {
        disposable.Dispose();
      }

      base.OnExit(e);
    }
  }
}
