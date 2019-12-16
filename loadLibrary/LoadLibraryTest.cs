using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Xunit;

namespace loadLibrary
{
    public class LoadLibraryTest
    {
        internal const string Kernel32 = "kernel32.dll";

        [DllImport(Kernel32, ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern SafeLibraryHandle LoadLibraryExW([In] string lpwLibFileName, [In] IntPtr hFile, [In] uint dwFlags);

        [DllImport(Kernel32, CharSet = CharSet.Ansi, BestFitMapping = false)]
        private static extern uint GetLastError();

        private const string DllPath = @"C:\Program Files\Microsoft SQL Server\130\LocalDB\Binn\SqlUserInstance.dll";

        private SafeLibraryHandle LoadUserInstanceDll()
        {
            // Load the dll
            var libraryHandle = LoadLibraryExW(DllPath, IntPtr.Zero, 0);
            if (libraryHandle.IsInvalid)
            {
                var error = GetLastError();
                throw new Exception(error.ToString());
            }

            return libraryHandle;
        }

        public sealed class SafeLibraryHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
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

        [Fact]
        public void TestMethod1()
        {
            Assert.False(LoadUserInstanceDll().IsInvalid);
        }
    }
}