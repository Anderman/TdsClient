using System;
using Medella.TdsClient.Constants;

namespace Medella.TdsClient.TDS.Package.Reader
{
    public partial class TdsPackageReader
    {
        internal byte[] ReadPlpBlobBytes(ulong plpLength)
        {
            if (plpLength == 0)
            {
                ReadUInt32(); //read terminator
                return Array.Empty<byte>();
            }

            var chungLength = ReadUInt32();
            if (chungLength == 0)
                return Array.Empty<byte>();

            // If total length is known up front, allocate the whole buffer in one shot instead of realloc'ing and copying over each time
            var buff = plpLength == TdsEnums.SQL_PLP_UNKNOWNLEN
                ? new byte[chungLength]
                : new byte[plpLength > int.MaxValue ? int.MaxValue : plpLength];

            var offset = 0;
            var bytesToRead = chungLength > int.MaxValue ? int.MaxValue : (int)chungLength;
            while (true)
            {
                bytesToRead = (uint)offset + (uint)bytesToRead > int.MaxValue ? int.MaxValue - bytesToRead : bytesToRead;
                if (buff.Length < offset + bytesToRead)
                    ResizeArray(ref buff, bytesToRead);

                ReadByteArray(buff, offset, bytesToRead);
                offset += bytesToRead;
                chungLength -= (uint)bytesToRead;

                // Read the next chunk or cleanup state if hit the end
                if (chungLength == 0)
                    chungLength = ReadUInt32();
                if (chungLength == 0 || offset == int.MaxValue) // Data read complete. bytesleft>0 if len > blob.Length and SQL_PLP_UNKNOWNLEN
                    break;
            }

            return buff;
        }

        private static void ResizeArray(ref byte[] buff, int grow)
        {
            var newbuf = new byte[buff.Length + grow];
            Buffer.BlockCopy(buff, 0, newbuf, 0, buff.Length);
            buff = newbuf;
        }
    }
}