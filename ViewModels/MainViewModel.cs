using System;
using System.Diagnostics;
using System.Linq;
using WinRegistryTweaks.Services;

namespace WinRegistryTweaks.ViewModels
{
  public class MainViewModel : MVVM.Observable
  {
    private readonly IWindowsRegistryKeyService _registryKeyService;
    private readonly IDialogService _dialogService;

    public MainViewModel(IWindowsRegistryKeyService windowsRegistryKeyService, IDialogService dialogService) 
    {
      _dialogService = dialogService;
      _registryKeyService = windowsRegistryKeyService;
      
      // default key states
      _driverSearchingKeyEnabled = _registryKeyService.GetKeyState(WindowsRegistryKeyService.RKEY_DRIVER_SEARCH);
      _wuDriversInQualityUpdateKeyEnabled = _registryKeyService.GetKeyState(WindowsRegistryKeyService.RKEY_EXCLUDE_WU_DRIVERS);
      _webSearchKeyEnabled = _registryKeyService.GetKeyState(WindowsRegistryKeyService.RKEY_WEB_SEARCH);
      _fastStartupKeyEnabled = _registryKeyService.GetKeyState(WindowsRegistryKeyService.RKEY_FAST_BOOT);
      _contextMenuKeyEnabled = _registryKeyService.GetKeyState(WindowsRegistryKeyService.RKEY_CONTEXT_MENU);
    }

    private bool _driverSearchingKeyEnabled;
    public bool DriverSearchkingKeyEnabled
    {
      get => _driverSearchingKeyEnabled;
      set
      {
        _driverSearchingKeyEnabled = value;
        NotifyPropertyChanged(nameof(DriverSearchkingKeyEnabled));
        _registryKeyService.SetKeyState(WindowsRegistryKeyService.RKEY_DRIVER_SEARCH, value);
      }
    }

    private bool _wuDriversInQualityUpdateKeyEnabled;
    public bool WuDriversInQualityUpdateKeyEnabled
    {
      get => _wuDriversInQualityUpdateKeyEnabled;
      set
      {
        _wuDriversInQualityUpdateKeyEnabled = value;
        NotifyPropertyChanged(nameof(WuDriversInQualityUpdateKeyEnabled));
        _registryKeyService.SetKeyState(WindowsRegistryKeyService.RKEY_EXCLUDE_WU_DRIVERS, value);
      }
    }

    private bool _webSearchKeyEnabled;
    public bool WebSearchKeyEnabled
    {
      get => _webSearchKeyEnabled;
      set
      {
        _webSearchKeyEnabled = value;
        NotifyPropertyChanged(nameof(WebSearchKeyEnabled));
        _registryKeyService.SetKeyState(WindowsRegistryKeyService.RKEY_WEB_SEARCH, value);
      }
    }

    private bool _fastStartupKeyEnabled;
    public bool FastStartupKeyEnabled
    {
      get => _fastStartupKeyEnabled;
      set
      {
        _fastStartupKeyEnabled = value;
        NotifyPropertyChanged(nameof(FastStartupKeyEnabled));
        _registryKeyService.SetKeyState(WindowsRegistryKeyService.RKEY_FAST_BOOT, value);
      }
    }

    private bool _contextMenuKeyEnabled;
    public bool ContextMenuKeyEnabled
    {
      get => _contextMenuKeyEnabled;  
      set
      {
        _contextMenuKeyEnabled = value;
        NotifyPropertyChanged(nameof(ContextMenuKeyEnabled));
        _registryKeyService.SetKeyState(WindowsRegistryKeyService.RKEY_CONTEXT_MENU, value);
      }
    }

    public MVVM.RelayCommand ForceExplorerRestartCommand => new(_ => true, (e) => ForceExplorerRestart());
    public void ForceExplorerRestart()
    {
      try
      {
        if (_dialogService.ShowConfirmationDialog("This action will restart the Windows explorer process. Continue?", "Atención"))
        {
          Process.GetProcesses()
            .Where(p => p.ProcessName.ToLower().Equals("explorer"))
            .ToList()
            .ForEach(p => p.Kill());

          Process.Start("explorer.exe");
        }
      }
      catch (Exception ex)
      {
        _dialogService.ShowErrorDialog(ex.Message, "Unable to force Explorer restart");
      }
    }
  }
}
