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
        public static void WriteMarsHeader(this TdsPackageWriter writer, long sqlTransactionId)
        {
            const int marsHeaderSize = 18; // 4 + (2 + 8 + 4)= size + data
            const int totalHeaderLength = 22; // 4 + (4 + 2 + 8 + 4) size+mars
            writer.WriteInt32(totalHeaderLength);

            writer.WriteInt32(marsHeaderSize);

            writer.WriteInt16(TdsEnums.HEADERTYPE_MARS);

            writer.WriteInt64(sqlTransactionId);

            writer.WriteInt32(1);
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
}