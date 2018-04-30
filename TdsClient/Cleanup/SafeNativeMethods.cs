using System;
using System.Runtime.InteropServices;

namespace Medella.TdsClient.Cleanup
{
    internal static class SafeNativeMethods
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true, SetLastError = true)]
        internal static extern IntPtr GetProcAddress(IntPtr HModule, [MarshalAs(UnmanagedType.LPStr)] [In] string funcName);
    }
}