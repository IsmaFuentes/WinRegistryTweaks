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
        #region general tweaks (W10/W11)

        private RegEntry WindowsDriverSearh = new RegEntry(@"SOFTWARE\Microsoft\Windows\CurrentVersion\DriverSearching", RegistryKeyType.Local);
        private RegEntry WindowsUpdate = new RegEntry(@"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate", RegistryKeyType.Local);
        private RegEntry WindowsExplorer = new RegEntry(@"Software\Policies\Microsoft\Windows\Explorer", RegistryKeyType.CurrentUser);

        #endregion

        #region w11

        // private RegEntry WindowsContextMenu = new RegEntry(@"SOFTWARE\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InProcServer32", RegistryKeyType.CurrentUser);

        #endregion

        public MainWindow()
        {
            InitializeComponent();

            SetDefaultValues();
        }

        // interesting stuff to take a look at:
        // - https://gist.github.com/trongtinh1212/caa7d00188626d9188f69e781fee82d8
        // - https://github.com/hellzerg/optimizer

        private void SetDefaultValues()
        {
            // General
            chkDisableDriverSearch.IsChecked = WindowsDriverSearh.GetEntryValue("SearchOrderConfig")?.ToString() == "1";
            chkDisableWIDriverUpdates.IsChecked = WindowsUpdate.GetEntryValue("ExcludeWUDriversInQualityUpdate")?.ToString() == "1";
            chkDisableWebSearch.IsChecked = WindowsExplorer.GetEntryValue("DisableSearchBoxSuggestions")?.ToString() == "1";

            // Win11
            if(RegEntry.EntryExists(@"SOFTWARE\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InProcServer32", RegistryKeyType.CurrentUser))
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
                        WindowsDriverSearh.SetEntryValue("SearchOrderConfig", parent.IsChecked == true ? 1 : 0);
                        break;
                    case "chkDisableWIDriverUpdates":
                        WindowsUpdate.SetEntryValue("ExcludeWUDriversInQualityUpdate", parent.IsChecked == true ? 1 : 0);
                        break;
                    case "chkDisableWebSearch":
                        WindowsExplorer.SetEntryValue("DisableSearchBoxSuggestions", parent.IsChecked == true ? 1 : 0);
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
                new RegEntry(@"SOFTWARE\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InProcServer32", RegistryKeyType.CurrentUser).SetEntryValue("", "", Microsoft.Win32.RegistryValueKind.String);
            }
            else
            {
                new RegEntry(@"SOFTWARE\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InProcServer32", RegistryKeyType.CurrentUser).DeleteEntry("");
            }
        }

        private void ForceExplorerRestart(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.GetProcesses()
                    .Where(p => p.ProcessName.ToLower().Equals("explorer"))
                    .ToList()
                    .ForEach(p => p.Kill());

                Process.Start("explorer.exe");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Unable to force Explorer restart", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
