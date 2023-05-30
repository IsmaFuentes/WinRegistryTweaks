using System;
using System.Collections.Generic;

namespace WinRegistryTweaks
{
    public class WinUtils
    {
        /// <summary>
        /// Windows build versions: https://en.wikipedia.org/wiki/List_of_Microsoft_Windows_versions
        /// </summary>
        private static Dictionary<int, string> _versions = new Dictionary<int, string>()
        {
            [22000] = "Windows 11 21H2",
            [22621] = "Windows 11 22H2"
        };

        // Environment.OSVersion.Version.Build >= 22000

        public static bool IsWindows11 => _versions.ContainsKey(Environment.OSVersion.Version.Build);
    }
}
