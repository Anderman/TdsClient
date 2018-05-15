using System;
using System.Collections.Generic;
using System.Text;

// ReSharper disable once CheckNamespace
namespace Medella.TdsClient.TDS.Package.Writer
{
    public partial class TdsPackageWriter
    {

        public void WriteUnicodeString(string v)
        {
            if (v.Length == 0) return;
            if (v.Length * 2 <= WriteBuffer.Length - WritePosition)
            {
                WritePosition += Encoding.Unicode.GetBytes(v, 0, v.Length, WriteBuffer, WritePosition);
            }
            else
            {
                var buffer = new byte[v.Length * 2];
                var bytes = Encoding.Unicode.GetBytes(v, 0, v.Length, buffer, 0);
                WriteByteArray(buffer);
            }
        }

        public void WriteByteArray(byte[] src)
        {
            var length = src.Length;
            var srcOffset = 0;
            var bytesLeft = length - srcOffset;
            while (bytesLeft > BufferSize - WritePosition)
            {
                var count = BufferSize - WritePosition;
                Buffer.BlockCopy(src, srcOffset, WriteBuffer, WritePosition, count);
                srcOffset += count;
                bytesLeft -= count;
                SendBatchPackage();
            }

            Buffer.BlockCopy(src, srcOffset, WriteBuffer, WritePosition, bytesLeft);
            WritePosition += bytesLeft;
        }

        public void WriteSqlUniqueId(Guid? value)
        {
            WriteByteArray(((Guid)value).ToByteArray());
        }
        public void WriteNullableSqlUniqueId(Guid? value)
        {
            WriteBuffer[WritePosition++] = (byte)(value == null ? 0 : 16);
            if (value != null)
                WriteByteArray(((Guid)value).ToByteArray());
            else
                CheckBuffer();
        }
    }
}
