namespace Medella.TdsClient.TDS.Package.Reader
{
    public partial class TdsPackageReader
    {
        public double? ReadNullableSqlFloatN(int index)
        {
            var length = ReadLengthNullableData(index);
            switch (length)
            {
                case 4: return ReadFloat();
                case 8: return ReadDouble();
            }

            return null;
        }

        public float? ReadNullableSqlFloat(int index)
        {
            var length = ReadLengthNullableData(index);
            return length == null ? (float?) null : ReadFloat();
        }

        public double? ReadNullableSqlDouble(int index)
        {
            var length = ReadLengthNullableData(index);
            return length == null ? (double?) null : ReadDouble();
        }
    }
}