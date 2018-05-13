using Medella.TdsClient.Contants;
using Medella.TdsClient.TDS.Package;

namespace Medella.TdsClient.TDS.Messages.Client
{
    public static class WriterTransaction
    {
        public static void SendTransactionBegin(this TdsPackageWriter writer, TdsEnums.TransactionManagerIsolationLevel isoLevel)
        {
            var transactionname = "";
            WriteTransactionHeader(writer, TdsEnums.TransactionManagerRequestType.Begin, 0);
            writer.WriteByte((byte) isoLevel);
            writer.WriteByte((byte) (transactionname.Length * 2)); // Write number of bytes (unicode string).
            writer.WriteUnicodeString(transactionname);
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
            var transactionname = "";
            WriteTransactionHeader(writer, TdsEnums.TransactionManagerRequestType.Rollback, sqlTransactionId);

            writer.WriteByte((byte) (transactionname.Length * 2)); // Write number of bytes (unicode string).
            writer.WriteUnicodeString(transactionname);
            writer.WriteByte(0); // No flags
            writer.SendLastMessage();
        }

        private static void WriteTransactionHeader(TdsPackageWriter writer, TdsEnums.TransactionManagerRequestType request, long sqlTransactionId)
        {
            writer.NewPackage(TdsEnums.MT_TRANS);

            const int marsHeaderSize = 18; // 4 + (2 + 8 + 4)= size + data
            const int totalHeaderLength = 22; // 4 + (4 + 2 + 8 + 4) size+mars
            writer.WriteInt32(totalHeaderLength);
            writer.WriteInt32(marsHeaderSize);
            WriteMarsHeaderData(writer, sqlTransactionId);

            writer.WriteInt16((short) request); // write TransactionManager Request type
        }

        private static void WriteMarsHeaderData(this TdsPackageWriter writer, long sqlTransactionId)
        {
            writer.WriteInt16(TdsEnums.HEADERTYPE_MARS);
            writer.WriteInt64(sqlTransactionId);
            writer.WriteInt32(1);
        }
    }
}