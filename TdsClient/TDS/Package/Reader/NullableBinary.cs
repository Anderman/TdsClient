using System;

namespace Medella.TdsClient.TDS.Package.Reader
{
    public partial class TdsPackageReader
    {
        public byte[]? ReadNullableSqlBinary(int index)
        {
            var isPlp = CurrentResultSet.ColumnsMetadata[index].IsPlp;
            var length = ReadLengthNullableData(index);
            return length == null
                ? null
                : isPlp
                    ? ReadPlpBlobBytes((ulong)length)
                    : ReadByteArray(new byte[(int)length], 0, (int)length);
        }

        public Guid? ReadNullableSqlGuid(int index)
        {
            var length = ReadLengthNullableData(index);
            return length == null ? (Guid?)null : ReadGuid();
        }
    }
}