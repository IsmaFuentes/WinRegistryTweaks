using Microsoft.Win32;

namespace WinRegistryTweaks.Reg
{
    public class RegEntry
    {
        private RegistryKey _entry;

        public RegEntry(string keyPath, RegistryKeyType type)
        {
            switch (type)
            {
                case RegistryKeyType.Local:
                    _entry = Registry.LocalMachine.CreateSubKey(keyPath, true);
                    break;
                case RegistryKeyType.CurrentConfig:
                    _entry = Registry.CurrentConfig.CreateSubKey(keyPath, true);
                    break;
                case RegistryKeyType.CurrentUser:
                    _entry = Registry.CurrentUser.CreateSubKey(keyPath, true);
                    break;
                default:
                    _entry = Registry.LocalMachine.CreateSubKey(keyPath, true);
                    break;
            }
        }

        public object? GetEntryValue(string keyValue) => _entry.GetValue(keyValue);

        public void SetEntryValue(string keyValue, object value) => _entry.SetValue(keyValue, value);

        public void SetEntryValue(string keyValue, object value, RegistryValueKind kind) => _entry.SetValue(keyValue, value, kind);

        public void DeleteEntry(string keyValue) => _entry.DeleteSubKey(keyValue);
    }
}
