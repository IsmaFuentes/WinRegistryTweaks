using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WinRegistryTweaks.Reg;

namespace WinRegistryTweaks
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            SetDefaultValues();
        }

        // interesting stuff to take a look at:
        // - https://gist.github.com/trongtinh1212/caa7d00188626d9188f69e781fee82d8
        // - https://github.com/hellzerg/optimizer
        // - https://github.com/svenmauch/WinSlap/blob/master/WinSlap/Slapper.cs

        private void SetDefaultValues()
        {
            // General
            chkDisableDriverSearch.IsChecked = RegistryEditor.IsRegistryKeyEnabled(@"SOFTWARE\Microsoft\Windows\CurrentVersion\DriverSearching", "SearchOrderConfig", RegistryKeyType.Local, 1);
            chkDisableWIDriverUpdates.IsChecked = RegistryEditor.IsRegistryKeyEnabled(@"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate", "ExcludeWUDriversInQualityUpdate", RegistryKeyType.Local, 1);
            chkDisableWebSearch.IsChecked = RegistryEditor.IsRegistryKeyEnabled(@"SOFTWARE\Policies\Microsoft\Windows\Explorer", "DisableSearchBoxSuggestions", RegistryKeyType.CurrentUser, 1);
            chkDisableFastStartup.IsChecked = RegistryEditor.IsRegistryKeyEnabled(@"SYSTEM\CurrentControlSet\Control\Session Manager\Power", "HiberbootEnabled", RegistryKeyType.Local, 0);

            // Win11
            if (RegistryEditor.RegistryEntryExists(@"SOFTWARE\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InProcServer32", RegistryKeyType.CurrentUser))
            {
                chkDisableW11ContextMenu.IsChecked = true;
            }
        }

        private void CheckboxChecked(object sender, RoutedEventArgs e)
        {
            try
            {
                var parent = (CheckBox)sender;

                switch (parent.Name)
                {
                    case "chkDisableDriverSearch":
                        RegistryEditor.SetRegistryValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\DriverSearching", "SearchOrderConfig", RegistryKeyType.Local, parent.IsChecked == true ? 1 : 0);
                        break;
                    case "chkDisableWIDriverUpdates":
                        RegistryEditor.SetRegistryValue(@"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate", "ExcludeWUDriversInQualityUpdate", RegistryKeyType.Local, parent.IsChecked == true ? 1 : 0);
                        break;
                    case "chkDisableWebSearch":
                        RegistryEditor.SetRegistryValue(@"Software\Policies\Microsoft\Windows\Explorer", "DisableSearchBoxSuggestions", RegistryKeyType.CurrentUser, parent.IsChecked == true ? 1 : 0);
                        break;
                    case "chkDisableFastStartup":
                        RegistryEditor.SetRegistryValue(@"SYSTEM\CurrentControlSet\Control\Session Manager\Power", "HiberbootEnabled", RegistryKeyType.Local, parent.IsChecked == true ? 0 : 1);
                        break;
                    case "chkDisableW11ContextMenu":
                        SetWin11ContextMenu(parent.IsChecked);
                        break;
                    default:
                        break;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Unable to set registry value", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetWin11ContextMenu(bool? enabled)
        {
            // In powershell:
            // disable: reg.exe add "HKCU\Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InprocServer32" /f /ve
            // enable:  reg.exe delete "HKCU\Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}" /f

            if (enabled == true)
            {
                RegistryEditor.SetRegistryValue(@"SOFTWARE\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InProcServer32", "", RegistryKeyType.CurrentUser, "", Microsoft.Win32.RegistryValueKind.String);
            }
            else
            {
                RegistryEditor.DeleteRegistryKey(@"SOFTWARE\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InProcServer32", "", RegistryKeyType.CurrentUser);
            }
        }

        private void DisableTelemetry()
        {
            //RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Windows\DataCollection", true);
            //key.SetValue("AllowTelemetry", "0", RegistryValueKind.DWord);

            //RegistryKey key2 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\DataCollection", true);
            //key2.SetValue("AllowTelemetry", "0", RegistryValueKind.DWord);

            //RegistryKey key3 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Policies\DataCollection", true);
            //key3.SetValue("AllowTelemetry", "0", RegistryValueKind.DWord);

            //Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\WMI\Autologger\AutoLogger-Diagtrack-Listener");
            //RegistryKey key4 = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\WMI\Autologger\AutoLogger-Diagtrack-Listener", true);
            //key4.SetValue("Start", "0", RegistryValueKind.DWord);

            //RegistryKey key5 = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\dmwappushservice", true);
            //key5.SetValue("Start", "4", RegistryValueKind.DWord);

            //RegistryKey key6 = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\DiagTrack", true);
            //key6.SetValue("Start", "4", RegistryValueKind.DWord);

            //Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\AppCompat", true);

            //RegistryKey key7 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Windows\AppCompat", true);
            //key7.SetValue("AITEnable", "0", RegistryValueKind.DWord);

            //using (PowerShell ps = PowerShell.Create())
            //{
            //    ps.AddScript("Disable-ScheduledTask -TaskName \"Microsoft\\Windows\\Application Experience\\Microsoft Compatibility Appraiser\"");
            //    ps.AddScript("Disable-ScheduledTask -TaskName \"Microsoft\\Windows\\Application Experience\\ProgramDataUpdater\"");
            //    ps.AddScript("Disable-ScheduledTask -TaskName \"Microsoft\\Windows\\Autochk\\Proxy\"");
            //    ps.AddScript("Disable-ScheduledTask -TaskName \"Microsoft\\Windows\\Customer Experience Improvement Program\\Consolidator\"");
            //    ps.AddScript("Disable-ScheduledTask -TaskName \"Microsoft\\Windows\\Customer Experience Improvement Program\\UsbCeip\"");
            //    ps.AddScript("Disable-ScheduledTask -TaskName \"Microsoft\\Windows\\DiskDiagnostic\\Microsoft-Windows-DiskDiagnosticDataCollector\"");
            //    ps.Invoke();
            //}
        }

        private void DisableTabletTypingInfo()
        {
            //Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\TabletPC");
            //Registry.LocalMachine.CreateSubKey(@"SOFTWARE\WOW6432Node\Policies\Microsoft\Windows\TabletPC");

            //RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Input\TIPC", true);
            //key.SetValue("Enabled", "0", RegistryValueKind.DWord);

            //RegistryKey key2 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Windows\TabletPC", true);
            //key2.SetValue("PreventHandwritingDataSharing", "1", RegistryValueKind.DWord);

            //RegistryKey key3 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Policies\Microsoft\Windows\TabletPC", true);
            //key3.SetValue("PreventHandwritingDataSharing", "1", RegistryValueKind.DWord);
        }

        private void DisableAppDiagnostics()
        {
            //RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Privacy", true);
            //key.SetValue("TailoredExperiencesWithDiagnosticDataEnabled", "0", RegistryValueKind.DWord);

            //RegistryKey key2 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Privacy", true);
            //key2.SetValue("TailoredExperiencesWithDiagnosticDataEnabled", "0", RegistryValueKind.DWord);
        }

        private void ForceExplorerRestart(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show("This will restart Windows explorer process. Continue?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if(result == MessageBoxResult.Yes)
                {
                    Process.GetProcesses()
                        .Where(p => p.ProcessName.ToLower().Equals("explorer"))
                        .ToList()
                        .ForEach(p => p.Kill());

                    Process.Start("explorer.exe");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Unable to force Explorer restart", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
