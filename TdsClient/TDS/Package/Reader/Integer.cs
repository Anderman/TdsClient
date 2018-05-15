using System;
using System.Collections.Generic;
using System.Text;

namespace Medella.TdsClient.TDS.Package.Reader
{
    public partial class TdsPackageReader
    {
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
    }
}
