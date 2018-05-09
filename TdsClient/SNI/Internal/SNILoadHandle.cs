using System.Threading;
using Medella.TdsClient.Contants;
using Medella.TdsClient.Exceptions;

namespace Medella.TdsClient.SNI.Internal
{
    /// <summary>
    ///     Global SNI settings and status
    /// </summary>
    internal class SniLoadHandle
    {
        public static readonly SniLoadHandle SingletonInstance = new SniLoadHandle();

        public readonly EncryptionOptions EncryptionOption = EncryptionOptions.OFF;

        public ThreadLocal<SniError> LastSniError = new ThreadLocal<SniError>(() => new SniError(SniProviders.INVALID_PROV, 0, TdsEnums.SNI_SUCCESS, string.Empty));

        /// <summary>
        ///     Last SNI error
        /// </summary>
        public SniError LastError
        {
            get => LastSniError.Value;

            set => LastSniError.Value = value;
        }
    }
}