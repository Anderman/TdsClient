using Medella.TdsClient.TDS.Package;
using Medella.TdsClient.TDS.Reader.StringHelpers;

namespace Medella.TdsClient.TDS.Messages.Server.Environment
{
    public static class ParserEnvCollation
    {
        internal static void EnvCollation(this TdsPackageReader reader)
        {
            var newLength = reader.ReadByte();
            if (newLength == 5)
            {
                var collation = reader.ReadCollation();
                var codepage = collation.GetCodePage();
                reader.CurrentSession.DefaultCollation = collation;
                reader.CurrentSession.DefaultEncoding = reader.CurrentSession.GetEncodingFromCache(codepage);
            }

            var oldLength = reader.ReadByte();
            if (oldLength == 5)
            {
                var oldCollation = reader.ReadCollation();
            }
        }
    }
    public static class ParserEnvSqlTransaction
    {
        internal static long EnvSqlTransaction(this TdsPackageReader reader)
        {
            var newTransactionId = 0L;
            if (reader.ReadByte()== 8) newTransactionId=reader.ReadInt64();
            if (reader.ReadByte()== 8) reader.ReadInt64();
            return newTransactionId;
        }
    }
}