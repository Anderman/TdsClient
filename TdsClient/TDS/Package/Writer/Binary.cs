using System;

// ReSharper disable once CheckNamespace
namespace Medella.TdsClient.TDS.Package.Writer
{
    public partial class TdsPackageWriter
    {
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


        public void WriteSqlUniqueId(Guid value)
        {
            WriteByteArray(value.ToByteArray());
        }
    }
}