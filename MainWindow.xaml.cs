﻿using System;
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

        private void SetDefaultValues()
        {
            // Windows 10/11
            chkDisableDriverSearch.IsChecked = RegistryEditor.IsRegistryKeyEnabled(@"SOFTWARE\Microsoft\Windows\CurrentVersion\DriverSearching", "SearchOrderConfig", RegistryKeyType.Local, 1);
            chkDisableWIDriverUpdates.IsChecked = RegistryEditor.IsRegistryKeyEnabled(@"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate", "ExcludeWUDriversInQualityUpdate", RegistryKeyType.Local, 1);
            chkDisableWebSearch.IsChecked = RegistryEditor.IsRegistryKeyEnabled(@"SOFTWARE\Policies\Microsoft\Windows\Explorer", "DisableSearchBoxSuggestions", RegistryKeyType.CurrentUser, 1);
            chkDisableFastStartup.IsChecked = RegistryEditor.IsRegistryKeyEnabled(@"SYSTEM\CurrentControlSet\Control\Session Manager\Power", "HiberbootEnabled", RegistryKeyType.Local, 0);

            // Windows 11
            chkDisableW11ContextMenu.IsChecked = RegistryEditor.RegistryEntryExists(@"SOFTWARE\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InProcServer32", RegistryKeyType.CurrentUser);
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
            // powershell variant:
            // disable: reg.exe add "HKCU\Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InprocServer32" /f /ve
            // enable:  reg.exe delete "HKCU\Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}" /f

            if (enabled == true)
            {
                string subkey = @"SOFTWARE\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}";

                using (var key = new RegistryEntry(subkey, RegistryKeyType.CurrentUser))
                {
                    RegistryEditor.SetRegistryValue(@$"{subkey}\InProcServer32", "", RegistryKeyType.CurrentUser, "", Microsoft.Win32.RegistryValueKind.String);
                }
            }
            else
            {
                RegistryEditor.DeleteRegistryKey(@"SOFTWARE\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InProcServer32", "", RegistryKeyType.CurrentUser);
            }
        }

        private void ForceExplorerRestart(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialogResult = MessageBox.Show("This action will restart the Windows explorer process. Continue?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                
                if (dialogResult == MessageBoxResult.Yes)
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
                MessageBox.Show(ex.Message, "Unable to force Explorer restart", MessageBoxButton.OK, MessageBoxImage.Error);
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
    }
}
