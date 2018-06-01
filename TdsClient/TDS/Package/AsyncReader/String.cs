//using System.Text;
//using Medella.TdsClient.TDS.Row.Reader.StringHelpers;

//namespace Medella.TdsClient.TDS.Package.AsyncReader
//{
//    public partial class AsyncPackageReader
//    {
//        internal string ReadPlpString(Encoding encoding, ulong plpLength)
//        {
//            if (plpLength == 0)
//                return string.Empty;

//            var chungLength = ReadUInt32();
//            if (chungLength == 0)
//                return string.Empty;


//            var bytesToRead = chungLength > int.MaxValue ? int.MaxValue : (int) chungLength;
//            if (plpLength == (ulong) bytesToRead)
//            {
//                var v = ReadString(encoding, bytesToRead);
//                ReadUInt32(); //read the 0 chunklen
//                return v;
//            }

//            // If total length is known up front, allocate the whole buffer in one shot instead of realloc'ing and copying over each time
//            var sb = new StringBuilder();
//            var bytesRead = 0;
//            while (true)
//            {
//                bytesToRead = ((uint) bytesRead + chungLength > int.MaxValue) ? int.MaxValue - bytesRead : (int)chungLength; //read not futher than 2Gb

//                ReadString(sb, encoding, bytesToRead);
//                bytesRead += bytesToRead;
//                chungLength -= (uint) bytesToRead;

//                // Read the next chunk or cleanup state if hit the end
//                if (chungLength == 0)
//                    chungLength = ReadUInt32();
//                if (chungLength == 0 || bytesRead == int.MaxValue) // Data read complete. bytesleft>0 if len > blob.Length and SQL_PLP_UNKNOWNLEN
//                    break;
//            }

//            return sb.ToString();
//        }

//        internal string ReadPlpUnicodeChars(ulong plpLength)
//        {
//            if (plpLength == 0)
//                return string.Empty;

//            var chungLength = ReadUInt32(); //chunck is always smaller than packetsize
//            if (chungLength == 0)
//                return string.Empty;

//            // If total length is known up front, allocate the whole buffer in one shot instead of realloc'ing and copying over each time

//            if (plpLength == chungLength)
//            {
//                var v = ReadUnicodeChars((int) chungLength);
//                ReadUInt32(); //read the 0 chunklen
//                return v;
//            }

//            var sb = new StringBuilder();
//            byte? byte1 = null;
//            while (true)
//            {
//                if (byte1 != null)
//                {
//                    var byte2 = ReadByte();
//                    sb.Append((char) ((int) (byte1 << 8) + byte2));
//                    chungLength--;
//                    byte1 = null;
//                }

//                var byteToRead = chungLength & 0xFFFFFFFE;
//                if (byteToRead != chungLength)
//                {
//                    ReadUnicodeChars(sb, (int) byteToRead);
//                    byte1 = ReadByte();
//                }
//                else
//                {
//                    ReadUnicodeChars(sb, (int) chungLength);
//                }

//                // Read the next chunk or cleanup state if hit the end
//                chungLength = ReadUInt32();
//                if (chungLength == 0) // Data read complete. bytesleft>0 if len > blob.Length and SQL_PLP_UNKNOWNLEN
//                    break;
//            }

//            return sb.ToString();
//        }

//        public string ReadString(Encoding encoding, int length)
//        {
//            var packageLength = _readEndPos - _pos;
//            var count = length > packageLength ? packageLength : length;

//            var str = encoding.GetString(ReadBuffer, _pos, count);
//            _pos += count;
//            return str;
//        }

//        public SqlCollations ReadCollation()
//        {
//            return new SqlCollations
//            {
//                Info = ReadUInt32(),
//                SortId = ReadByte()
//            };
//        }
//    }
//}