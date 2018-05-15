using Medella.TdsClient.Contants;
using Medella.TdsClient.TDS.Package;
using Medella.TdsClient.TDS.Package.Reader;

namespace Medella.TdsClient.TDS.Messages.Server
{
    public static class ParserTokenExtentions
    {
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
            }

            switch (token & TdsEnums.SQLLenMask)
            {
                case TdsEnums.SQLZeroLen:
                    return 0;
                case TdsEnums.SQLFixedLen:
                    return (0x01 << ((token & 0x0c) >> 2)) & 0xff;
                default:
                    return tdsPackageReader.ReadUInt16();
            }
        }
    }

    public static class ParserColumnHeaderExtentions
    {

        public static ulong? ReadLengthNullableData(this TdsPackageReader reader, int columnOrdinal)
        {
            // query NBC row information first
            var row = reader.CurrentRow;
            return row.IsNbcRow && row.IsNull(columnOrdinal)
                ? null
                : reader.ReadLengthNotNullData(columnOrdinal);
        }

        private static ulong? ReadLengthNotNullData(this TdsPackageReader reader, int columnOrdinal)
        {
            reader.WriteDebugString("header:");
            var col = reader.CurrentResultset.ColumnsMetadata[columnOrdinal];
            // plp-blob columns  Var...(max) + SQLXMLTYPE
            if (col.IsPlp)
                return reader.ReadPlpLength();

            // TdsEnums.SQLTEXT || TdsEnums.SQLNTEXT || TdsEnums.SQLIMAGE
            if (col.IsTextOrImage)
                return reader.ReadBlobLength(col.TdsType);

            // All other Columns
            return (ulong?)reader.GetDataLen(col.TdsType);
        }
        public static ulong? ReadPlpLength(this TdsPackageReader reader)
        {
            var plpLength = (ulong)reader.ReadInt64();

            return plpLength == TdsEnums.SQL_PLP_NULL
                ? null
                : (ulong?)plpLength;
        }

        public static ulong? ReadBlobLength(this TdsPackageReader reader, byte tdsType)
        {
            //
            // we don't care about TextPtrs, simply go after the data after it
            // TdsEnums.SQLTEXT || TdsEnums.SQLNTEXT || TdsEnums.SQLIMAGE
            //,0xD1,0x00 == null value!!!
            //0xD1,
            //0x10,len
            //0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,
            //0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,
            //0x02,0x00,0x00,0x00, 32bit len
            //0x2A,0x2A,
            var length = reader.ReadByte();
            if (length == 0) return null;
            reader.GetBytes(length); // read past text pointer
            reader.GetBytes(TdsEnums.TEXT_TIME_STAMP_LEN); // read past timestamp
            return (ulong)reader.ReadInt32();
        }

        public static int? GetDataLen(this TdsPackageReader tdsPackageReader, byte tdsType)
        {
            switch (tdsType & TdsEnums.SQLLenMask)
            {
                case TdsEnums.SQLFixedLen:
                    return (0x01 << ((tdsType & 0x0c) >> 2)) & 0xff;
                default:
                    if ((tdsType & 0x0c) == 0) //SQLVARIANT
                    {
                        var v = tdsPackageReader.ReadInt32();
                        return v == 0 ? (int?)null : v;
                    }
                    else if ((tdsType & 0x80) != 0)
                    {
                        var v = tdsPackageReader.ReadUInt16();
                        return v == TdsEnums.VARNULL ? (int?)null : v;
                    }
                    else
                    {
                        var v = tdsPackageReader.ReadByte();
                        return v == 0 ? (int?)null : v;
                    }
            }
        }
    }
}