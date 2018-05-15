using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Medella.TdsClient.Contants;
using Medella.TdsClient.TdsStream;

namespace Medella.TdsClient.TDS.Package.Reader
{
    public partial class TdsPackageReader
    {
        private const int BufferSize = 8000;
        private const int MaxSizeSqlValue = 1 + 1 + 4 * 4; //nullable decimal 
        private const int Guidsize = 16;
        private readonly byte[] _splitValueBuffer = new byte[MaxSizeSqlValue];
        private readonly ITdsStream _tdsStream;
        private bool _isSplitValueBuffer;
        private int _packageEnd;
        private byte _packageStatus;
        private int _pos;
        private int _readEndPos;
        private int _savedPos;
        private byte[] _savedReadBuffer;
        private int _savedReadEndPos;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CheckBuffer(int size)
        {
            if (size > _readEndPos - _pos)
                GetPackage(size, _readEndPos - _pos);
        }

        private void GetPackage(int size, int left)
        {
            if (left > 0)
            {
                ReadBuffer = SetupSplitValueBuffer(size, left);
                _isSplitValueBuffer = true;
                return;
            }

            if (_isSplitValueBuffer)
            {
                RestoreReadbuffer();
                _isSplitValueBuffer = false;
                return;
            }

            Receive();
            _pos = +8;
        }

        private void RestoreReadbuffer()
        {
            ReadBuffer = _savedReadBuffer;
            _readEndPos = _savedReadEndPos;
            _pos = _savedPos;
        }

        private byte[] SetupSplitValueBuffer(int size, int left)
        {
            Buffer.BlockCopy(ReadBuffer, _pos, _splitValueBuffer, 0, left);
            _savedReadEndPos = _tdsStream.Receive(ReadBuffer, 0, BufferSize);
            _savedReadBuffer = ReadBuffer;
            _savedPos = size - left + 8;
            Buffer.BlockCopy(ReadBuffer, TdsEnums.HEADER_LEN, _splitValueBuffer, left, size - left);
            _pos = 0;
            _readEndPos = size;
            return _splitValueBuffer;
        }

        public void Receive()
        {
            _readEndPos = _tdsStream.Receive(ReadBuffer, 0, BufferSize);
            _packageStatus = ReadBuffer[1];
            _packageEnd = (ReadBuffer[TdsEnums.HEADER_LEN_FIELD_OFFSET] << 8) | ReadBuffer[TdsEnums.HEADER_LEN_FIELD_OFFSET + 1];
            if (_readEndPos != _packageEnd)
                if (_readEndPos > _packageEnd)
                    throw new Exception("read more than one package");
                else
                    throw new Exception("read less than one package");
            _pos = 0;
        }

        public void PackageDone()
        {
            _pos = _packageEnd;
        }

        public int GetReadPos()
        {
            return _pos;
        }

        public int GetReadEndPos()
        {
            return _readEndPos;
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

        public void SkipBytes(int length)
        {
            CheckBuffer(length);
            _pos += length;
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
                if (length > 0) Receive();
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