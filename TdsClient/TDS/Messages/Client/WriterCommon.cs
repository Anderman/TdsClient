using Medella.TdsClient.TDS.Package;
using Medella.TdsClient.TDS.Reader.StringHelpers;

namespace Medella.TdsClient.TDS.Messages.Client
{
    public static class WriterCommon
    {
        public static void WriteIdentifier(this TdsPackageWriter writer, string s)
        {
            if (null == s)
            {
                writer.WriteByte(0);
            }
            else
            {
                writer.WriteByte(checked((byte) s.Length));
                writer.WriteString(s);
            }
        }

        public static void WriteCollation(this TdsPackageWriter writer, SqlCollations collation)
        {
            if (collation == null)
            {
                writer.WriteByte(0);
            }
            else
            {
                writer.WriteByte(sizeof(uint) + sizeof(byte));
                writer.WriteUInt32(collation.Info);
                writer.WriteByte(collation.SortId);
            }
        }
    }
}