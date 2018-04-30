using System;
using System.Runtime.InteropServices;
using System.Text;
using Medella.TdsClient.SNI;
using Medella.TdsClient.SNI.Internal;
using Medella.TdsClient.System2.Net.Security;

namespace Medella.TdsClient.LocalDb
{
    public class LocalDb
    {
        //HKEY_LOCAL_MACHINE

        private const string ProcLocalDbStartInstance = "LocalDBStartInstance";

        private const int MaxLocalDbConnectionStringSize = 260;

        /// <summary>
        ///     Retrieves the part of the sqlUserInstance.dll from the registry
        /// </summary>
        /// <returns></returns>
        private const string LocalDbInstalledVersionRegistryKey = "SOFTWARE\\Microsoft\\Microsoft SQL Server Local DB\\Installed Versions\\";

        private const string InstanceApiPathValueName = "InstanceAPIPath";
        private static readonly LocalDb Instance = new LocalDb();

        private LocalDbStartInstance _localDbStartInstanceFunc;

        private volatile SafeLibraryHandle _sqlUserInstanceLibraryHandle;

        private IntPtr _startInstanceHandle = IntPtr.Zero;

        private LocalDb()
        {
        }

        public static string GetLocalDbConnectionString(string localDbInstance)
        {
            return Instance.LoadUserInstanceDll() ? Instance.GetConnectionString(localDbInstance) : null;
        }

        internal static IntPtr GetProcAddress(string functionName)
        {
            return Instance.LoadUserInstanceDll() ? Interop.Kernel32.GetProcAddress(Instance._sqlUserInstanceLibraryHandle, functionName) : IntPtr.Zero;
        }

        private string GetConnectionString(string localDbInstance)
        {
            var localDbConnectionString = new StringBuilder(MaxLocalDbConnectionStringSize + 1);
            var sizeOfbuffer = localDbConnectionString.Capacity;
            _localDbStartInstanceFunc(localDbInstance, 0, localDbConnectionString, ref sizeOfbuffer);
            return localDbConnectionString.ToString();
        }

        internal static uint MapLocalDbErrorStateToCode(LocalDbErrorState errorState)
        {
            switch (errorState)
            {
                case LocalDbErrorState.NO_INSTALLATION:
                    return SniCommon.LocalDBNoInstallation;
                case LocalDbErrorState.INVALID_CONFIG:
                    return SniCommon.LocalDBInvalidConfig;
                case LocalDbErrorState.NO_SQLUSERINSTANCEDLL_PATH:
                    return SniCommon.LocalDBNoSqlUserInstanceDllPath;
                case LocalDbErrorState.INVALID_SQLUSERINSTANCEDLL_PATH:
                    return SniCommon.LocalDBInvalidSqlUserInstanceDllPath;
                case LocalDbErrorState.NONE:
                    return 0;
                default:
                    return SniCommon.LocalDBInvalidConfig;
            }
        }

        /// <summary>
        ///     Loads the User Instance dll.
        /// </summary>
        private bool LoadUserInstanceDll()
        {
            // Check in a non thread-safe way if the handle is already set for performance.
            if (_sqlUserInstanceLibraryHandle != null) return true;

            lock (this)
            {
                if (_sqlUserInstanceLibraryHandle != null) return true;
                //Get UserInstance Dll path

                // Get the LocalDB instance dll path from the registry
                var dllPath = GetUserInstanceDllPath(out var registryQueryErrorState);

                // If there was no DLL path found, then there is an error.
                if (dllPath == null)
                {
                    SniLoadHandle.SingletonInstance.LastError = new SniError(SniProviders.INVALID_PROV, 0, MapLocalDbErrorStateToCode(registryQueryErrorState), string.Empty);
                    return false;
                }

                // In case the registry had an empty path for dll
                if (string.IsNullOrWhiteSpace(dllPath))
                {
                    SniLoadHandle.SingletonInstance.LastError = new SniError(SniProviders.INVALID_PROV, 0, SniCommon.LocalDBInvalidSqlUserInstanceDllPath, string.Empty);
                    return false;
                }

                // Load the dll
                var libraryHandle = Interop.Kernel32.LoadLibraryExW(dllPath.Trim(), IntPtr.Zero, 0);

                if (libraryHandle.IsInvalid)
                {
                    SniLoadHandle.SingletonInstance.LastError = new SniError(SniProviders.INVALID_PROV, 0, SniCommon.LocalDBFailedToLoadDll, string.Empty);
                    libraryHandle.Dispose();
                    return false;
                }

                // Load the procs from the DLLs
                _startInstanceHandle = Interop.Kernel32.GetProcAddress(libraryHandle, ProcLocalDbStartInstance);

                if (_startInstanceHandle == IntPtr.Zero)
                {
                    SniLoadHandle.SingletonInstance.LastError = new SniError(SniProviders.INVALID_PROV, 0, SniCommon.LocalDBBadRuntime, string.Empty);
                    libraryHandle.Dispose();
                    return false;
                }

                // Set the delegate the invoke.
                _localDbStartInstanceFunc = (LocalDbStartInstance)Marshal.GetDelegateForFunctionPointer(_startInstanceHandle, typeof(LocalDbStartInstance));

                if (_localDbStartInstanceFunc == null)
                {
                    SniLoadHandle.SingletonInstance.LastError = new SniError(SniProviders.INVALID_PROV, 0, SniCommon.LocalDBBadRuntime, string.Empty);
                    libraryHandle.Dispose();
                    _startInstanceHandle = IntPtr.Zero;
                    return false;
                }

                _sqlUserInstanceLibraryHandle = libraryHandle;

                return true;
            }
        }

