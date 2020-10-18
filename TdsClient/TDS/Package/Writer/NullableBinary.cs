using System;
using Medella.TdsClient.Constants;

// ReSharper disable once CheckNamespace
namespace Medella.TdsClient.TDS.Package.Writer
{
    public partial class TdsPackageWriter
    {
        private static readonly byte[] TextOrImageHeader = { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff };

        public void WriteNullableSqlBinary(byte[]? value, int index)
        {
            var md = ColumnsMetadata[index];
            if (md.IsPlp)
                WriteNullableSqlPlpBinary(value);
            else if (md.IsTextOrImage)
                WriteNullableSqlImage(value);
            else
                WriteNullableSqlBinary(value);
        }

        public void WriteNullableSqlUniqueId(Guid? value)
        {
            WriteBuffer[WritePosition++] = (byte)(value == null ? 0 : 16);
            if (value != null)
                WriteByteArray(((Guid)value).ToByteArray());
            else
                CheckBuffer();
        }

        public void WriteNullableSqlBinary(byte[]? value)
        {
            WriteInt16(value?.Length ?? TdsEnums.VARNULL);
            if (value == null) return;
            WriteByteArray(value);
        }

        private void WriteNullableSqlImage(byte[]? value)
        {
            WriteByte(value == null ? 0 : 0x10);
            if (value == null) return;
            WriteByteArray(TextOrImageHeader);
            WriteInt32(value.Length);
            WriteByteArray(value);
        }

        private void WriteNullableSqlPlpBinary(byte[]? value)
        {
            WriteUInt64(value == null ? TdsEnums.SQL_PLP_NULL : TdsEnums.SQL_PLP_UNKNOWNLEN);
            if (value == null) return;
            if (value.Length > 0)
            {
                WriteInt32(value.Length); //write in chunks
                WriteByteArray(value);
            }

            WriteInt32(TdsEnums.SQL_PLP_CHUNK_TERMINATOR); //chunks terminate
        }
    }
}