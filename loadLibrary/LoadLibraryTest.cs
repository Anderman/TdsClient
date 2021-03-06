using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Xunit;
using System.Text;
using Microsoft.Win32;
namespace loadLibrary
{
    public class LoadLibraryTest
    {
        internal const string Kernel32 = "kernel32.dll";

        [DllImport(Kernel32, ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern SafeLibraryHandle LoadLibraryExW([In] string lpwLibFileName, [In] IntPtr hFile, [In] uint dwFlags);

        [DllImport(Kernel32, CharSet = CharSet.Ansi, BestFitMapping = false)]
        private static extern uint GetLastError();


        private SafeLibraryHandle LoadUserInstanceDll()
        {
            // Load the dll

            var dllPath = GetUserInstanceDllPath();
            var libraryHandle = LoadLibraryExW(dllPath, IntPtr.Zero, 0);
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
        private const string LocalDbInstalledVersionRegistryKey = "SOFTWARE\\Microsoft\\Microsoft SQL Server Local DB\\Installed Versions\\";

        private const string InstanceApiPathValueName = "InstanceAPIPath";

        private static string GetUserInstanceDllPath()
        {
            using var key = Registry.LocalMachine.OpenSubKey(LocalDbInstalledVersionRegistryKey);
            if (key == null)
                throw new InvalidOperationException("<sc.SNI.LocalDB.Windows.GetUserInstanceDllPath |SNI|ERR > not installed. Error state ={0}.");

            var zeroVersion = new Version();

            var latestVersion = zeroVersion;

            foreach (var subKey in key.GetSubKeyNames())
                if (Version.TryParse(subKey, out var currentKeyVersion) && latestVersion.CompareTo(currentKeyVersion) < 0)
                    latestVersion = currentKeyVersion;

            // If no valid versions are found, then error out
            if (latestVersion.Equals(zeroVersion))
                throw new InvalidOperationException("<sc.SNI.LocalDB.Windows.GetUserInstanceDllPath |SNI|ERR > Invalid Configuration. state ={0}.");

            // Use the latest version to get the DLL path
            using var latestVersionKey = key.OpenSubKey(latestVersion.ToString());
            var instanceApiPathRegistryObject = latestVersionKey!.GetValue(InstanceApiPathValueName);
            if (instanceApiPathRegistryObject == null)
                throw new InvalidOperationException("<sc.SNI.LocalDB.Windows.GetUserInstanceDllPath |SNI|ERR > No SQL user instance DLL. Instance API Path Registry Object Error. state ={0}.");

            var valueKind = latestVersionKey.GetValueKind(InstanceApiPathValueName);
            if (valueKind != RegistryValueKind.String)
                throw new InvalidOperationException("<sc.SNI.LocalDB.Windows.GetUserInstanceDllPath |SNI|ERR > No SQL user instance DLL. state ={0}. Registry value kind error.");

            var dllPath = (string)instanceApiPathRegistryObject;

            return dllPath;
        }
    }
}