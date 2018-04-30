using System;
using System.Runtime.InteropServices;

namespace Medella.TdsClient.System2.Net.Security
{
    internal interface SSPIInterface
    {
        SecurityPackageInfoClass[] SecurityPackages { get; set; }
        int EnumerateSecurityPackages(out int pkgnum, out SafeFreeContextBuffer pkgArray);
        int AcquireCredentialsHandle(string moduleName, Interop.SspiCli.CredentialUse usage, ref Interop.SspiCli.SEC_WINNT_AUTH_IDENTITY_W authdata, out SafeFreeCredentials outCredential);
        int AcquireCredentialsHandle(string moduleName, Interop.SspiCli.CredentialUse usage, ref SafeSspiAuthDataHandle authdata, out SafeFreeCredentials outCredential);
        int AcquireDefaultCredential(string moduleName, Interop.SspiCli.CredentialUse usage, out SafeFreeCredentials outCredential);
        int AcquireCredentialsHandle(string moduleName, Interop.SspiCli.CredentialUse usage, ref Interop.SspiCli.SCHANNEL_CRED authdata, out SafeFreeCredentials outCredential);
        int AcceptSecurityContext(ref SafeFreeCredentials credential, ref SafeDeleteContext context, SecurityBuffer inputBuffer, Interop.SspiCli.ContextFlags inFlags, Interop.SspiCli.Endianness endianness, SecurityBuffer outputBuffer, ref Interop.SspiCli.ContextFlags outFlags);
        int AcceptSecurityContext(SafeFreeCredentials credential, ref SafeDeleteContext context, SecurityBuffer[] inputBuffers, Interop.SspiCli.ContextFlags inFlags, Interop.SspiCli.Endianness endianness, SecurityBuffer outputBuffer, ref Interop.SspiCli.ContextFlags outFlags);
        int InitializeSecurityContext(ref SafeFreeCredentials credential, ref SafeDeleteContext context, string targetName, Interop.SspiCli.ContextFlags inFlags, Interop.SspiCli.Endianness endianness, SecurityBuffer inputBuffer, SecurityBuffer outputBuffer, ref Interop.SspiCli.ContextFlags outFlags);
        int InitializeSecurityContext(SafeFreeCredentials credential, ref SafeDeleteContext context, string targetName, Interop.SspiCli.ContextFlags inFlags, Interop.SspiCli.Endianness endianness, SecurityBuffer[] inputBuffers, SecurityBuffer outputBuffer, ref Interop.SspiCli.ContextFlags outFlags);
        int EncryptMessage(SafeDeleteContext context, ref Interop.SspiCli.SecBufferDesc inputOutput, uint sequenceNumber);
        int DecryptMessage(SafeDeleteContext context, ref Interop.SspiCli.SecBufferDesc inputOutput, uint sequenceNumber);
        int MakeSignature(SafeDeleteContext context, ref Interop.SspiCli.SecBufferDesc inputOutput, uint sequenceNumber);
        int VerifySignature(SafeDeleteContext context, ref Interop.SspiCli.SecBufferDesc inputOutput, uint sequenceNumber);

        int QueryContextChannelBinding(SafeDeleteContext phContext, Interop.SspiCli.ContextAttribute attribute, out SafeFreeContextBufferChannelBinding refHandle);
        int QueryContextAttributes(SafeDeleteContext phContext, Interop.SspiCli.ContextAttribute attribute, byte[] buffer, Type handleType, out SafeHandle refHandle);
        int SetContextAttributes(SafeDeleteContext phContext, Interop.SspiCli.ContextAttribute attribute, byte[] buffer);
        int QuerySecurityContextToken(SafeDeleteContext phContext, out SecurityContextTokenHandle phToken);
        int CompleteAuthToken(ref SafeDeleteContext refContext, SecurityBuffer[] inputBuffers);
        int ApplyControlToken(ref SafeDeleteContext refContext, SecurityBuffer[] inputBuffers);
    }
}