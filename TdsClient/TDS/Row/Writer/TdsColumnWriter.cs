using System;
using Medella.TdsClient.TDS.Messages.Server.Internal;
using Medella.TdsClient.TDS.Package.Writer;

namespace Medella.TdsClient.TDS.Row.Writer
{
    public class TdsColumnWriter
    {
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
            _writer.WriteNullableSqlBinary(value, index);
        }

        public void WriteNullableSqlString(string value, int index)
        {
            _writer.WriteNullableSqlString(value, index);
        }

        public void WriteNullableSqlVariant(object value, int index)
        {
            _writer.WriteNullableSqlVariant(value, index);
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