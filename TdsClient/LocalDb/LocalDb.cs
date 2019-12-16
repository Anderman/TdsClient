using System;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace Medella.TdsClient.LocalDb
{
    public class LocalDb
    {
        //HKEY_LOCAL_MACHINE

        private const string ProcLocalDbStartInstance = "LocalDBStartInstance";

        private const int MaxLocalDbConnectionStringSize = 260;
        internal const string Kernel32 = "kernel32.dll";

        /// <summary>
        ///     Retrieves the part of the sqlUserInstance.dll from the registry
        /// </summary>
        /// <returns></returns>
        private static readonly LocalDb Instance = new LocalDb();

        private LocalDbStartInstance _localDbStartInstanceFunc;

        private volatile object _sqlUserInstanceLibraryHandle;

        private IntPtr _startInstanceHandle = IntPtr.Zero;

        private LocalDb()
        {
        }

        public static string GetLocalDbConnectionString(string localDbInstance) => Instance.LoadUserInstanceDll() ? Instance.GetConnectionString(localDbInstance) : null;

        private string GetConnectionString(string localDbInstance)
        {
            var localDbConnectionString = new StringBuilder(MaxLocalDbConnectionStringSize + 1);
            var sizeOfBuffer = localDbConnectionString.Capacity;
            _localDbStartInstanceFunc(localDbInstance, 0, localDbConnectionString, ref sizeOfBuffer);
            return localDbConnectionString.ToString();
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
                var dllPath = GetUserInstanceDllPath();

                // Load the dll
                var libraryHandle = LoadLibraryExW(dllPath.Trim(), IntPtr.Zero, 0);
                if (libraryHandle.IsInvalid)
                {
                    var error = GetLastError();
                    throw new Exception(error.ToString());
                }

                // Load the process from the DLLs
                _startInstanceHandle = GetProcAddress(libraryHandle, ProcLocalDbStartInstance);

                // Set the delegate the invoke.
                _localDbStartInstanceFunc = (LocalDbStartInstance)Marshal.GetDelegateForFunctionPointer(_startInstanceHandle, typeof(LocalDbStartInstance));
                _sqlUserInstanceLibraryHandle = libraryHandle;

                return true;
            }
        }

        [DllImport(Kernel32, CharSet = CharSet.Ansi, BestFitMapping = false)]
        private static extern IntPtr GetProcAddress(SafeLibraryHandle hModule, string lpProcName);

        [DllImport(Kernel32, ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern SafeLibraryHandle LoadLibraryExW([In] string lpwLibFileName, [In] IntPtr hFile, [In] uint dwFlags);

        [DllImport(Kernel32, CharSet = CharSet.Ansi, BestFitMapping = false)]
        private static extern uint GetLastError();

        private static string GetUserInstanceDllPath() => @"C:\Program Files\Microsoft SQL Server\130\LocalDB\Binn\SqlUserInstance.dll";

        // Local Db api doc https://msdn.microsoft.com/en-us/library/hh217143.aspx
        // HRESULT LocalDBStartInstance( [Input ] PCWSTR pInstanceName, [Input ] DWORD dwFlags,[Output] LPWSTR wszSqlConnection,[Input/Output] LPDWORD lpcchSqlConnection);  
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int LocalDbStartInstance([In] [MarshalAs(UnmanagedType.LPWStr)] string localDbInstanceName, [In] int flags, [Out] [MarshalAs(UnmanagedType.LPWStr)]
            StringBuilder sqlConnectionDataSource, [In] [Out] ref int bufferLength);
    }

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