using System;
using System.Runtime.InteropServices;

namespace System.Runtime.InteropServices
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct SecPkgContext_NegotiationInfoW
    {
        internal IntPtr PackageInfo;
        internal uint NegotiationState;
        internal static readonly int Size = Marshal.SizeOf<SecPkgContext_NegotiationInfoW>();
        internal static readonly int NegotiationStateOffest = (int)Marshal.OffsetOf<SecPkgContext_NegotiationInfoW>("NegotiationState");
    }
}