using System;
using System.Text;
using Medella.TdsClient.Contants;
using Medella.TdsClient.TDS.Package;

namespace Medella.TdsClient.TDS.Messages.Server
{
    public static class ParserColumnPlpExtentions
    {
        public static uint ReadPlpBlobChunkLength(this TdsPackageReader reader)
        {
            return reader.ReadUInt32();
        }

        internal static byte[] ReadPlpBlobBytes(this TdsPackageReader reader, ulong plpLength)
        {
            if (plpLength == 0)
            {
                ReadPlpBlobChunkLength(reader); //read terminator
                return Array.Empty<byte>();
            }

            var chungLength = reader.ReadPlpBlobChunkLength();
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

                reader.ReadByteArray(buff, offset, bytesToRead);
                offset += bytesToRead;
                chungLength -= (uint)bytesToRead;

                // Read the next chunk or cleanup state if hit the end
                if (chungLength == 0)
                    chungLength = reader.ReadPlpBlobChunkLength();
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

        internal static string ReadPlpString(this TdsPackageReader reader, Encoding encoding, ulong plpLength)
        {
            if (plpLength == 0)
                return string.Empty;

            var chungLength = reader.ReadPlpBlobChunkLength();
            if (chungLength == 0)
                return string.Empty;


            var bytesToRead = chungLength > int.MaxValue ? int.MaxValue : (int)chungLength;
            if (plpLength == (ulong)bytesToRead)
            {
                var v = reader.ReadString(encoding, bytesToRead);
                reader.ReadPlpBlobChunkLength(); //read the 0 chunklen
                return v;
            }

            // If total length is known up front, allocate the whole buffer in one shot instead of realloc'ing and copying over each time
            var sb = new StringBuilder();
            var bytesRead = 0;
            while (true)
            {
                bytesToRead = (uint)bytesRead + (uint)bytesToRead > int.MaxValue ? int.MaxValue - bytesToRead : bytesToRead;

                reader.ReadString(sb, encoding, bytesToRead);
                bytesRead += bytesToRead;
                chungLength -= (uint)bytesToRead;

                // Read the next chunk or cleanup state if hit the end
                if (chungLength == 0)
                    chungLength = reader.ReadPlpBlobChunkLength();
                if (chungLength == 0 || bytesRead == int.MaxValue) // Data read complete. bytesleft>0 if len > blob.Length and SQL_PLP_UNKNOWNLEN
                    break;
            }

            return sb.ToString();
        }

        internal static string ReadPlpUnicodeChars(this TdsPackageReader reader, ulong plpLength)
        {
            if (plpLength == 0)
                return string.Empty;

            var chungLength = reader.ReadPlpBlobChunkLength(); //chunck is always smaller than packetsize
            if (chungLength == 0)
                return string.Empty;

            // If total length is known up front, allocate the whole buffer in one shot instead of realloc'ing and copying over each time

            if (plpLength == chungLength)
            {
                var v = reader.ReadUnicodeChars((int)chungLength);
                reader.ReadPlpBlobChunkLength(); //read the 0 chunklen
                return v;
            }

            var sb = new StringBuilder();
            byte? byte1 = null;
            while (true)
            {
                if (byte1 != null)
                {
                    var byte2 = reader.ReadByte();
                    sb.Append((char)((int)(byte1 << 8) + byte2));
                    chungLength--;
                    byte1 = null;
                }
                var byteToRead = chungLength & 0xFFFFFFFE;
                if (byteToRead != chungLength)
                {
                    reader.ReadUnicodeChars(sb, (int)byteToRead);
                    byte1 = reader.ReadByte();
                }
                else
                    reader.ReadUnicodeChars(sb, (int)chungLength);
                // Read the next chunk or cleanup state if hit the end
                chungLength = reader.ReadPlpBlobChunkLength();
                if (chungLength == 0) // Data read complete. bytesleft>0 if len > blob.Length and SQL_PLP_UNKNOWNLEN
                    break;
            }

            return sb.ToString();
        }
    }
}