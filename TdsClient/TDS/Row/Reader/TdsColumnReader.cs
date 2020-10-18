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
            MetaData = reader.CurrentResultSet.ColumnsMetadata;
        }

        public decimal? ReadDecimal(int index) => _reader.ReadNullableDecimal(index, MetaData[index].Scale);

        public byte[] ReadBinary(int index) => _reader.ReadNullableSqlBinary(index);

        public string ReadString(int index) => _reader.ReadNullableSqlString(index);

        public string ReadUnicodeString(int index) => _reader.ReadNullableUnicodeString(index);

        public DateTime? ReadSqlDate(int index) => _reader.ReadNullableSqlDate(index);

        public TimeSpan? ReadSqlTime(int index) => _reader.ReadNullableSqlTime(index);

        public DateTime? ReadSqlDateTime2(int index) => _reader.ReadNullableSqlDateTime2(index);

        public DateTime? ReadSqlDateTime(int index) => _reader.ReadNullableSqlDateTime(index);

        public DateTimeOffset? ReadSqlDateTimeOffset(int index) => _reader.ReadNullableSqlDateTimeOffset(index);

        public bool? ReadSqlBit(int index) => _reader.ReadNullableSqlBit(index);

        public long? ReadSqlIntN(int index) => _reader.ReadNullableSqlIntN(index);

        public byte? ReadSqlByte(int index) => _reader.ReadNullableSqlByte(index);

        public short? ReadSqlInt16(int index) => _reader.ReadNullableSqlInt16(index);

        public int? ReadSqlInt32(int index) => _reader.ReadNullableSqlInt32(index);

        public long? ReadSqlInt64(int index) => _reader.ReadNullableSqlInt64(index);

        public double? ReadSqlFloatN(int index) => _reader.ReadNullableSqlFloatN(index);

        public float? ReadSqlFloat(int index) => _reader.ReadNullableSqlFloat(index);

        public double? ReadSqlDouble(int index) => _reader.ReadNullableSqlDouble(index);

        public decimal? ReadSqlMoney(int index) => _reader.ReadNullableSqlMoney(index);

        public Guid? ReadSqlGuid(int index) => _reader.ReadNullableSqlGuid(index);

        public object ReadSqlVariant(int index) => _reader.ReadNullableSqlVariant(index);
    }
}