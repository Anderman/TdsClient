using System;

namespace Medella.TdsClient.SNI.Internal
{
    public class SniError
    {
        public readonly uint Error;
        public readonly string ErrorMessage;
        public readonly Exception Exception;
        public readonly string Function;
        public readonly uint LineNumber;
        public readonly uint NativeError;
        public readonly SniProviders Provider;

        public SniError(SniProviders provider, uint nativeError, uint sniErrorCode, string errorMessage)
        {
            LineNumber = 0;
            Function = string.Empty;
            Provider = provider;
            NativeError = nativeError;
            Error = sniErrorCode;
            ErrorMessage = errorMessage;
            Exception = null;
        }

        public SniError(SniProviders provider, uint sniErrorCode, Exception sniException)
        {
            LineNumber = 0;
            Function = string.Empty;
            Provider = provider;
            NativeError = 0;
            Error = sniErrorCode;
            ErrorMessage = string.Empty;
            Exception = sniException;
        }
    }
}