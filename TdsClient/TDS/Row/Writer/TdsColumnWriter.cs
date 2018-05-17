using System;
using System.Text;
using Medella.TdsClient.Contants;
using Medella.TdsClient.TDS.Messages.Server.Internal;
using Medella.TdsClient.TDS.Package.Writer;

namespace Medella.TdsClient.TDS.Row.Writer
{
    public class TdsColumnWriter
    {
        private static readonly byte[] TextOrImageHeader = {0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff};

        private readonly TdsPackageWriter _writer;
        public readonly MetadataBulkCopy[] MetaData;

        public TdsColumnWriter(TdsPackageWriter writer)
        {
            _writer = writer;
            MetaData = writer.ColumnsMetadata;
        }

        public void WriteNullableSqlBit(bool? value, int index)
        {
            _writer.WriteNullableSqlBit(value);
        }

        public void WriteNullableSqlByte(byte? value, int index)
        {
            _writer.WriteNullableSqlByte(value);
        }

        public void WriteNullableSqlInt16(short? value, int index)
        {
            _writer.WriteNullableSqlInt16(value);
        }

        public void WriteNullableSqlInt32(int? value, int index)
        {
            _writer.WriteNullableSqlInt32(value);
        }

        public void WriteNullableSqlInt64(long? value, int index)
        {
            _writer.WriteNullableSqlInt64(value);
        }

        public void WriteNullableSqlMoneyN(decimal? value, int index)
        {
            var len = MetaData[index].Length;
            if (len == 4) _writer.WriteNullableSqlMoney4(value);
            if (len == 8) _writer.WriteNullableSqlMoney(value);
        }

        public void WriteNullableSqlMoney4(decimal? value, int index)
        {
            _writer.WriteNullableSqlMoney4(value);
        }

        public void WriteNullableSqlMoney(decimal? value, int index)
        {
            _writer.WriteNullableSqlMoney(value);
        }

        public void WriteNullableSqlFloat(float? value, int index)
        {
            _writer.WriteNullableSqlFloat(value);
        }

        public void WriteNullableSqlDouble(double? value, int index)
        {
            _writer.WriteNullableSqlDouble(value);
        }

        public void WriteNullableSqlDecimal(decimal? value, int index)
        {
            var precision = MetaData[index].Precision;
            var scale = MetaData[index].Scale;
            _writer.WriteNullableSqlDecimal(value, precision, scale);
        }

        public void WriteNullableSqlDate(DateTime? value, int index)
        {
            _writer.WriteNullableSqlDate(value);
        }

        public void WriteNullableSqlTime(TimeSpan? value, int index)
        {
            var scale = MetaData[index].Scale;
            _writer.WriteNullableSqlTime(value, scale);
        }

        public void WriteNullableSqlDateTime2(DateTime? value, int index)
        {
            var scale = MetaData[index].Scale;
            _writer.WriteNullableSqlDateTime2(value, scale);
        }

        public void WriteNullableSqlDateTime(DateTime? value, int index)
        {
            var len = MetaData[index].Length;
            _writer.WriteNullableSqlDateTime(value, len);
        }

        public void WriteNullableSqlDateTimeOffset(DateTimeOffset? value, int index)
        {
            var scale = MetaData[index].Scale;
            _writer.WriteNullableSqlDateTimeOffset(value, scale);
        }

        public void WriteNullableSqlUniqueId(Guid? value, int index)
        {
            _writer.WriteNullableSqlUniqueId(value);
        }

        //0xD1,row
        //0xFE,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF, Unknown length
        //0x01,0x00,0x00,0x00, chunk
        //0x31,
        //0x00,0x00,0x00,0x00, terminator
        public void WriteNullableSqlBinary(byte[] value, int index)
        {
            var md = MetaData[index];
            if (md.IsPlp)
                WriteNullableSqlPlpBinary(value);
            else if (md.IsTextOrImage)
                WriteNullableSqlImage(value);
            else
                WriteNullableSqlBinary(value);
        }