        private static string GetUserInstanceDllPath(out LocalDbErrorState errorState)
        {
            errorState = LocalDbErrorState.NONE;
            return @"C:\Program Files\Microsoft SQL Server\130\LocalDB\Binn\SqlUserInstance.dll";
            //errorState = LocalDbErrorState.NO_INSTALLATION;

            //using (RegistryKey key = Registry.LocalMachine.OpenSubKey(LocalDbInstalledVersionRegistryKey))
            //{
            //    if (key == null)
            //    {
            //        errorState = LocalDbErrorState.NO_INSTALLATION;
            //        return null;
            //    }

            //    var zeroVersion = new Version();

            //    var latestVersion = zeroVersion;

            //    foreach (string subKey in key.GetSubKeyNames())
            //    {
            //        if (!Version.TryParse(subKey, out var currentKeyVersion))
            //        {
            //            errorState = LocalDbErrorState.INVALID_CONFIG;
            //            return null;
            //        }

            //        if (latestVersion.CompareTo(currentKeyVersion) < 0)
            //        {
            //            latestVersion = currentKeyVersion;
            //        }
            //    }

            //    // If no valid versions are found, then error out
            //    if (latestVersion.Equals(zeroVersion))
            //    {
            //        errorState = LocalDbErrorState.INVALID_CONFIG;
            //        return null;
            //    }

            //    // Use the latest version to get the DLL path
            //    using (RegistryKey latestVersionKey = key.OpenSubKey(latestVersion.ToString()))
            //    {

            //        object instanceAPIPathRegistryObject = latestVersionKey.GetValue(InstanceApiPathValueName);

            //        if (instanceAPIPathRegistryObject == null)
            //        {
            //            errorState = LocalDbErrorState.NO_SQLUSERINSTANCEDLL_PATH;
            //            return null;
            //        }

            //        RegistryValueKind valueKind = latestVersionKey.GetValueKind(InstanceApiPathValueName);

            //        if (valueKind != RegistryValueKind.String)
            //        {
            //            errorState = LocalDbErrorState.INVALID_SQLUSERINSTANCEDLL_PATH;
            //            return null;
            //        }

            //        var dllPath = (string)instanceAPIPathRegistryObject;

            //        errorState = LocalDbErrorState.NONE;
            //        return dllPath;
            //}
            //}
        }

        // Local Db api doc https://msdn.microsoft.com/en-us/library/hh217143.aspx
        // HRESULT LocalDBStartInstance( [Input ] PCWSTR pInstanceName, [Input ] DWORD dwFlags,[Output] LPWSTR wszSqlConnection,[Input/Output] LPDWORD lpcchSqlConnection);  
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int LocalDbStartInstance(
            [In] [MarshalAs(UnmanagedType.LPWStr)] string localDbInstanceName,
            [In] int flags,
            [Out] [MarshalAs(UnmanagedType.LPWStr)]
            StringBuilder sqlConnectionDataSource,
            [In] [Out] ref int bufferLength);

        internal enum LocalDbErrorState
        {
            NO_INSTALLATION,
            INVALID_CONFIG,
            NO_SQLUSERINSTANCEDLL_PATH,
            INVALID_SQLUSERINSTANCEDLL_PATH,
            NONE
        }
    }
}