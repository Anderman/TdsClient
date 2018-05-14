using System;
using Medella.TdsClient.Contants;

// ReSharper disable once CheckNamespace
namespace Medella.TdsClient.TDS.Package
{
    public partial class TdsPackageWriter
    {
        public void WriteNullableSqlBit(bool? value)
        {
            WriteBuffer[WritePosition++] = (byte)(value == null ? 0 : 1);
            if (value != null)
                WriteSqlBitUnchecked((bool)value);
            CheckBuffer();
        }
        public void WriteNullableSqlByte(byte? value)
        {
            WriteBuffer[WritePosition++] = (byte)(value == null ? 0 : 1);
            if (value != null)
                WriteSqlByteUnchecked((byte)value);
            CheckBuffer();
        }

        public void WriteNullableSqlInt16(short? value)
        {
            WriteBuffer[WritePosition++] = (byte)(value == null ? 0 : 2);
            if (value != null)
                WriteInt16Unchecked((short)value);
            CheckBuffer();
        }

        public void WriteNullableSqlInt32(int? value)
        {
            WriteBuffer[WritePosition++] = (byte)(value == null ? 0 : 4);
            if (value != null)
                WriteInt32Unchecked((int)value);
            CheckBuffer();
        }

        public void WriteNullableSqlInt64(long? value)
        {
            WriteBuffer[WritePosition++] = (byte)(value == null ? 0 : 8);
            if (value != null)
                WriteInt64Unchecked((long)value);
            CheckBuffer();
        }
    }
}