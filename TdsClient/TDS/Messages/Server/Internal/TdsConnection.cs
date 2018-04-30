using Medella.TdsClient.Exceptions;

namespace Medella.TdsClient.TDS
{
    public class TdsConnection
    {
        public int Version { get; set; }
        public bool IsYukonOrLater => Version >= 9;
        public bool IsMarsCapable { get; set; }
        public EncryptionOptions EncryptionOption = EncryptionOptions.OFF;
    }
}