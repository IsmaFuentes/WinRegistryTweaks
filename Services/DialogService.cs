using System.Windows;

namespace WinRegistryTweaks.Services
{
  public interface IDialogService
  {
    public bool ShowConfirmationDialog(string message, string caption);
    public void ShowErrorDialog(string message, string caption);
  }

  public class DialogService : IDialogService
  {
    public bool ShowConfirmationDialog(string message, string caption)
    {
      return MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
    }

    public void ShowErrorDialog(string message, string caption)
    {
      MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
    }
  }
}
