using Medella.TdsClient.Constants;
using Medella.TdsClient.TDS.Package.Writer;

namespace Medella.TdsClient.TDS.Messages.Client
{
    public static class WriterTransaction
    {
        public static void SendTransactionBegin(this TdsPackageWriter writer, TdsEnums.TransactionManagerIsolationLevel isoLevel)
        {
            var transactionName = "";
            WriteTransactionHeader(writer, TdsEnums.TransactionManagerRequestType.Begin, 0);
            writer.WriteByte((byte)isoLevel);
            writer.WriteByte((byte)(transactionName.Length * 2)); // Write number of bytes (unicode string).
            writer.WriteUnicodeString(transactionName);
            writer.SendLastMessage();
        }

        public static void SendTransactionCommit(this TdsPackageWriter writer, long sqlTransactionId)
        {
            WriteTransactionHeader(writer, TdsEnums.TransactionManagerRequestType.Commit, sqlTransactionId);
            writer.WriteByte(0); // No xact name
            writer.WriteByte(0); // No flags
            writer.SendLastMessage();
        }

        public static void SendTransactionRollback(this TdsPackageWriter writer, long sqlTransactionId)
        {
            var transactionName = "";
            WriteTransactionHeader(writer, TdsEnums.TransactionManagerRequestType.Rollback, sqlTransactionId);

            writer.WriteByte((byte)(transactionName.Length * 2)); // Write number of bytes (unicode string).
            writer.WriteUnicodeString(transactionName);
            writer.WriteByte(0); // No flags
            writer.SendLastMessage();
        }

        private static void WriteTransactionHeader(TdsPackageWriter writer, TdsEnums.TransactionManagerRequestType request, long sqlTransactionId)
        {
            writer.NewPackage(TdsEnums.MT_TRANS);
            writer.WriteMarsHeader(sqlTransactionId);

            writer.WriteInt16((short)request); // write TransactionManager Request type
        }
    }
}