using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Medella.TdsClient.LocalDb
{
    public sealed class SafeLibraryHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        internal const string Kernel32 = "kernel32.dll";

        internal SafeLibraryHandle() : base(true)
        {
        }

        internal SafeLibraryHandle(bool ownsHandle) : base(ownsHandle)
        {
        }

        protected override bool ReleaseHandle() => FreeLibrary(handle);

        [DllImport(Kernel32, ExactSpelling = true, SetLastError = true)]
        private static extern bool FreeLibrary([In] IntPtr hModule);
    }
}