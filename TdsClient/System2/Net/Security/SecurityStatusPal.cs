using System;

namespace Medella.TdsClient.System2.Net.Security
{
    public struct SecurityStatusPal
    {
        public readonly SecurityStatusPalErrorCode ErrorCode;
        public readonly Exception Exception;

        public SecurityStatusPal(SecurityStatusPalErrorCode errorCode, Exception exception = null)
        {
            ErrorCode = errorCode;
            Exception = exception;
        }

        public override string ToString()
        {
            return Exception == null ?
                $"{nameof(ErrorCode)}={ErrorCode}" :
                $"{nameof(ErrorCode)}={ErrorCode}, {nameof(Exception)}={Exception}";
        }
    }
}