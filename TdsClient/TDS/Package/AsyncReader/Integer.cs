using System;

namespace Medella.TdsClient.TDS.Package.AsyncReader
{
    public partial class AsyncPackageReader
    {
        public byte ReadByte()
        {
            return ReadBuffer[_pos++];
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

        public long ReadInt64()
        {
            var v = BitConverter.ToInt64(ReadBuffer, _pos);
            _pos += 8;
            return v;
        }
    }
}