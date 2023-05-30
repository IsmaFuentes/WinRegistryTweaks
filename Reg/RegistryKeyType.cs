using System;

namespace WinRegistryTweaks.Reg
{
    [Flags]
    public enum RegistryKeyType : int
    {
        Local,
        CurrentUser,
        CurrentConfig
    }
}
