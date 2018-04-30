using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Medella.TdsClient.System2.Net.Security
{
    internal abstract class SafeFreeContextBuffer : SafeHandleZeroOrMinusOneIsInvalid
    {
        protected SafeFreeContextBuffer() : base(true) { }

        // This must be ONLY called from this file.
        internal void Set(IntPtr value)
        {
            this.handle = value;
        }

        internal static int EnumeratePackages(out int pkgnum, out SafeFreeContextBuffer pkgArray)
        {
            int res = -1;
            SafeFreeContextBuffer_SECURITY pkgArray_SECURITY = null;
            res = Interop.SspiCli.EnumerateSecurityPackagesW(out pkgnum, out pkgArray_SECURITY);
            pkgArray = pkgArray_SECURITY;

            if (res != 0 && pkgArray != null)
            {
                pkgArray.SetHandleAsInvalid();
            }

            return res;
        }

        internal static SafeFreeContextBuffer CreateEmptyHandle()
        {
            return new SafeFreeContextBuffer_SECURITY();
        }

        //
        // After PInvoke call the method will fix the refHandle.handle with the returned value.
        // The caller is responsible for creating a correct SafeHandle template or null can be passed if no handle is returned.
        //
        // This method switches between three non-interruptible helper methods.  (This method can't be both non-interruptible and
        // reference imports from all three DLLs - doing so would cause all three DLLs to try to be bound to.)
        //
        public static unsafe int QueryContextAttributes(SafeDeleteContext phContext, Interop.SspiCli.ContextAttribute contextAttribute, byte* buffer, SafeHandle refHandle)
        {
            return QueryContextAttributes_SECURITY(phContext, contextAttribute, buffer, refHandle);
        }

        private static unsafe int QueryContextAttributes_SECURITY(
            SafeDeleteContext phContext,
            Interop.SspiCli.ContextAttribute contextAttribute,
            byte* buffer,
            SafeHandle refHandle)
        {
            int status = (int)Interop.SECURITY_STATUS.InvalidHandle;

            try
            {
                bool ignore = false;
                phContext.DangerousAddRef(ref ignore);
                status = Interop.SspiCli.QueryContextAttributesW(ref phContext._handle, contextAttribute, buffer);
            }
            finally
            {
                phContext.DangerousRelease();
            }

            if (status == 0 && refHandle != null)
            {
                if (refHandle is SafeFreeContextBuffer)
                {
                    ((SafeFreeContextBuffer)refHandle).Set(*(IntPtr*)buffer);
                }
                else
                {
                    ((SafeFreeCertContext)refHandle).Set(*(IntPtr*)buffer);
                }
            }

            if (status != 0 && refHandle != null)
            {
                refHandle.SetHandleAsInvalid();
            }

            return status;
        }

        public static int SetContextAttributes(
            SafeDeleteContext phContext,
            Interop.SspiCli.ContextAttribute contextAttribute, byte[] buffer)
        {
            return SetContextAttributes_SECURITY(phContext, contextAttribute, buffer);
        }

        private static int SetContextAttributes_SECURITY(
            SafeDeleteContext phContext,
            Interop.SspiCli.ContextAttribute contextAttribute,
            byte[] buffer)
        {
            try
            {
                bool ignore = false;
                phContext.DangerousAddRef(ref ignore);
                return Interop.SspiCli.SetContextAttributesW(ref phContext._handle, contextAttribute, buffer, buffer.Length);
            }
            finally
            {
                phContext.DangerousRelease();
            }
        }
    }
}