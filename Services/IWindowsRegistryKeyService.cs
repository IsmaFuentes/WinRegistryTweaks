namespace WinRegistryTweaks.Services
{
  public interface IWindowsRegistryKeyService
  {
    public bool GetKeyState(string key);
    public void SetKeyState(string key, bool enabled);
  }
}
