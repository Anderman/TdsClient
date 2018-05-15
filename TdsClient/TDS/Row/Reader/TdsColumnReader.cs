using System;
using Medella.TdsClient.TDS.Messages.Server.Internal;
using Medella.TdsClient.TDS.Package.Reader;

namespace Medella.TdsClient.TDS.Row.Reader
{
    public class TdsColumnReader
    {
        private readonly TdsPackageReader _reader;
        public readonly ColumnsMetadata MetaData;

        public TdsColumnReader(TdsPackageReader reader)
        {
            _reader = reader;
            MetaData = reader.CurrentResultset.ColumnsMetadata;
        }

        public decimal? ReadDecimal(int index)
        {
            return _reader.ReadNullableDecimal(index, MetaData[index].Scale);
        }

        public byte[] ReadBinary(int index)
        {
            return _reader.ReadNullableSqlBinary(index);
        }

        public string ReadString(int index)
        {
            return _reader.ReadNullableSqlString(index);
        }

        public string ReadUnicodeString(int index)
        {
            return _reader.ReadNullableUnicodeString(index);
        }

        public DateTime? ReadSqlDate(int index)
        {
            return _reader.ReadNullableSqlDate(index);
        }

        public TimeSpan? ReadSqlTime(int index)
        {
            return _reader.ReadNullableSqlTime(index);
        }

        public DateTime? ReadSqlDateTime2(int index)
        {
            return _reader.ReadNullableSqlDateTime2(index);
        }

        public DateTime? ReadSqlDateTime(int index)
        {
            return _reader.ReadNullableSqlDateTime(index);
        }

        public DateTimeOffset? ReadSqlDateTimeOffset(int index)
        {
            return _reader.ReadNullableSqlDateTimeOffset(index);
        }

        public bool? ReadSqlBit(int index)
        {
            return _reader.ReadNullableSqlBit(index);
        }

        public long? ReadSqlIntN(int index)
        {
            return _reader.ReadNullableSqlIntN(index);
        }

        public byte? ReadSqlByte(int index)
        {
            return _reader.ReadNullableSqlByte(index);
        }

        public short? ReadSqlInt16(int index)
        {
            return _reader.ReadNullableSqlInt16(index);
        }

        public int? ReadSqlInt32(int index)
        {
            return _reader.ReadNullableSqlInt32(index);
        }

        public long? ReadSqlInt64(int index)
        {
            return _reader.ReadNullableSqlInt64(index);
        }

        public double? ReadSqlFloatN(int index)
        {
            return _reader.ReadNullableSqlFloatN(index);
        }

        public float? ReadSqlFloat(int index)
        {
            return _reader.ReadNullableSqlFloat(index);
        }

        public double? ReadSqlDouble(int index)
        {
            return _reader.ReadNullableSqlDouble(index);
        }

        public decimal? ReadSqlMoney(int index)
        {
            return _reader.ReadNullableSqlMoney(index);
        }

        public Guid? ReadSqlGuid(int index)
        {
            return _reader.ReadNullableSqlGuid(index);
        }

        public object ReadSqlVariant(int index)
        {
            return _reader.ReadNullableSqlVariant(index);
        }
    }
}