using Microsoft.Win32;
using System;
using System.Collections.Generic;

namespace WinRegistryTweaks.Services
{
  public enum RegistryKeyType
  {
    Local,
    CurrentUser,
    CurrentConfig
  }

  public class WindowsRegistryKeyService : IWindowsRegistryKeyService
  {
    public const string RKEY_DRIVER_SEARCH = "SearchOrderConfig";
    public const string RKEY_EXCLUDE_WU_DRIVERS = "ExcludeWUDriversInQualityUpdate";
    public const string RKEY_WEB_SEARCH = "DisableSearchBoxSuggestions";
    public const string RKEY_FAST_BOOT = "HiberbootEnabled";
    public const string RKEY_CONTEXT_MENU = "InProcServer32";

    private record RegistrySetting(string Path, RegistryKeyType KeyType, object Value, bool IsSubKey = false);

    private readonly Dictionary<string, RegistrySetting> _registrySettings = new()
    {
      { RKEY_DRIVER_SEARCH, new RegistrySetting(@"SOFTWARE\Microsoft\Windows\CurrentVersion\DriverSearching", RegistryKeyType.Local, 1) },
      { RKEY_EXCLUDE_WU_DRIVERS, new RegistrySetting(@"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate", RegistryKeyType.Local, 1) },
      { RKEY_WEB_SEARCH, new RegistrySetting(@"SOFTWARE\Policies\Microsoft\Windows\Explorer", RegistryKeyType.CurrentUser, 1) },
      { RKEY_FAST_BOOT, new RegistrySetting(@"SYSTEM\CurrentControlSet\Control\Session Manager\Power", RegistryKeyType.Local, 0) },
      { RKEY_CONTEXT_MENU, new RegistrySetting(@"SOFTWARE\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}", RegistryKeyType.CurrentUser, "", IsSubKey: true) }
    };

    private RegistryKey OpenBaseKey(RegistryKeyType type)
    {
      return type switch
      {
        RegistryKeyType.Local => Registry.LocalMachine,
        RegistryKeyType.CurrentUser => Registry.CurrentUser,
        RegistryKeyType.CurrentConfig => Registry.CurrentConfig,
        _ => throw new ArgumentOutOfRangeException(nameof(type))
      };
    }

    public bool GetKeyState(string key)
    {
      if (!_registrySettings.TryGetValue(key, out var setting))
        throw new ArgumentException($"Setting {key} is not registered in the dictionary.");

      if (setting.IsSubKey)
      {
        using(var baseKey = OpenBaseKey(setting.KeyType))
        {
          using(var subKey = baseKey.OpenSubKey($@"{setting.Path}\{key}"))
          {
            return subKey != null;
          }
        }
      }

      using(var baseKey = OpenBaseKey(setting.KeyType))
      {
        using(var subKey = baseKey.OpenSubKey(setting.Path))
        {
          var value = subKey?.GetValue(key);
          if (value == null)
            return false;

          return value.Equals(setting.Value);
        }
      }
    }

    public void SetKeyState(string key, bool enabled)
    {
      if (!_registrySettings.TryGetValue(key, out var setting))
        throw new ArgumentException($"Setting {key} is not registered in the dictionary.");

      using(var baseKey = OpenBaseKey(setting.KeyType))
      {
        if (setting.IsSubKey)
        {
          string keyPath = $@"{setting.Path}\{key}";

          if (enabled)
          {
            using(var subKey = baseKey.CreateSubKey(keyPath))
            {
              subKey.SetValue("", "", RegistryValueKind.String);
            }
          }
          else
          {
            baseKey.DeleteSubKeyTree(keyPath, false);
          }

          return;
        }

        object newValue = (enabled ? setting.Value : 0);

        using (var subKey = baseKey.CreateSubKey(setting.Path))
        {
          subKey.SetValue(key, newValue);
        }
      }
    }
  }
}


// interesting stuff to take a look at:
// - https://gist.github.com/trongtinh1212/caa7d00188626d9188f69e781fee82d8
// - https://github.com/hellzerg/optimizer
// - https://github.com/svenmauch/WinSlap/blob/master/WinSlap/Slapper.cs
// - https://github.com/spinda/Destroy-Windows-10-Spying/tree/master/DWS

//private void DisableTelemetry()
//{
//    RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Windows\DataCollection", true);
//    key.SetValue("AllowTelemetry", "0", RegistryValueKind.DWord);

//    RegistryKey key2 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\DataCollection", true);
//    key2.SetValue("AllowTelemetry", "0", RegistryValueKind.DWord);

//    RegistryKey key3 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Policies\DataCollection", true);
//    key3.SetValue("AllowTelemetry", "0", RegistryValueKind.DWord);

//    Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\WMI\Autologger\AutoLogger-Diagtrack-Listener");
//    RegistryKey key4 = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\WMI\Autologger\AutoLogger-Diagtrack-Listener", true);
//    key4.SetValue("Start", "0", RegistryValueKind.DWord);

//    RegistryKey key5 = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\dmwappushservice", true);
//    key5.SetValue("Start", "4", RegistryValueKind.DWord);

//    RegistryKey key6 = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\DiagTrack", true);
//    key6.SetValue("Start", "4", RegistryValueKind.DWord);

//    Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\AppCompat", true);

//    RegistryKey key7 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Windows\AppCompat", true);
//    key7.SetValue("AITEnable", "0", RegistryValueKind.DWord);

//    using (PowerShell ps = PowerShell.Create())
//    {
//        ps.AddScript("Disable-ScheduledTask -TaskName \"Microsoft\\Windows\\Application Experience\\Microsoft Compatibility Appraiser\"");
//        ps.AddScript("Disable-ScheduledTask -TaskName \"Microsoft\\Windows\\Application Experience\\ProgramDataUpdater\"");
//        ps.AddScript("Disable-ScheduledTask -TaskName \"Microsoft\\Windows\\Autochk\\Proxy\"");
//        ps.AddScript("Disable-ScheduledTask -TaskName \"Microsoft\\Windows\\Customer Experience Improvement Program\\Consolidator\"");
//        ps.AddScript("Disable-ScheduledTask -TaskName \"Microsoft\\Windows\\Customer Experience Improvement Program\\UsbCeip\"");
//        ps.AddScript("Disable-ScheduledTask -TaskName \"Microsoft\\Windows\\DiskDiagnostic\\Microsoft-Windows-DiskDiagnosticDataCollector\"");
//        ps.Invoke();
//    }
//}