        private void WriteNullableSqlBinary(byte[] value)
        {
            _writer.WriteInt16(value?.Length ?? TdsEnums.VARNULL);
            if (value == null) return;
            _writer.WriteByteArray(value);
        }

        private void WriteNullableSqlImage(byte[] value)
        {
            _writer.WriteByte(value == null ? 0 : 0x10);
            if (value == null) return;
            _writer.WriteByteArray(TextOrImageHeader);
            _writer.WriteInt32(value.Length);
            _writer.WriteByteArray(value);
        }

        private void WriteNullableSqlPlpBinary(byte[] value)
        {
            _writer.WriteUInt64(value == null ? TdsEnums.SQL_PLP_NULL : TdsEnums.SQL_PLP_UNKNOWNLEN);
            if (value == null) return;
            _writer.WriteInt32(value.Length); //write in chunks
            _writer.WriteByteArray(value);
            _writer.WriteInt32(TdsEnums.SQL_PLP_CHUNK_TERMINATOR); //chunks terminate
        }

        public void WriteNullableSqlString(string value, int index)
        {
            if (value == null)
            {
                WriteNullableSqlBinary(null, index);
                return;
            }

            var md = MetaData[index];
            if (md.NonUniCode)
                WriteNullableSqlBinary(md.Encoding.GetBytes(value), index);
            else
                WriteNullableSqlBinary(Encoding.Unicode.GetBytes(value), index);
        }

