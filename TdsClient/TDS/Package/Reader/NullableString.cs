namespace Medella.TdsClient.TDS.Package.Reader
{
    public partial class TdsPackageReader
    {
        public string? ReadNullableSqlString(int index)
        {
            var md = CurrentResultSet.ColumnsMetadata[index];
            var isPlp = md.IsPlp;
            var length = ReadLengthNullableData(index);
            return length == null
                ? null
                : isPlp
                    ? ReadPlpString(md.Encoding!, (ulong)length)
                    : ReadString(md.Encoding!, (int)length);
        }

        public string? ReadNullableUnicodeString(int index)
        {
            var md = CurrentResultSet.ColumnsMetadata[index];
            var isPlp = md.IsPlp;
            var length = ReadLengthNullableData(index);
            return length == null
                ? null
                : isPlp
                    ? ReadPlpUnicodeChars((ulong)length)
                    : ReadUnicodeChars((int)length);
        }
    }
}