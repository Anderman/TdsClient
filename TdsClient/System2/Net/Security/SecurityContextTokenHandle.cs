using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace Medella.TdsClient.System2.Net.Security
{
    internal sealed class SecurityContextTokenHandle : CriticalHandleZeroOrMinusOneIsInvalid
    {
        private int _disposed;

        private SecurityContextTokenHandle() : base()
        {
        }

        internal IntPtr DangerousGetHandle()
        {
            return handle;
        }

        protected override bool ReleaseHandle()
        {
            if (!IsInvalid)
            {
                if (Interlocked.Increment(ref _disposed) == 1)
                {
                    return Interop.Kernel32.CloseHandle(handle);
                }
            }
            return true;
        }
    }
}