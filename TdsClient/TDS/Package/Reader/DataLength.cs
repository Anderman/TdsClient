using Medella.TdsClient.Constants;

namespace Medella.TdsClient.TDS.Package.Reader
{
    public partial class TdsPackageReader
    {
        public ulong? ReadLengthNullableData(int columnOrdinal)
        {
            // query NBC row information first
            var row = CurrentRow;
            return row.IsNbcRow && row.IsNull(columnOrdinal)
                ? null
                : ReadLengthNotNullData(columnOrdinal);
        }

        private ulong? ReadLengthNotNullData(int columnOrdinal)
        {
            var col = CurrentResultSet.ColumnsMetadata[columnOrdinal];
            // plp-blob columns  Var...(max) + SQLXMLTYPE
            if (col.IsPlp)
                return ReadPlpLength();

            // TdsEnums.SQLTEXT || TdsEnums.SQLNTEXT || TdsEnums.SQLIMAGE
            if (col.IsTextOrImage)
                return ReadBlobLength();

            // All other Columns
            return (ulong?)GetDataLen(col.TdsType);
        }

        public ulong? ReadBlobLength()
        {
            // TdsEnums.SQLTEXT || TdsEnums.SQLNTEXT || TdsEnums.SQLIMAGE
            // has textpointers and timestamp
            // we don't care about TextPtrs, simply go after the data after it
            // 0xD1,0x00 == null value!!!
            // 0xD1,
            // 0x10,len
            // 0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,
            // 0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,
            // 0x02,0x00,0x00,0x00, 32bit len
            var lengthTextPointer = ReadByte();
            if (lengthTextPointer == 0) return null;
            SkipBytes(lengthTextPointer + TdsEnums.TEXT_TIME_STAMP_LEN); // read past text pointer and timestamp
            return (ulong)ReadInt32();
        }

        public ulong? ReadPlpLength()
        {
            var plpLength = (ulong)ReadInt64();

            return plpLength == TdsEnums.SQL_PLP_NULL
                ? null
                : (ulong?)plpLength;
        }

        public int? GetDataLen(byte tdsType)
        {
            if ((tdsType & TdsEnums.SQLLenMask) == TdsEnums.SQLFixedLen)
            {
                var result = (0x01 << ((tdsType & 0x0c) >> 2)) & 0x0f;
                return result;
            }

            int? v;
            if ((tdsType & 0x80) != 0)
            {
                v = ReadUInt16();
                return v == TdsEnums.VARNULL ? null : v;
            }

            v = tdsType == TdsEnums.SQLVARIANT
                ? ReadInt32()
                : ReadByte();
            return v == 0 ? null : v;
        }
    }
}