        public void WriteNullableSqlVariant(object value, int index)
        {
            if (value == null)
            {
                _writer.WriteInt32(0);
                return;
            }

            //
            // now Write the value
            //
            switch (value)
            {
                case bool v:
                    _writer.WriteInt32(TdsEnums.SQLVARIANT_SIZE + 1);
                    _writer.WriteByte(TdsEnums.SQLBIT);
                    _writer.WriteByte(0);
                    _writer.WriteByte(v ? 1 : 0);
                    return;
                case byte v:
                    _writer.WriteInt32(TdsEnums.SQLVARIANT_SIZE + 1);
                    _writer.WriteByte(TdsEnums.SQLINT1);
                    _writer.WriteByte(0);
                    _writer.WriteByte(v);
                    return;
                case short v:
                    _writer.WriteInt32(TdsEnums.SQLVARIANT_SIZE + 2);
                    _writer.WriteByte(TdsEnums.SQLINT2);
                    _writer.WriteByte(0);
                    _writer.WriteInt16(v);
                    return;
                case int v:
                    _writer.WriteInt32(TdsEnums.SQLVARIANT_SIZE + 4);
                    _writer.WriteByte(TdsEnums.SQLINT4);
                    _writer.WriteByte(0);
                    _writer.WriteInt32(v);
                    return;
                case long v:
                    _writer.WriteInt32(TdsEnums.SQLVARIANT_SIZE + 8);
                    _writer.WriteByte(TdsEnums.SQLINT8);
                    _writer.WriteByte(0);
                    _writer.WriteInt64(v);
                    return;
                case float v:
                    _writer.WriteInt32(TdsEnums.SQLVARIANT_SIZE + 4);
                    _writer.WriteByte(TdsEnums.SQLFLT4);
                    _writer.WriteByte(0);
                    _writer.WriteFloat(v);
                    return;
                case double v:
                    _writer.WriteInt32(TdsEnums.SQLVARIANT_SIZE + 8);
                    _writer.WriteByte(TdsEnums.SQLFLT8);
                    _writer.WriteByte(0);
                    _writer.WriteDouble(v);
                    return;
                case DateTime v:
                    _writer.WriteInt32(TdsEnums.SQLVARIANT_SIZE + 8);
                    _writer.WriteByte(TdsEnums.SQLDATETIME);
                    _writer.WriteByte(0);
                    _writer.WriteDateTime(v);
                    return;
                case Guid v:
                    _writer.WriteInt32(TdsEnums.SQLVARIANT_SIZE + 16);
                    _writer.WriteByte(TdsEnums.SQLUNIQUEID);
                    _writer.WriteByte(0);
                    _writer.WriteSqlUniqueId(v);
                    return;
                case decimal v:
                {
                    _writer.WriteInt32(TdsEnums.SQLVARIANT_SIZE + 2 + 13);
                    _writer.WriteByte(TdsEnums.SQLDECIMALN);
                    _writer.WriteByte(2);
                    _writer.WriteByte(28);
                    var scale = (byte) (decimal.GetBits(v)[3] >> 16);
                    _writer.WriteByte(scale);
                    _writer.WriteSqlDecimal(v, 13);
                    return;
                }
                case byte[] v:
                {
                    _writer.WriteInt32(TdsEnums.SQLVARIANT_SIZE + 2 + v.Length);
                    _writer.WriteByte(TdsEnums.SQLBIGBINARY);
                    _writer.WriteByte(2);
                    _writer.WriteInt16(v.Length);
                    _writer.WriteByteArray(v);
                    return;
                }
                case string v:
                {
                    var collation = _writer.ColumnsMetadata[index].Collation;
                    var encoding = _writer.ColumnsMetadata[index].Encoding;
                    var bytes = encoding.GetBytes(v);
                    _writer.WriteInt32(TdsEnums.SQLVARIANT_SIZE + 5 + 2 + bytes.Length);
                    _writer.WriteByte(TdsEnums.SQLBIGVARCHAR);
                    _writer.WriteByte(7);
                    _writer.WriteUInt32(collation.Info);
                    _writer.WriteByte(collation.SortId);
                    WriteNullableSqlBinary(bytes);
                    return;
                }
                case TimeSpan v:
                {
                    const byte scale = 7;
                    const int len = 5;
                    _writer.WriteInt32(TdsEnums.SQLVARIANT_SIZE + 1 + len);
                    _writer.WriteByte(TdsEnums.SQLTIME);
                    _writer.WriteByte(1);
                    _writer.WriteByte(scale);
                    _writer.WriteSqlTime(v, scale);
                    return;
                }
                case DateTimeOffset v:
                {
                    const byte scale = 7;
                    const int len = 10;
                    _writer.WriteInt32(TdsEnums.SQLVARIANT_SIZE + 1 + len);
                    _writer.WriteByte(TdsEnums.SQLDATETIMEOFFSET);
                    _writer.WriteByte(1);
                    _writer.WriteByte(scale);
                    _writer.WriteSqlDateTimeOffset(v, scale);
                    return;
                }
            }

            throw new Exception("Unsupported variant object");
        }


        public void WriteSqlBit(bool value, int index)
        {
            _writer.WriteByte(value ? 1 : 0);
        }

        public void WriteSqlByte(byte value, int index)
        {
            _writer.WriteByte(value);
        }

        public void WriteSqlInt16(short value, int index)
        {
            _writer.WriteInt16(value);
        }

        public void WriteSqlInt32(int value, int index)
        {
            _writer.WriteInt32(value);
        }

        public void WriteSqlInt64(long value, int index)
        {
            _writer.WriteInt64(value);
        }

        public void WriteSqlMoney4(decimal value, int index)
        {
            _writer.WriteSqlMoney4(value);
        }

        public void WriteSqlMoney(decimal value, int index)
        {
            _writer.WriteSqlMoney(value);
        }

        public void WriteSqlFloat(float value, int index)
        {
            _writer.WriteFloat(value);
        }

        public void WriteSqlDouble(double value, int index)
        {
            _writer.WriteDouble(value);
        }

        public void WriteSqlDateTime4(DateTime value, int index)
        {
            _writer.WriteSqlDateTime4(value);
        }

        public void WriteSqlDateTime(DateTime value, int index)
        {
            _writer.WriteSqlDateTime(value);
        }
    }
}