namespace Medella.TdsClient.TDS.Package.Reader
{
    public partial class TdsPackageReader
    {
        public bool? ReadNullableSqlBit(int index)
        {
            var length = ReadLengthNullableData(index);
            return length == null ? (bool?)null : ReadByte() != 0;
        }

        public byte? ReadNullableSqlByte(int index)
        {
            var length = ReadLengthNullableData(index);
            return length == null ? (byte?)null : ReadByte();
        }

        public short? ReadNullableSqlInt16(int index)
        {
            var length = ReadLengthNullableData(index);
            return length == null ? (short?)null : ReadInt16();
        }

        public int? ReadNullableSqlInt32(int index)
        {
            var length = ReadLengthNullableData(index);
            return length == null ? (int?)null : ReadInt32();
        }

        public long? ReadNullableSqlInt64(int index)
        {
            var length = ReadLengthNullableData(index);
            return length == null ? (long?)null : ReadInt64();
        }

        public long? ReadNullableSqlIntN(int index)
        {
            var length = ReadLengthNullableData(index);
            switch (length)
            {
                case 1: return ReadByte();
                case 2: return ReadInt16();
                case 4: return ReadInt32();
                case 8: return ReadInt64();
            }

            return null;
        }
    }
}