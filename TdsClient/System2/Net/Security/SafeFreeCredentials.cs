using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Medella.TdsClient.System2.Net.Security
{
    public abstract class SafeFreeCredentials : SafeHandle
    {

        internal Interop.SspiCli.CredHandle _handle;    //should be always used as by ref in PInvokes parameters

        protected SafeFreeCredentials() : base(IntPtr.Zero, true)
        {
            _handle = new Interop.SspiCli.CredHandle();
        }

#if TRACE_VERBOSE
        public override string ToString()
        {
            return "0x" + _handle.ToString();
        }
#endif

        public override bool IsInvalid
        {
            get { return IsClosed || _handle.IsZero; }
        }

#if DEBUG
        public new IntPtr DangerousGetHandle()
        {
            Debug.Fail("This method should never be called for this type");
            throw new NotImplementedException();
        }
#endif

        public static unsafe int AcquireCredentialsHandle(
            string package,
            Interop.SspiCli.CredentialUse intent,
            ref Interop.SspiCli.SEC_WINNT_AUTH_IDENTITY_W authdata,
            out SafeFreeCredentials outCredential)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(null, package, intent, authdata);

            int errorCode = -1;
            long timeStamp;

            outCredential = new SafeFreeCredential_SECURITY();

            errorCode = Interop.SspiCli.AcquireCredentialsHandleW(
                null,
                package,
                (int)intent,
                null,
                ref authdata,
                null,
                null,
                ref outCredential._handle,
                out timeStamp);
#if TRACE_VERBOSE
            if (NetEventSource.IsEnabled) NetEventSource.Info(null, $"{nameof(Interop.SspiCli.AcquireCredentialsHandleW)} returns 0x{errorCode:x}, handle:{outCredential}");
#endif

            if (errorCode != 0)
            {
                outCredential.SetHandleAsInvalid();
            }

            return errorCode;
        }

        public static unsafe int AcquireDefaultCredential(
            string package,
            Interop.SspiCli.CredentialUse intent,
            out SafeFreeCredentials outCredential)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(null, package, intent);

            int errorCode = -1;
            long timeStamp;

            outCredential = new SafeFreeCredential_SECURITY();

            errorCode = Interop.SspiCli.AcquireCredentialsHandleW(
                null,
                package,
                (int)intent,
                null,
                IntPtr.Zero,
                null,
                null,
                ref outCredential._handle,
                out timeStamp);

#if TRACE_VERBOSE
            if (NetEventSource.IsEnabled) NetEventSource.Info(null, $"{nameof(Interop.SspiCli.AcquireCredentialsHandleW)} returns 0x{errorCode:x}, handle = {outCredential}");
#endif

            if (errorCode != 0)
            {
                outCredential.SetHandleAsInvalid();
            }

            return errorCode;
        }

        public static unsafe int AcquireCredentialsHandle(
            string package,
            Interop.SspiCli.CredentialUse intent,
            ref SafeSspiAuthDataHandle authdata,
            out SafeFreeCredentials outCredential)
        {
            int errorCode = -1;
            long timeStamp;

            outCredential = new SafeFreeCredential_SECURITY();
            errorCode = Interop.SspiCli.AcquireCredentialsHandleW(
                null,
                package,
                (int)intent,
                null,
                authdata,
                null,
                null,
                ref outCredential._handle,
                out timeStamp);

            if (errorCode != 0)
            {
                outCredential.SetHandleAsInvalid();
            }

            return errorCode;
        }

        public static unsafe int AcquireCredentialsHandle(
            string package,
            Interop.SspiCli.CredentialUse intent,
            ref Interop.SspiCli.SCHANNEL_CRED authdata,
            out SafeFreeCredentials outCredential)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(null, package, intent, authdata);

            int errorCode = -1;
            long timeStamp;


            // If there is a certificate, wrap it into an array.
            // Not threadsafe.
            IntPtr copiedPtr = authdata.paCred;
            try
            {
                IntPtr certArrayPtr = new IntPtr(&copiedPtr);
                if (copiedPtr != IntPtr.Zero)
                {
                    authdata.paCred = certArrayPtr;
                }

                outCredential = new SafeFreeCredential_SECURITY();

                errorCode = Interop.SspiCli.AcquireCredentialsHandleW(
                    null,
                    package,
                    (int)intent,
                    null,
                    ref authdata,
                    null,
                    null,
                    ref outCredential._handle,
                    out timeStamp);
            }
            finally
            {
                authdata.paCred = copiedPtr;
            }

#if TRACE_VERBOSE
            if (NetEventSource.IsEnabled) NetEventSource.Info(null, $"{nameof(Interop.SspiCli.AcquireCredentialsHandleW)} returns 0x{errorCode:x}, handle = {outCredential}");
#endif

            if (errorCode != 0)
            {
                outCredential.SetHandleAsInvalid();
            }

            return errorCode;
        }
    }
}