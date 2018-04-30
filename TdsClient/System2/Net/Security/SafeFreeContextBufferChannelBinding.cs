using System;
using System.Runtime.InteropServices;
using System.Security.Authentication.ExtendedProtection;

namespace Medella.TdsClient.System2.Net.Security
{
    internal abstract class SafeFreeContextBufferChannelBinding : ChannelBinding
    {
        private int _size;

        public override int Size
        {
            get { return _size; }
        }

        public override bool IsInvalid
        {
            get { return handle == new IntPtr(0) || handle == new IntPtr(-1); }
        }

        internal unsafe void Set(IntPtr value)
        {
            this.handle = value;
        }

        internal static SafeFreeContextBufferChannelBinding CreateEmptyHandle()
        {
            return new SafeFreeContextBufferChannelBinding_SECURITY();
        }

        public static unsafe int QueryContextChannelBinding(SafeDeleteContext phContext, Interop.SspiCli.ContextAttribute contextAttribute, SecPkgContext_Bindings* buffer, SafeFreeContextBufferChannelBinding refHandle)
        {
            return QueryContextChannelBinding_SECURITY(phContext, contextAttribute, buffer, refHandle);
        }

        private static unsafe int QueryContextChannelBinding_SECURITY(SafeDeleteContext phContext, Interop.SspiCli.ContextAttribute contextAttribute, SecPkgContext_Bindings* buffer, SafeFreeContextBufferChannelBinding refHandle)
        {
            int status = (int)Interop.SECURITY_STATUS.InvalidHandle;

            // SCHANNEL only supports SECPKG_ATTR_ENDPOINT_BINDINGS and SECPKG_ATTR_UNIQUE_BINDINGS which
            // map to our enum ChannelBindingKind.Endpoint and ChannelBindingKind.Unique.
            if (contextAttribute != Interop.SspiCli.ContextAttribute.SECPKG_ATTR_ENDPOINT_BINDINGS &&
                contextAttribute != Interop.SspiCli.ContextAttribute.SECPKG_ATTR_UNIQUE_BINDINGS)
            {
                return status;
            }

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
                refHandle.Set((*buffer).Bindings);
                refHandle._size = (*buffer).BindingsLength;
            }

            if (status != 0 && refHandle != null)
            {
                refHandle.SetHandleAsInvalid();
            }

            return status;
        }

        public override string ToString()
        {
            if (IsInvalid)
            {
                return null;
            }

            var bytes = new byte[_size];
            Marshal.Copy(handle, bytes, 0, bytes.Length);
            return BitConverter.ToString(bytes).Replace('-', ' ');
        }
    }
}