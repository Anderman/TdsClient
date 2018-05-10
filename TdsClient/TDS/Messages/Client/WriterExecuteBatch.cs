using Medella.TdsClient.Contants;
using Medella.TdsClient.TDS.Package;

namespace Medella.TdsClient.TDS.Messages.Client
{
    public static class WriterExecuteBatch
    {
        public static void SendExcuteBatch(this TdsPackageWriter tdsPackageWriter, string text)
        {
            tdsPackageWriter.NewPackage(TdsEnums.MT_SQL);

            tdsPackageWriter.WriteRpcBatchHeaders();

            tdsPackageWriter.WriteString(text);
            tdsPackageWriter.SendLastMessage();
        }
    }
}