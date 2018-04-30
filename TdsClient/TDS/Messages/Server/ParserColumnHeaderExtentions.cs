using Medella.TdsClient.Contants;
using Medella.TdsClient.TDS.Package;

namespace Medella.TdsClient.TDS.Messages.Server
{
    public static class ParserColumnHeaderExtentions
    {
        public static ulong? ReadBlobLength(this TdsPackageReader reader, byte tdsType)
        {
            //
            // we don't care about TextPtrs, simply go after the data after it
            //
            var textPtrLen = reader.ReadByte();

            if (textPtrLen == 0)
                return null;

            reader.GetBytes(textPtrLen); // read past text pointer
            reader.GetBytes(TdsEnums.TEXT_TIME_STAMP_LEN); // read past timestamp
            return (ulong)reader.GetTokenLength(tdsType);
        }

        public static uint? ReadPlpBlobLength(this TdsPackageReader reader)
        {
            var plpLength = (ulong)reader.ReadInt64();

            return plpLength == TdsEnums.SQL_PLP_NULL
                ? null
                : (uint?)plpLength;
        }

        public static int GetTokenLength(this TdsPackageReader tdsPackageReader, byte token)
        {
            switch (token)
            {
                case TdsEnums.SQLFEATUREEXTACK:
                    return -1;
                case TdsEnums.SQLUDT:
                    // special case for UDTs
                    return -1; // Should we return -1 or not call GetTokenLength for UDTs?
                case TdsEnums.SQLSESSIONSTATE:

                    return tdsPackageReader.ReadInt32();
                case TdsEnums.SQLRETURNVALUE:
                    return -1; // In Yukon, the RETURNVALUE token stream no longer has length
                case TdsEnums.SQLXMLTYPE:
                    return tdsPackageReader.ReadInt16();
            }

            return GetTdsTypeLen(tdsPackageReader, token);
        }

        public static int GetTdsTypeLen(TdsPackageReader tdsPackageReader, byte token)
        {
            switch (token & TdsEnums.SQLLenMask)
            {
                case TdsEnums.SQLFixedLen:
                    return (0x01 << ((token & 0x0c) >> 2)) & 0xff;
                case TdsEnums.SQLZeroLen:
                    return 0;
                case TdsEnums.SQLVarLen:
                case TdsEnums.SQLVarCnt:
                    if ((token & 0x80) != 0)
                        return tdsPackageReader.ReadUInt16();
                    else if ((token & 0x0c) == 0)
                        return tdsPackageReader.ReadInt32();
                    else
                        return tdsPackageReader.ReadByte();
                default:
                    return 0;
            }
        }

        public static ulong? ReadColumnHeader(this TdsPackageReader reader, int columnOrdinal)
        {
            // query NBC row information first
            var row = reader.CurrentRow;
            return row.IsNbcRow && row.IsNull(columnOrdinal)
                ? null
                : reader.GetColumnHeaderNoNbc(columnOrdinal);
        }

        private static ulong? GetColumnHeaderNoNbc(this TdsPackageReader reader, int columnOrdinal)
        {
            var col = reader.CurrentResultset.ColumnsMetadata[columnOrdinal];
            // plp-blob columns 
            if (col.IsLong && col.IsPlp)
                return reader.ReadPlpBlobLength();

            // non-plp-blob columns
            if (col.IsLong && !col.IsPlp)
                return reader.ReadBlobLength(col.TdsType);

            // non-blob columns
            var length = reader.GetTokenLength(col.TdsType);
            return length == 0 || length == TdsEnums.VARNULL ? null : (ulong?)length;
        }
    }
}