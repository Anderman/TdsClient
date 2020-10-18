using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Medella.TdsClient.LocalDb
{
    public static class LocalDb
    {
        private const string ProcLocalDbStartInstance = "LocalDBStartInstance";
        private const int MaxLocalDbConnectionStringSize = 260;
        private const string Kernel32 = "kernel32.dll";
        private static readonly object SpinLock = new object();
        private static LocalDbStartInstance? _localDbStartInstanceFunc;
        private static SafeLibraryHandle? _libraryHandle;

        public static string? GetLocalDbConnectionString(string localDbInstance) // "mssqllocaldb"
        {
            if (_libraryHandle == null && !TryLoadUserInstanceDll())
                return null;
            var localDbConnectionString = new StringBuilder(MaxLocalDbConnectionStringSize + 1);
            var sizeOfBuffer = localDbConnectionString.Capacity;
            _localDbStartInstanceFunc!(localDbInstance, 0, localDbConnectionString, ref sizeOfBuffer);
            return localDbConnectionString.ToString(); //np:\\.\pipe\LOCALDB#7A7A31A5\tsql\query
        }

        private static bool TryLoadUserInstanceDll()
        {
            // Check in a non thread-safe way if the handle is already set for performance.
            lock (SpinLock)
            {
                if (_libraryHandle != null) return true;
                var dllPath = SqlLocalDbPathResolver.GetDllPath();

                // Load the dll
                var libraryHandle = LoadLibraryExW(dllPath.Trim(), IntPtr.Zero, 0);
                if (libraryHandle.IsInvalid)
                    throw new Exception(GetLastError().ToString());

                // Load the process from the DLLs
                var startInstanceHandle = GetProcAddress(libraryHandle, ProcLocalDbStartInstance);

                // Set the delegate the invoke.
                _localDbStartInstanceFunc = (LocalDbStartInstance)Marshal.GetDelegateForFunctionPointer(startInstanceHandle, typeof(LocalDbStartInstance));
                _libraryHandle = libraryHandle; // Hold ref to prevent garbage collection

                return true;
            }
        }

        [DllImport(Kernel32, CharSet = CharSet.Ansi, BestFitMapping = false)]
        private static extern IntPtr GetProcAddress(SafeLibraryHandle hModule, string lpProcName);

        [DllImport(Kernel32, ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern SafeLibraryHandle LoadLibraryExW([In] string lpwLibFileName, [In] IntPtr hFile, [In] uint dwFlags);

        [DllImport(Kernel32, CharSet = CharSet.Ansi, BestFitMapping = false)]
        private static extern uint GetLastError();

        // Local Db api doc https://msdn.microsoft.com/en-us/library/hh217143.aspx
        // HRESULT LocalDBStartInstance( [Input ] PCWSTR pInstanceName, [Input ] DWORD dwFlags,[Output] LPWSTR wszSqlConnection,[Input/Output] LPDWORD lpcchSqlConnection);  
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int LocalDbStartInstance([In] [MarshalAs(UnmanagedType.LPWStr)] string localDbInstanceName, [In] int flags, [Out] [MarshalAs(UnmanagedType.LPWStr)]
            StringBuilder sqlConnectionDataSource, [In] [Out] ref int bufferLength);
    }
}