using System;
using Medella.TdsClient.Contants;
using Medella.TdsClient.TDS.Messages.Server.Internal;
using Medella.TdsClient.TDS.Package;
using Medella.TdsClient.TDS.Reader.StringHelpers;

namespace Medella.TdsClient.TDS.Messages.Client
{
    public static class WriterCommon
    {
        public static void WriteByteLenString(this TdsPackageWriter writer, string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                writer.WriteByte(0);
            }
            else
            {
                writer.WriteByte(checked((byte)s.Length));
                writer.WriteUnicodeString(s);
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
        public static void WriteCollation2(this TdsPackageWriter writer, SqlCollations collation)
        {
            writer.WriteUInt32(collation.Info);
            writer.WriteByte(collation.SortId);
        }
        public static void WriteRpcBatchHeaders(this TdsPackageWriter tdsPackageWriter,long sqlConnectionId)
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
            WriteMarsHeaderData(tdsPackageWriter, sqlConnectionId);
        }
        private static void WriteMarsHeaderData(TdsPackageWriter tdsPackageWriter, long sqlConnectionId) //transactions not implemented
        {
            tdsPackageWriter.WriteInt16(TdsEnums.HEADERTYPE_MARS);
            tdsPackageWriter.WriteInt64(sqlConnectionId);
            tdsPackageWriter.WriteInt32(1);
        }
        public static void WriteTypeInfoLen(this TdsPackageWriter writer, TdsMetaType.MetaDataWrite metaType, int size, bool isNull)
        {
            //write length or isNull information about the type 
            if (metaType.IsPlp)
            {
                if (isNull)
                    writer.WriteInt64(unchecked((long)TdsEnums.SQL_PLP_NULL));
                else
                    writer.WriteInt64(size);
            }
            else if (metaType.IsLong)
            {
                // text/image/SQLVariant have a 4 byte length, plp datatypes have 8 byte lengths
                if (isNull)
                    writer.WriteInt32(unchecked((int)TdsEnums.VARLONGNULL));
                else
                    writer.WriteInt32(size);
            }
            else if (metaType.NullableType == TdsEnums.SQLDATETIME2 || metaType.NullableType == TdsEnums.SQLTIME || metaType.NullableType == TdsEnums.SQLDATETIMEOFFSET)
            {
                if (isNull)
                    writer.WriteByte(TdsEnums.FIXEDNULL);
                else
                    writer.WriteByte((byte)size);
            }
            else if (metaType.NullableType == TdsEnums.SQLBIGVARBINARY || metaType.NullableType == TdsEnums.SQLBIGCHAR || metaType.NullableType == TdsEnums.SQLNCHAR || metaType.NullableType == TdsEnums.SQLNVARCHAR)//skip varchar
            {
                // non-long but variable length column, must be a BIG* type: 2 byte length
                writer.WriteInt16(isNull ? TdsEnums.VARNULL : size);
            }
            else
            {
                if (isNull)
                    writer.WriteByte(TdsEnums.FIXEDNULL);
                else
                    writer.WriteByte((byte)size); // 1 byte for everything else
            }
        }

        public static readonly DateTime BaseDate1 = new DateTime(1, 1, 1);
        public static void WriteDate(this TdsPackageWriter writer, DateTime value)
        {
            var dayPart = value.Subtract(BaseDate1).Days;
            writer.WriteByte(dayPart);
            writer.WriteByte(dayPart>>8);
            writer.WriteByte(dayPart>>16);
        }
        public static void WriteDateTime(this TdsPackageWriter writer, DateTime value)
        {
            int dayPart, timepart;
            (dayPart, timepart) = ConverDateTime(value);
            writer.WriteInt32(dayPart);
            writer.WriteInt32(timepart);
        }

        public static (int dayPart, int timepart) ConverDateTime(DateTime value)
        {
            const double sqlTicksPerMillisecond = 0.3;
            var baseDate = new DateTime(1900, 1, 1);
            var dayPart = value.Subtract(baseDate).Days;
            var timepart = (int)((value.TimeOfDay.Ticks / (double)TimeSpan.TicksPerMillisecond) * sqlTicksPerMillisecond);
            return (dayPart, timepart);
        }

        public static void WriteSqlDecimal(this TdsPackageWriter writer, decimal value, int length)
        {
            var bits = decimal.GetBits(value);
            writer.WriteByte(0x80000000 == (bits[3] & 0x80000000) ? 0 : 1);
            writer.WriteInt32(bits[0]);
            if (length >= 9) writer.WriteInt32(bits[1]);
            if (length >= 13) writer.WriteInt32(bits[2]);
            if (length == 17) writer.WriteInt32(0);
        }

        public static void WriteSqlGuid(this TdsPackageWriter writer, Guid value)
        {
            writer.WriteByteArray(value.ToByteArray());

        }
        public static void WriteParameterLen(this TdsPackageWriter writer, TdsMetaType.MetaDataWrite metaType, int size, bool isNull)
        {
            if (isNull) return;
            //write length of the parameter
            if (metaType.IsPlp)
                writer.WriteInt64(size);
            else if (metaType.IsLong)
                // text/image/SQLVariant have a 4 byte length, plp datatypes have 8 byte lengths
                writer.WriteInt32(size);
            else if (metaType.NullableType == TdsEnums.SQLDATETIME2 || metaType.NullableType == TdsEnums.SQLTIME || metaType.NullableType == TdsEnums.SQLDATETIMEOFFSET)
                writer.WriteByte((byte)size);
            else if (metaType.NullableType == TdsEnums.SQLBIGVARBINARY || metaType.NullableType == TdsEnums.SQLBIGCHAR || metaType.NullableType == TdsEnums.SQLNCHAR || metaType.NullableType == TdsEnums.SQLNVARCHAR)//skip varchar
                writer.WriteInt16(size);
            else
                writer.WriteByte((byte)size); // 1 byte for everything else
        }

    }

    public static class WriterColMetaData
    {
        public static void WriteColumn(this TdsPackageWriter writer, ColumnMetadata col)
        {
            writer.WriteInt32(0);
            writer.WriteByte(col.TdsType);
            writer.WriteByte(col.Flag1);
            writer.WriteByte(col.Flag2);
            //writer.WriteTypeInfoLen();
        }
    }
}