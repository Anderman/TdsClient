using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Medella.TdsClient.Contants;
using Medella.TdsClient.SNI.SniNp;

namespace Medella.TdsClient.TDS.Package
{
    public class TdsPackageReader
    {
        private const int BufferSize = 16000;
        private const int Guidsize = 16;
        private readonly SniNpHandle _sniHandle;
        private int _packageEnd;
        private byte _packageStatus;
        private int _pos;
        private int _readEndPos;
        public byte[] ReadBuffer = new byte[BufferSize];

        public TdsPackageReader(SniNpHandle sniHandle)
        {
            _sniHandle = sniHandle;
        }

        public TdsSession CurrentSession { get; } = new TdsSession();
        public TdsResultset CurrentResultset { get; } = new TdsResultset();
        public TdsRow CurrentRow { get; } = new TdsRow();

        [Conditional("DEBUG")]
        public void GetBytesString(string prefix)
        {
            var sb = new StringBuilder($"{prefix}lentgh:{_packageEnd - _pos,4:##0} ");
            sb.Append("data: ");
            for (var i = _pos; i < _packageEnd; i++)
                sb.Append($"{ReadBuffer[i],2:X2} ");
            Debug.WriteLine(sb.ToString());
        }

        public void Receive(int minsize, int startPackage)
        {
            int len;
            do
            {
                len = _sniHandle.Receive(ReadBuffer, _readEndPos, BufferSize - _readEndPos);
                _readEndPos += len;
            } while (len > 0 && _readEndPos - startPackage < minsize);
        }

        public byte ReadPackage()
        {
            if (_pos < _packageEnd)
                return _packageStatus;
            if (_pos + 8 >= _readEndPos)
                CompletePackage(8);
            if (_readEndPos == 0) return 0;
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

        public byte ReadByte()
        {
            return ReadBuffer[_pos++];
        }

        public int ReadInt32()
        {
            var v = BitConverter.ToInt32(ReadBuffer, _pos);
            _pos += 4;
            return v;
        }

        public uint ReadUInt32()
        {
            var v = BitConverter.ToUInt32(ReadBuffer, _pos);
            _pos += 4;
            return v;
        }

        public short ReadInt16()
        {
            var v = BitConverter.ToInt16(ReadBuffer, _pos);
            _pos += 2;
            return v;
        }

        public ushort ReadUInt16()
        {
            var v = BitConverter.ToUInt16(ReadBuffer, _pos);
            _pos += 2;
            return v;
        }

        public long ReadInt64()
        {
            var v = BitConverter.ToInt64(ReadBuffer, _pos);
            _pos += 8;
            return v;
        }

        public Span<byte> GetBytes(int length)
        {
            if (length > _packageEnd - _pos) length = _packageEnd - _pos;
            var v = new Span<byte>(ReadBuffer, _pos, length);
            _pos += length;
            return v;
        }

        public byte[] ReadByteArray(byte[] dstArray, int offset, int length)
        {
            while (length > 0)
            {
                var packageLength = _packageEnd - _pos;
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
            StringBuilder sb = null;
            while (length > 0)
            {
                var packageLength = _packageEnd - _pos;
                var count = length > packageLength ? packageLength : length;

                var str = encoding.GetString(ReadBuffer, _pos, count);
                _pos += count;
                length -= count;
                if (length == 0) return str;
                if (sb == null)
                    sb = new StringBuilder(str, length);
                else sb.Append(str);
                ReadPackage();
            }

            return sb.ToString();
        }

        public void ReadString(StringBuilder sb, Encoding encoding, int length)
        {
            while (length > 0)
            {
                var packageLength = _packageEnd - _pos;
                var count = length > packageLength ? packageLength : length;

                var str = encoding.GetString(ReadBuffer, _pos, count);
                _pos += count;
                length -= count;
                sb.Append(str);
                if (length == 0) return;
                ReadPackage();
            }
        }

        public string ReadUnicodeChars(int length)
        {
            StringBuilder sb = null;
            while (length > 0)
            {
                var packageLength = _packageEnd - _pos;
                var count = length > packageLength ? packageLength : length;
                var unicode = MemoryMarshal.Cast<byte, char>(new ReadOnlySpan<byte>(ReadBuffer, _pos, count));
                _pos += count;

                if (length == count) return new string(unicode);
                length -= count;
                if (sb == null)
                    sb = new StringBuilder(new string(unicode), length);
                else sb.Append(unicode);
                ReadPackage();
            }

            return sb?.ToString();
        }

        public void ReadUnicodeChars(StringBuilder sb, int length)
        {
            while (length > 0)
            {
                var packageLength = _packageEnd - _pos;
                var count = length > packageLength ? packageLength : length;
                var unicode = MemoryMarshal.Cast<byte, char>(new ReadOnlySpan<byte>(ReadBuffer, _pos, count));
                _pos += count;

                length -= count;
                sb.Append(unicode);
                if (length == 0) return;
                ReadPackage();
            }
        }

        public T ReadType<T>(int length) where T : struct
        {
            var v = MemoryMarshal.Cast<byte, T>(new ReadOnlySpan<byte>(ReadBuffer, _pos, length))[0];
            _pos += length;
            return v;
        }

        public Guid ReadGuid()
        {
            var v = MemoryMarshal.Cast<byte, Guid>(new ReadOnlySpan<byte>(ReadBuffer, _pos, Guidsize))[0];
            _pos += Guidsize;
            return v;
        }

        public string ReadString(int length)
        {
            return ReadUnicodeChars(length * 2);
        }
    }
}