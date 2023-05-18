using System;
using System.Windows;
using System.Windows.Controls;
using WinRegistryTweaks.Reg;

namespace WinRegistryTweaks
{
    public partial class MainWindow : Window
    {
        #region general tweaks (W10/W11)

        private RegEntry DriverSearching = new RegEntry(@"SOFTWARE\Microsoft\Windows\CurrentVersion\DriverSearching", RegistryKeyType.Local);
        private RegEntry WindowsUpdate = new RegEntry(@"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate", RegistryKeyType.Local);
        private RegEntry WindowsExplorer = new RegEntry(@"Software\Policies\Microsoft\Windows\Explorer", RegistryKeyType.CurrentUser);

        #endregion

        #region w11

        private RegEntry WindowsContextMenu = new RegEntry(@"SOFTWARE\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}", RegistryKeyType.CurrentUser);

        #endregion

        public MainWindow()
        {
            InitializeComponent();

            SetDefaultValues();
        }

        private void SetDefaultValues()
        {
            // General
            chkDisableDriverSearch.IsChecked = DriverSearching.GetEntryValue("SearchOrderConfig")?.ToString() == "1";
            chkDisableWIDriverUpdates.IsChecked = WindowsUpdate.GetEntryValue("ExcludeWUDriversInQualityUpdate")?.ToString() == "1";
            chkDisableWebSearch.IsChecked = WindowsExplorer.GetEntryValue("DisableSearchBoxSuggestions")?.ToString() == "1";

            // Win11
            if(WindowsContextMenu.GetEntryValue("InProcServer32") == null)
            {
                chkDisableW11ContextMenu.IsChecked = false;
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
                        DriverSearching.SetEntryValue("SearchOrderConfig", parent.IsChecked == true ? 1 : 0);
                        break;
                    case "chkDisableWIDriverUpdates":
                        WindowsUpdate.SetEntryValue("ExcludeWUDriversInQualityUpdate", parent.IsChecked == true ? 1 : 0);
                        break;
                    case "chkDisableWebSearch":
                        WindowsExplorer.SetEntryValue("DisableSearchBoxSuggestions", parent.IsChecked == true ? 1 : 0);
                        break;
                    case "chkDisableW11ContextMenu":
                        // does not work aparently
                        // WindowsContextMenuRegistry.SetEntryValue("InProcServer32", parent.IsChecked == true ? "" : "Apartment", Microsoft.Win32.RegistryValueKind.String);
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
    }
}
