using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using Medella.TdsClient.Cleanup;
using Medella.TdsClient.Contants;
using Medella.TdsClient.Exceptions;
using Medella.TdsClient.SNI.Native;
using Medella.TdsClient.System2;
using Medella.TdsClient.TDS;

namespace Medella.TdsClient.LocalDb
{
    internal static partial class LocalDBAPI
    {
        private const string const_localDbPrefix = @"(localdb)\";

        // check if name is in format (localdb)\<InstanceName - not empty> and return instance name if it is
        internal static string GetLocalDbInstanceNameFromServerName(string serverName)
        {
            if (serverName == null)
                return null;
            serverName = serverName.TrimStart(); // it can start with spaces if specified in quotes
            if (!serverName.StartsWith(const_localDbPrefix, StringComparison.OrdinalIgnoreCase))
                return null;
            var instanceName = serverName.Substring(const_localDbPrefix.Length).Trim();
            if (instanceName.Length == 0)
                return null;
            return instanceName;
        }


        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private delegate int LocalDBFormatMessageDelegate(int hrLocalDB, uint dwFlags, uint dwLanguageId, StringBuilder buffer, ref uint buflen);
    }

    internal static partial class LocalDBAPI
    {
        private static IntPtr UserInstanceDLLHandle
        {
            get
            {
                if (s_userInstanceDLLHandle == IntPtr.Zero)
                    lock (s_dllLock)
                    {
                        if (s_userInstanceDLLHandle == IntPtr.Zero)
                        {
                            SniNativeMethodWrapper.SNIQueryInfo(SniNativeMethodWrapper.QTypes.SNI_QUERY_LOCALDB_HMODULE, ref s_userInstanceDLLHandle);
                            if (s_userInstanceDLLHandle == IntPtr.Zero)
                            {
                                SniNativeMethodWrapper.SNI_Error sniError;
                                SniNativeMethodWrapper.SNIGetLastError(out sniError);
                                throw CreateLocalDBException(SR.GetString("LocalDB_FailedGetDLLHandle"), sniError: (int) sniError.sniError);
                            }
                        }
                    }

                return s_userInstanceDLLHandle;
            }
        }

        private static IntPtr LoadProcAddress()
        {
            return SafeNativeMethods.GetProcAddress(UserInstanceDLLHandle, "LocalDBFormatMessage");
        }
    }

    internal static partial class LocalDBAPI
    {
        private const uint const_LOCALDB_TRUNCATE_ERR_MESSAGE = 1; // flag for LocalDBFormatMessage that indicates that message can be truncated if it does not fit in the buffer
        private const int const_ErrorMessageBufferSize = 1024; // Buffer size for Local DB error message 1K will be enough for all messages
        private static LocalDBFormatMessageDelegate s_localDBFormatMessage;

        //This is copy of handle that SNI maintains, so we are responsible for freeing it - therefore there we are not using SafeHandle
        private static IntPtr s_userInstanceDLLHandle = IntPtr.Zero;

        private static readonly object s_dllLock = new object();


        private static LocalDBFormatMessageDelegate LocalDBFormatMessage
        {
            get
            {
                if (s_localDBFormatMessage == null)
                    lock (s_dllLock)
                    {
                        if (s_localDBFormatMessage == null)
                        {
                            var functionAddr = LoadProcAddress();

                            if (functionAddr == IntPtr.Zero)
                            {
                                var hResult = Marshal.GetLastWin32Error();
                                throw CreateLocalDBException(Strings.LocalDB_MethodNotFound);
                            }

                            s_localDBFormatMessage = Marshal.GetDelegateForFunctionPointer<LocalDBFormatMessageDelegate>(functionAddr);
                        }
                    }

                return s_localDBFormatMessage;
            }
        }

        internal static string GetLocalDBMessage(int hrCode)
        {
            Debug.Assert(hrCode < 0, "HRCode does not indicate error");
            try
            {
                var buffer = new StringBuilder(const_ErrorMessageBufferSize);
                var len = (uint) buffer.Capacity;


                // First try for current culture                
                var hResult = LocalDBFormatMessage(hrCode, const_LOCALDB_TRUNCATE_ERR_MESSAGE, (uint) CultureInfo.CurrentCulture.LCID,
                    buffer, ref len);
                if (hResult >= 0) return buffer.ToString();

                // Message is not available for current culture, try default 
                buffer = new StringBuilder(const_ErrorMessageBufferSize);
                len = (uint) buffer.Capacity;
                hResult = LocalDBFormatMessage(hrCode, const_LOCALDB_TRUNCATE_ERR_MESSAGE, 0 /* thread locale with fallback to English */,
                    buffer, ref len);
                if (hResult >= 0)
                    return buffer.ToString();
                return string.Format(CultureInfo.CurrentCulture, "{0} (0x{1:X}).", Strings.LocalDB_UnobtainableMessage, hResult);
            }
            catch (SqlException exc)
            {
                return string.Format(CultureInfo.CurrentCulture, "{0} ({1}).", Strings.LocalDB_UnobtainableMessage, exc.Message);
            }
        }


        private static SqlException CreateLocalDBException(string errorMessage, string instance = null, int localDbError = 0, int sniError = 0)
        {
            Debug.Assert(localDbError == 0 || sniError == 0, "LocalDB error and SNI error cannot be specified simultaneously");
            Debug.Assert(!string.IsNullOrEmpty(errorMessage), "Error message should not be null or empty");
            var collection = new SqlErrorCollection();

            var errorCode = localDbError == 0 ? sniError : localDbError;

            if (sniError != 0)
            {
                var sniErrorMessage = SQL.GetSNIErrorMessage(sniError);
                errorMessage = string.Format(null, "{0} (error: {1} - {2})",
                    errorMessage, sniError, sniErrorMessage);
            }

            collection.Add(new SqlInfoAndError {Number = errorCode, Class = TdsEnums.FATAL_ERROR_CLASS, Server = instance, Message = errorMessage});

            if (localDbError != 0)
                collection.Add(new SqlInfoAndError {Number = errorCode, Class = TdsEnums.FATAL_ERROR_CLASS, Server = instance, Message = GetLocalDBMessage(localDbError)});

            var exc = SqlException.CreateException(collection, null);

            exc._doNotReconnect = true;

            return exc;
        }
    }
}