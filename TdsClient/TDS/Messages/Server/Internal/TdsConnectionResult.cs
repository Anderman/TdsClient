using Medella.TdsClient.Exceptions;

namespace Medella.TdsClient.TDS.Messages.Server.Internal
{
    public class TdsConnectionResult
    {
        public int Version { get; set; }
        public bool IsYukonOrLater => Version >= 9;
        public bool IsMarsCapable { get; set; }
        public EncryptionOptions EncryptionOption = EncryptionOptions.OFF;
    }
}