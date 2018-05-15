namespace Medella.TdsClient.TDS.Package.Reader
{
    public partial class TdsPackageReader
    {
        public decimal? ReadNullableSqlMoney(int index)
        {
            var length = ReadLengthNullableData(index);
            return length == null ? (decimal?) null : ReadSqlMoney((int) length);
        }

        public decimal? ReadNullableDecimal(int index, byte scale)
        {
            var length = ReadLengthNullableData(index);
            return length == null
                ? (decimal?) null
                : ReadSqlDecimal((int) length, scale);
        }
    }
}