using Medella.TdsClient.Contants;
using Medella.TdsClient.TDS.Package;

namespace Medella.TdsClient.TDS.Messages.Client
{
    public static class WriterExecuteBatch
    {
        public static void SendExcuteBatch(this TdsPackageWriter tdsPackageWriter, string text)
        {
            tdsPackageWriter.NewPackage();

            tdsPackageWriter.WriteRpcBatchHeaders();

            tdsPackageWriter.WriteString(text);
            tdsPackageWriter.SetHeader(TdsEnums.ST_EOM, TdsEnums.MT_SQL);
            tdsPackageWriter.FlushBuffer();
        }
    }
}