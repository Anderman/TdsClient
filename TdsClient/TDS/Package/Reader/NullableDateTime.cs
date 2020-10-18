using System;

namespace Medella.TdsClient.TDS.Package.Reader
{
    public partial class TdsPackageReader
    {
        public DateTime? ReadNullableSqlDate(int index)
        {
            var length = ReadLengthNullableData(index);
            return length == null
                ? (DateTime?)null
                : ReadSqlDate();
        }

        public TimeSpan? ReadNullableSqlTime(int index)
        {
            var scale = CurrentResultSet.ColumnsMetadata[index].Scale;
            var length = ReadLengthNullableData(index);
            return length == null ? (TimeSpan?)null : ReadSqlTime((int)length, scale);
        }

        public DateTime? ReadNullableSqlDateTime2(int index)
        {
            var scale = CurrentResultSet.ColumnsMetadata[index].Scale;
            var length = ReadLengthNullableData(index);
            return length == null ? (DateTime?)null : ReadSqlDateTime((int)length, scale);
        }

        public DateTime? ReadNullableSqlDateTime(int index)
        {
            var length = ReadLengthNullableData(index);
            return length == null ? (DateTime?)null : ReadSqlDateTime((int)length);
        }

        public DateTimeOffset? ReadNullableSqlDateTimeOffset(int index)
        {
            var scale = CurrentResultSet.ColumnsMetadata[index].Scale;
            var length = ReadLengthNullableData(index);
            return length == null ? (DateTimeOffset?)null : ReadSqlDateTimeOffset((int)length, scale);
        }
    }
}