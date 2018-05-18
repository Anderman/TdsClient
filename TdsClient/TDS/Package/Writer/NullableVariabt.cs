using System;
using System.Collections.Generic;
using System.Text;
using Medella.TdsClient.Contants;

// ReSharper disable once CheckNamespace
namespace Medella.TdsClient.TDS.Package.Writer
{
    public partial class TdsPackageWriter
    {

        public void WriteNullableSqlVariant(object value, int index)
        {
            if (value == null)
            {
                WriteInt32(0);
                return;
            }

            //
            // now Write the value
            //
            switch (value)
            {
                case bool v:
                    WriteInt32(TdsEnums.SQLVARIANT_SIZE + 1);
                    WriteByte(TdsEnums.SQLBIT);
                    WriteByte(0);
                    WriteByte(v ? 1 : 0);
                    return;
                case byte v:
                    WriteInt32(TdsEnums.SQLVARIANT_SIZE + 1);
                    WriteByte(TdsEnums.SQLINT1);
                    WriteByte(0);
                    WriteByte(v);
                    return;
                case short v:
                    WriteInt32(TdsEnums.SQLVARIANT_SIZE + 2);
                    WriteByte(TdsEnums.SQLINT2);
                    WriteByte(0);
                    WriteInt16(v);
                    return;
                case int v:
                    WriteInt32(TdsEnums.SQLVARIANT_SIZE + 4);
                    WriteByte(TdsEnums.SQLINT4);
                    WriteByte(0);
                    WriteInt32(v);
                    return;
                case long v:
                    WriteInt32(TdsEnums.SQLVARIANT_SIZE + 8);
                    WriteByte(TdsEnums.SQLINT8);
                    WriteByte(0);
                    WriteInt64(v);
                    return;
                case float v:
                    WriteInt32(TdsEnums.SQLVARIANT_SIZE + 4);
                    WriteByte(TdsEnums.SQLFLT4);
                    WriteByte(0);
                    WriteFloat(v);
                    return;
                case double v:
                    WriteInt32(TdsEnums.SQLVARIANT_SIZE + 8);
                    WriteByte(TdsEnums.SQLFLT8);
                    WriteByte(0);
                    WriteDouble(v);
                    return;
                case DateTime v:
                    WriteInt32(TdsEnums.SQLVARIANT_SIZE + 8);
                    WriteByte(TdsEnums.SQLDATETIME);
                    WriteByte(0);
                    WriteDateTime(v);
                    return;
                case Guid v:
                    WriteInt32(TdsEnums.SQLVARIANT_SIZE + 16);
                    WriteByte(TdsEnums.SQLUNIQUEID);
                    WriteByte(0);
                    WriteSqlUniqueId(v);
                    return;
                case decimal v:
                    {
                        WriteInt32(TdsEnums.SQLVARIANT_SIZE + 2 + 13);
                        WriteByte(TdsEnums.SQLDECIMALN);
                        WriteByte(2);
                        WriteByte(28);
                        var scale = (byte)(decimal.GetBits(v)[3] >> 16);
                        WriteByte(scale);
                        WriteSqlDecimal(v, 13);
                        return;
                    }
                case byte[] v:
                    {
                        WriteInt32(TdsEnums.SQLVARIANT_SIZE + 2 + v.Length);
                        WriteByte(TdsEnums.SQLBIGBINARY);
                        WriteByte(2);
                        WriteInt16(v.Length);
                        WriteByteArray(v);
                        return;
                    }
                case string v:
                    {
                        var collation = ColumnsMetadata[index].Collation;
                        var encoding = ColumnsMetadata[index].Encoding;
                        var bytes = encoding.GetBytes(v);
                        WriteInt32(TdsEnums.SQLVARIANT_SIZE + 5 + 2 + bytes.Length);
                        WriteByte(TdsEnums.SQLBIGVARCHAR);
                        WriteByte(7);
                        WriteUInt32(collation.Info);
                        WriteByte(collation.SortId);
                        WriteNullableSqlBinary(bytes);
                        return;
                    }
                case TimeSpan v:
                    {
                        const byte scale = 7;
                        const int len = 5;
                        WriteInt32(TdsEnums.SQLVARIANT_SIZE + 1 + len);
                        WriteByte(TdsEnums.SQLTIME);
                        WriteByte(1);
                        WriteByte(scale);
                        WriteSqlTime(v, scale);
                        return;
                    }
                case DateTimeOffset v:
                    {
                        const byte scale = 7;
                        const int len = 10;
                        WriteInt32(TdsEnums.SQLVARIANT_SIZE + 1 + len);
                        WriteByte(TdsEnums.SQLDATETIMEOFFSET);
                        WriteByte(1);
                        WriteByte(scale);
                        WriteSqlDateTimeOffset(v, scale);
                        return;
                    }
            }

            throw new Exception("Unsupported variant object");
        }
    }
}
