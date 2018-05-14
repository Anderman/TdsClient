using System;
using System.Diagnostics;
using System.Text;
using Medella.TdsClient.Contants;
using Medella.TdsClient.TdsStream;

namespace Medella.TdsClient.TDS.Package
{
    public class TdsPackageReader
    {
        private const int BufferSize = 16000;
        private const int Guidsize = 16;
        private readonly ITdsStream _tdsStream;
        private int _packageEnd;
        private byte _packageStatus;
        private int _pos;
        private int _readEndPos;
        public byte[] ReadBuffer = new byte[BufferSize];

        public TdsPackageReader(ITdsStream tdsStream)
        {
            _tdsStream = tdsStream;
        }

        public TdsSession CurrentSession { get; } = new TdsSession();
        public TdsResultset CurrentResultset { get; } = new TdsResultset();
        public TdsRow CurrentRow { get; } = new TdsRow();

        [Conditional("DEBUG")]
        public void WriteDebugString(string prefix)
        {
            var sb = new StringBuilder($"{prefix}lentgh:{_packageEnd - _pos,4:##0} ");
            sb.Append("data: ");
            for (var i = _pos; i < _packageEnd; i++)
                sb.Append($"{ReadBuffer[i],2:X2} ");
            Debug.WriteLine(sb.ToString());
        }

        public void CheckBuffer(int size)
        {
            var left = _readEndPos - _pos;
            if (size <= left)
                return;
            Buffer.BlockCopy(ReadBuffer, _pos, ReadBuffer, 0, left);
            _readEndPos = _tdsStream.Receive(ReadBuffer, left, BufferSize - left) + left;
            _pos = 8;
        }

        public void Receive(int minsize, int startPackage)
        {
            int len;
            do
            {
                len = _tdsStream.Receive(ReadBuffer, _readEndPos, BufferSize - _readEndPos);
                _readEndPos += len;
            } while (len > 0 && _readEndPos - startPackage < minsize);
        }

        public byte ReadPackage()
        {
            if (_pos < _packageEnd)
                return _packageStatus;
            if (_pos + 8 >= _readEndPos)
                CompletePackage(8);
            if (_readEndPos == 0) return 255;
            _packageStatus = ReadBuffer[_pos + 1];
            var size = (ReadBuffer[_pos + TdsEnums.HEADER_LEN_FIELD_OFFSET] << 8) | ReadBuffer[_pos + TdsEnums.HEADER_LEN_FIELD_OFFSET + 1];
            if (_pos + size > _readEndPos)
                CompletePackage(size);
            _packageEnd = _pos + size;
            _pos += 8;
            return _packageStatus;
        }

        public void PackageDone()
        {
            _pos = _packageEnd;
        }

        public void CompletePackage(int size)
        {
            if (_pos > 0)
            {
                Buffer.BlockCopy(ReadBuffer, _pos, ReadBuffer, 0, _readEndPos - _pos);
                _readEndPos = _readEndPos - _pos;
                _pos = 0;
            }

            if (size > _readEndPos)
                Receive(size, 0);
        }

        public int GetReadPos()
        {
            return _pos;
        }

        public int GetReadEndPos()
        {
            return _readEndPos;
        }

        public byte ReadByte()
        {
            CheckBuffer(1);
            return ReadBuffer[_pos++];
        }

        public int ReadInt32()
        {
            CheckBuffer(4);
            var v = BitConverter.ToInt32(ReadBuffer, _pos);
            _pos += 4;
            return v;
        }

        public uint ReadUInt32()
        {
            CheckBuffer(4);
            var v = BitConverter.ToUInt32(ReadBuffer, _pos);
            _pos += 4;
            return v;
        }

        public short ReadInt16()
        {
            CheckBuffer(2);
            var v = BitConverter.ToInt16(ReadBuffer, _pos);
            _pos += 2;
            return v;
        }

        public ushort ReadUInt16()
        {
            CheckBuffer(2);
            var v = BitConverter.ToUInt16(ReadBuffer, _pos);
            _pos += 2;
            return v;
        }

