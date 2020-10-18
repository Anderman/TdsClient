using Medella.TdsClient.Constants;
using Medella.TdsClient.TDS.Package.Writer;

namespace Medella.TdsClient.TDS.Messages.Client
{
    public static class WriterSspi
    {
        public static void SendSspi(this TdsPackageWriter writer, byte[] sspiData)
        {
            writer.NewPackage(TdsEnums.MT_SSPI);
            writer.WriteByteArray(sspiData);
            writer.SendLastMessage();
        }
    }
}