using System.Threading.Tasks;
using Medella.TdsClient.Contants;
using Medella.TdsClient.TDS.Package;
using TdsPackageWriter = Medella.TdsClient.TDS.Package.Writer.TdsPackageWriter;

namespace Medella.TdsClient.TDS.Messages.Client
{
    public static class WriterExecuteBatch
    {
        public static async Task SendExcuteBatchAsync(this TdsPackageWriter tdsPackageWriter, string text, long sqlConnectionId)
        {
            await Task.Run(() => SendExcuteBatch(tdsPackageWriter, text, sqlConnectionId));
        }

        public static void SendExcuteBatch(this TdsPackageWriter tdsPackageWriter, string text, long sqlConnectionId)
        {
            tdsPackageWriter.NewPackage(TdsEnums.MT_SQL);

            tdsPackageWriter.WriteMarsHeader(sqlConnectionId);

            tdsPackageWriter.WriteUnicodeString(text);
            tdsPackageWriter.SendLastMessage();
        }
    }
}