        public long ReadInt64()
        {
            CheckBuffer(8);
            var v = BitConverter.ToInt64(ReadBuffer, _pos);
            _pos += 8;
            return v;
        }

        public void GetBytes(byte[] dst, int length)
        {
            CheckBuffer(length);
            Buffer.BlockCopy(ReadBuffer, _pos, dst, 0, length);
            _pos += length;
        }

        public byte[] GetBytes(int length)
        {
            if (length == 0) return new byte[0];
            CheckBuffer(length);
            var v = new byte[length];
            Buffer.BlockCopy(ReadBuffer, _pos, v, 0, length);
            _pos += length;
            return v;
        }
        //public Span<byte> GetBytes(int length)
        //{
        //    if (length > _packageEnd - _pos) length = _packageEnd - _pos;
        //    var v = new Span<byte>(ReadBuffer, _pos, length);
        //    _pos += length;
        //    return v;
        //}

        public byte[] ReadByteArray(byte[] dstArray, int offset, int length)
        {
            CheckBuffer(length);
            while (length > 0)
            {
                var packageLength = _readEndPos - _pos;
                var count = length > packageLength ? packageLength : length;

                Buffer.BlockCopy(ReadBuffer, _pos, dstArray, offset, count);
                _pos += count;

                length -= count;
                offset += count;
                if (length > 0) ReadPackage();
            }

            return dstArray;
        }

        public string ReadString(Encoding encoding, int length)
        {
            CheckBuffer(length);
            var packageLength = _readEndPos - _pos;
            var count = length > packageLength ? packageLength : length;

            var str = encoding.GetString(ReadBuffer, _pos, count);
            _pos += count;
            return str;
        }

        public void ReadString(StringBuilder sb, Encoding encoding, int length)
        {
            CheckBuffer(length);
            var packageLength = _readEndPos - _pos;
            var count = length > packageLength ? packageLength : length;

            var str = encoding.GetString(ReadBuffer, _pos, count);
            _pos += count;
            sb.Append(str);
        }

        public string ReadUnicodeChars(int length)
        {
            CheckBuffer(length);
            var packageLength = _readEndPos - _pos;
            var count = length > packageLength ? packageLength : length;
            var unicode = Encoding.Unicode.GetString(ReadBuffer, _pos, count);
            //var unicode = MemoryMarshal.Cast<byte, char>(new ReadOnlySpan<byte>(ReadBuffer, _pos, count));
            _pos += count;
            return unicode;
        }

        public void ReadUnicodeChars(StringBuilder sb, int length)
        {
            CheckBuffer(length);
            var packageLength = _readEndPos - _pos;
            var count = length > packageLength ? packageLength : length;
            var unicode = Encoding.Unicode.GetString(ReadBuffer, _pos, count);
            //var unicode = MemoryMarshal.Cast<byte, char>(new ReadOnlySpan<byte>(ReadBuffer, _pos, count));
            _pos += count;
            sb.Append(unicode);
        }

        public float ReadFloat()
        {
            CheckBuffer(4);
            var v = BitConverter.ToSingle(ReadBuffer, _pos);
            _pos += 4;
            return v;
        }

        public double ReadDouble()
        {
            CheckBuffer(8);
            var v = BitConverter.ToDouble(ReadBuffer, _pos);
            _pos += 8;
            return v;
        }

        public Guid ReadGuid()
        {
            CheckBuffer(16);
            var b = ReadBuffer;
            var i = _pos;
            var guid = new Guid(new[] {b[i + 0], b[i + 1], b[i + 2], b[i + 3], b[i + 4], b[i + 5], b[i + 6], b[i + 7], b[i + 8], b[i + 9], b[i + 10], b[i + 11], b[i + 12], b[i + 13], b[i + 14], b[i + 15]});
            _pos += Guidsize;
            return guid;
        }

        public string ReadString(int length)
        {
            if (length == 0) return null;
            CheckBuffer(length);
            return ReadUnicodeChars(length * 2);
        }
    }
}