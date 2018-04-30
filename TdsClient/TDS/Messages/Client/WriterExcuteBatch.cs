using Medella.TdsClient.Contants;
using Medella.TdsClient.TDS.Package;

namespace Medella.TdsClient.TDS.Messages.Client
{
    public class WriterExcuteBatch
    {
        public static void Create(TdsPackageWriter tdsPackageWriter, string text)
        {
            tdsPackageWriter.NewPackage();

            WriteRpcBatchHeaders(tdsPackageWriter);

            tdsPackageWriter.WriteString(text);
            tdsPackageWriter.SetHeader(TdsEnums.ST_EOM, TdsEnums.MT_SQL);
            tdsPackageWriter.FlushBuffer();
        }

        private static void WriteRpcBatchHeaders(TdsPackageWriter tdsPackageWriter)
        {
            /* Header:
               TotalLength  - DWORD  - including all headers and lengths, including itself
               Each Data Session:
               {
                     HeaderLength - DWORD  - including all header length fields, including itself
                     HeaderType   - USHORT
                     HeaderData
               }
            */

            const int notificationHeaderSize = 0;

            const int marsHeaderSize = 18; // 4 + 2 + 8 + 4

            var totalHeaderLength = 4 + marsHeaderSize + notificationHeaderSize;
            // Write total header length
            tdsPackageWriter.WriteInt32(totalHeaderLength);

            // Write Mars header length
            tdsPackageWriter.WriteInt32(marsHeaderSize);
            // Write Mars header data
            WriteMarsHeaderData(tdsPackageWriter);
        }
        private static void WriteMarsHeaderData(TdsPackageWriter tdsPackageWriter) //transactions not implemented
        {
            tdsPackageWriter.WriteInt16(TdsEnums.HEADERTYPE_MARS);
                tdsPackageWriter.WriteLong(0);
                tdsPackageWriter.WriteInt32(1);
        }


    }
}