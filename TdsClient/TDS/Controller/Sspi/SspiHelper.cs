using System;
using System.Text;
using Medella.TdsClient.Contants;
using Medella.TdsClient.System2.Net.Security;

namespace Medella.TdsClient.TDS.Controller.Sspi
{
    public class SspiHelper
    {
        private readonly string _serverSpn;
        private SafeFreeCredentials _credentialsHandle;
        private SafeDeleteContext _securityContext;
        private ContextFlagsPal _contextFlags;
        public byte[] ClientToken;

        public SspiHelper(string serverSpn)
        {
            _serverSpn = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(serverSpn));
            CreateClientToken(null);
        }
        public void CreateClientToken(byte[] serverToken)
        {
            // if don't have SSPI data from the server, send null for the byte[] buffer 

            var securityContext = _securityContext;
            var contextFlags = _contextFlags;
            var credentialsHandle = _credentialsHandle;

            const string securityPackage = NegotiationInfoClass.Negotiate;

            if (securityContext == null) credentialsHandle = NegotiateStreamPal.AcquireDefaultCredential(securityPackage, false);

            var inSecurityBufferArray = serverToken == null
                ? new SecurityBuffer[] { }
                : new[] { new SecurityBuffer(serverToken, SecurityBufferType.SECBUFFER_TOKEN) };

            var tokenSize = NegotiateStreamPal.QueryMaxTokenSize(securityPackage);
            var outSecurityBuffer = new SecurityBuffer(tokenSize, SecurityBufferType.SECBUFFER_TOKEN);

            const ContextFlagsPal requestedContextFlags = ContextFlagsPal.Connection
                                                          | ContextFlagsPal.Confidentiality
                                                          | ContextFlagsPal.Delegate
                                                          | ContextFlagsPal.MutualAuth;

            var statusCode = NegotiateStreamPal.InitializeSecurityContext(
                credentialsHandle,
                ref securityContext,
                _serverSpn,
                requestedContextFlags,
                inSecurityBufferArray,
                outSecurityBuffer,
                ref contextFlags);

            if (statusCode.ErrorCode == SecurityStatusPalErrorCode.CompleteNeeded || statusCode.ErrorCode == SecurityStatusPalErrorCode.CompAndContinue)
            {
                inSecurityBufferArray = new[] { outSecurityBuffer };
                statusCode = NegotiateStreamPal.CompleteAuthToken(ref securityContext, inSecurityBufferArray);
                outSecurityBuffer.token = null;
            }
            if (IsErrorStatus(statusCode.ErrorCode))
            {
                if (statusCode.ErrorCode == SecurityStatusPalErrorCode.InternalError)
                    throw new InvalidOperationException(SQLMessage.KerberosTicketMissingError() + "\n" + statusCode);
                throw new InvalidOperationException(SQLMessage.SSPIGenerateError() + "\n" + statusCode);
            }

            _securityContext = securityContext;
            _contextFlags = contextFlags;
            _credentialsHandle = credentialsHandle;
            ClientToken = outSecurityBuffer.token ?? Array.Empty<byte>();
        }
        private static bool IsErrorStatus(SecurityStatusPalErrorCode errorCode)
        {
            return errorCode != SecurityStatusPalErrorCode.NotSet &&
                   errorCode != SecurityStatusPalErrorCode.OK &&
                   errorCode != SecurityStatusPalErrorCode.ContinueNeeded &&
                   errorCode != SecurityStatusPalErrorCode.CompleteNeeded &&
                   errorCode != SecurityStatusPalErrorCode.CompAndContinue &&
                   errorCode != SecurityStatusPalErrorCode.ContextExpired &&
                   errorCode != SecurityStatusPalErrorCode.CredentialsNeeded &&
                   errorCode != SecurityStatusPalErrorCode.Renegotiate;
        }
    }
}