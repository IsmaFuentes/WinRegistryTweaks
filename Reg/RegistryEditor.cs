using Microsoft.Win32;

namespace WinRegistryTweaks.Reg
{
    public class RegistryEditor
    {
        public static void SetRegistryValue(string path, string key, RegistryKeyType type, object value)
        {
            using (var entry = new RegistryEntry(path, type))
            {
                entry.SetEntryValue(key, value);
            }
        }

        public static void SetRegistryValue(string path, string key, RegistryKeyType type, object value, RegistryValueKind kind)
        {
            using (var entry = new RegistryEntry(path, type))
            {
                entry.SetEntryValue(key, value, kind);
            }
        }

        public static void DeleteRegistryKey(string path, string key, RegistryKeyType type)
        {
            using(var entry = new RegistryEntry(path, type))
            {
                entry.DeleteEntry(key);
            }
        }

        public static object? GetRegistryValue(string path, string key, RegistryKeyType type)
        {
            using (var entry = new RegistryEntry(path, type))
            {
                return entry.GetEntryValue(key);
            }
        }

        public static bool RegistryEntryExists(string path, RegistryKeyType type)
        {
            return RegistryEntry.EntryExists(path, type);
        }

        public static bool IsRegistryKeyEnabled(string path, string key, RegistryKeyType type, object expectedValue)
        {
            using(var entry = new RegistryEntry(path, type))
            {
                var value = entry.GetEntryValue(key);

                if(value == null)
                {
                    return false;
                }

                return value.Equals(expectedValue);
            }
        }
    }
}
