using System.Threading.Tasks;
using Medella.TdsClient.Constants;
using Medella.TdsClient.TDS.Package.Writer;

namespace Medella.TdsClient.TDS.Messages.Client
{
    public static class WriterExecuteBatch
    {
        public static async Task SendExecuteBatchAsync(this TdsPackageWriter tdsPackageWriter, string text, long sqlConnectionId)
        {
            await Task.Run(() => SendExecuteBatch(tdsPackageWriter, text, sqlConnectionId));
        }

        public static void SendExecuteBatch(this TdsPackageWriter tdsPackageWriter, string text, long sqlConnectionId)
        {
            tdsPackageWriter.NewPackage(TdsEnums.MT_SQL);

            tdsPackageWriter.WriteMarsHeader(sqlConnectionId);

            tdsPackageWriter.WriteUnicodeString(text);
            tdsPackageWriter.SendLastMessage();
        }
    }
}