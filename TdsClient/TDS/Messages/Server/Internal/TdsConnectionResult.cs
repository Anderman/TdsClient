using Medella.TdsClient.TDS.Controller;

namespace Medella.TdsClient.TDS.Messages.Server.Internal
{
    public class TdsConnectionResult
    {
        public EncryptionOptions EncryptionOption = EncryptionOptions.OFF;
        public int Version { get; set; }
        public bool IsYukonOrLater => Version >= 9;
        public bool IsMarsCapable { get; set; }
    }
}