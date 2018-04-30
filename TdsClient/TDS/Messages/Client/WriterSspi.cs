using Medella.TdsClient.Contants;
using Medella.TdsClient.TDS.Package;

namespace Medella.TdsClient.TDS.Messages.Client
{
    public static class WriterSspi
    {
        public static void SendSspi(this TdsPackageWriter writer, byte[] sspiData)
        {
            writer.WriteByteArray(sspiData, sspiData.Length);
            writer.SetHeader(TdsEnums.ST_EOM, TdsEnums.MT_SSPI);
            writer.FlushBuffer();
        }
    }
}