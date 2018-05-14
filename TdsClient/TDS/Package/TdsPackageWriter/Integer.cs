using System;
using System.Collections.Generic;
using System.Text;

// ReSharper disable once CheckNamespace
namespace Medella.TdsClient.TDS.Package
{
    public partial class TdsPackageWriter
    {

        public void WriteSqlBit(bool value)
        {
            WriteSqlBitUnchecked(value);
            CheckBuffer();
        }


        public void WriteByte(int v)
        {
            WriteSqlByteUnchecked((byte)v);
            CheckBuffer();
        }

        public void WriteInt16(int v)
        {
            WriteInt16Unchecked(v);
            CheckBuffer();
        }


        public void WriteUInt32(uint v)
        {
            WriteBuffer[WritePosition] = (byte)(v & 0xff);
            WriteBuffer[WritePosition + 1] = (byte)((v >> 8) & 0xff);
            WriteBuffer[WritePosition + 2] = (byte)((v >> 16) & 0xff);
            WriteBuffer[WritePosition + 3] = (byte)((v >> 24) & 0xff);
            WritePosition += sizeof(uint);
            CheckBuffer();
        }

        public void WriteInt32(int v)
        {
            WriteInt32Unchecked(v);
            CheckBuffer();
        }

        public void WriteInt64(long v)
        {
            WriteInt64Unchecked(v);
            CheckBuffer();
        }

        public void WriteUInt64(ulong v)
        {
            WriteBuffer[WritePosition] = (byte)(v & 0xff);
            WriteBuffer[WritePosition + 1] = (byte)((v >> 8) & 0xff);
            WriteBuffer[WritePosition + 2] = (byte)((v >> 16) & 0xff);
            WriteBuffer[WritePosition + 3] = (byte)((v >> 24) & 0xff);
            WriteBuffer[WritePosition + 4] = (byte)((v >> 32) & 0xff);
            WriteBuffer[WritePosition + 5] = (byte)((v >> 40) & 0xff);
            WriteBuffer[WritePosition + 6] = (byte)((v >> 48) & 0xff);
            WriteBuffer[WritePosition + 7] = (byte)((v >> 56) & 0xff);
            WritePosition += sizeof(ulong);
            CheckBuffer();
        }

        public void WriteSqlBitUnchecked(bool value)
        {
            WriteBuffer[WritePosition++] = (byte)(value ? 1 : 0);
        }
        public void WriteSqlByteUnchecked(byte value)
        {
            WriteBuffer[WritePosition++] = value;
        }
        private void WriteInt16Unchecked(int v)
        {
            WriteBuffer[WritePosition] = (byte)(v & 0xff);
            WriteBuffer[WritePosition + 1] = (byte)((v >> 8) & 0xff);
            WritePosition += sizeof(short);
        }
        private void WriteInt32Unchecked(int v)
        {
            WriteBuffer[WritePosition] = (byte)(v & 0xff);
            WriteBuffer[WritePosition + 1] = (byte)((v >> 8) & 0xff);
            WriteBuffer[WritePosition + 2] = (byte)((v >> 16) & 0xff);
            WriteBuffer[WritePosition + 3] = (byte)((v >> 24) & 0xff);
            WritePosition += sizeof(int);
        }

        private void WriteInt64Unchecked(long v)
        {
            WriteBuffer[WritePosition] = (byte)(v & 0xff);
            WriteBuffer[WritePosition + 1] = (byte)((v >> 8) & 0xff);
            WriteBuffer[WritePosition + 2] = (byte)((v >> 16) & 0xff);
            WriteBuffer[WritePosition + 3] = (byte)((v >> 24) & 0xff);
            WriteBuffer[WritePosition + 4] = (byte)((v >> 32) & 0xff);
            WriteBuffer[WritePosition + 5] = (byte)((v >> 40) & 0xff);
            WriteBuffer[WritePosition + 6] = (byte)((v >> 48) & 0xff);
            WriteBuffer[WritePosition + 7] = (byte)((v >> 56) & 0xff);
            WritePosition += 8;
        }


    }
}
