using System;
using System.Runtime.InteropServices;

namespace Medella.TdsClient.System2.Net.Security
{
    public partial class NegotiationInfoClass
    {
        internal const string NTLM = "NTLM";
        internal const string Kerberos = "Kerberos";
        public const string Negotiate = "Negotiate";
        internal const string Basic = "Basic";
    }

    public partial class NegotiationInfoClass
    {
        internal string AuthenticationPackage;

        internal NegotiationInfoClass(SafeHandle safeHandle, int negotiationState)
        {
            if (safeHandle.IsInvalid)
            {
                if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"Invalid handle:{safeHandle}");
                return;
            }

            IntPtr packageInfo = safeHandle.DangerousGetHandle();
            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"packageInfo:{packageInfo} negotiationState:{negotiationState:x}");

            if (negotiationState == Interop.SspiCli.SECPKG_NEGOTIATION_COMPLETE
                || negotiationState == Interop.SspiCli.SECPKG_NEGOTIATION_OPTIMISTIC)
            {
                IntPtr unmanagedString = Marshal.ReadIntPtr(packageInfo, SecurityPackageInfo.NameOffest);
                string name = null;
                if (unmanagedString != IntPtr.Zero)
                {
                    name = Marshal.PtrToStringUni(unmanagedString);
                }

                if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"packageInfo:{packageInfo} negotiationState:{negotiationState:x} name:{name}");

                // An optimization for future string comparisons.
                if (string.Compare(name, Kerberos, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    AuthenticationPackage = Kerberos;
                }
                else if (string.Compare(name, NTLM, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    AuthenticationPackage = NTLM;
                }
                else
                {
                    AuthenticationPackage = name;
                }
            }
        }
    }
}