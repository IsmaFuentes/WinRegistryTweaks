using System.Windows;

namespace WinRegistryTweaks
{
  public partial class MainWindow : Window
  {
    public MainWindow(ViewModels.MainViewModel vm)
    {
      DataContext = vm;
      InitializeComponent();
    }
  }
}
