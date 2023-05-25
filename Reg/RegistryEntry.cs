using Microsoft.Win32;
using System;

namespace WinRegistryTweaks.Reg
{
    public class RegistryEntry : IDisposable
    {
        private RegistryKey _entry;

        public RegistryEntry(string keyPath, RegistryKeyType type)
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

        public void Dispose() => _entry.Dispose();

        public static bool EntryExists(string keyPath, RegistryKeyType type)
        {
            switch (type)
            {
                case RegistryKeyType.Local:
                    return Registry.LocalMachine.OpenSubKey(keyPath) != null;
                case RegistryKeyType.CurrentConfig:
                    return Registry.CurrentConfig.OpenSubKey(keyPath) != null;
                case RegistryKeyType.CurrentUser:
                    return Registry.CurrentUser.OpenSubKey(keyPath) != null;
                default:
                    return false;
            }
        }

        public object? GetEntryValue(string keyValue) => _entry.GetValue(keyValue);

        public void SetEntryValue(string keyValue, object value) => _entry.SetValue(keyValue, value);

        public void SetEntryValue(string keyValue, object value, RegistryValueKind kind) => _entry.SetValue(keyValue, value, kind);

        public void DeleteEntry(string keyValue) => _entry.DeleteSubKey(keyValue);
    }
}
