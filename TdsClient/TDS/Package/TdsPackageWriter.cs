using System;
using System.Text;
using Medella.TdsClient.Contants;
using Medella.TdsClient.SNI;
using Medella.TdsClient.TdsStream;
using Medella.TdsClient.TDS.Messages.Server.Internal;
using Medella.TdsClient.TDS.Reader.StringHelpers;

namespace Medella.TdsClient.TDS.Package
{
    public class TdsPackageWriter
    {
        private const int BufferSize = 8000;
        private readonly ITdsStream _tdsStream;
        private byte _packageNumber;
        private int _packageStart;
        public SqlCollations SqlCollation = new SqlCollations();
        public byte[] WriteBuffer = new byte[BufferSize];
        public int WritePosition;

        public TdsPackageWriter(ITdsStream tdsStream)
        {
            _tdsStream = tdsStream;
        }

        public string InstanceName => _tdsStream.InstanceName;
        public MetadataBulkCopy[] ColumnsMetadata { get; set; }

        public byte[] GetClientToken(byte[] servertoken)
        {
            return _tdsStream.GetClientToken(servertoken);
        }

        public void SendBatchPackage()
        {
            WritePosition = BufferSize;
            SetHeader(TdsEnums.ST_BATCH);
            _tdsStream.FlushBuffer(WriteBuffer, WritePosition);
            _packageStart = 0;
            WritePosition = 8;
            _packageNumber++;
        }

        public void SetHeader(byte status)
        {
            var length = WritePosition - _packageStart;

            WriteBuffer[_packageStart + 1] = status;
            WriteBuffer[_packageStart + 2] = (byte)(length >> 8); // length - upper byte
            WriteBuffer[_packageStart + 3] = (byte)(length & 0xff); // length - lower byte
            WriteBuffer[_packageStart + 4] = 0; // channel
            WriteBuffer[_packageStart + 5] = 0;
            WriteBuffer[_packageStart + 6] = _packageNumber; // packet
            WriteBuffer[_packageStart + 7] = 0; // window
        }

        public void NewPackage(byte messageType)
        {
            _packageNumber++;
            _packageStart = WritePosition;
            WriteBuffer[_packageStart + 0] = messageType;
            WritePosition += 8;
        }

        public void SendLastMessage()
        {
            SetHeader(TdsEnums.ST_EOM);
            _tdsStream.FlushBuffer(WriteBuffer, WritePosition);
            _packageNumber = 0;
            WritePosition = 0;
        }

        public void WriteByte(int v)
        {
            CheckBuffer(1);
            WriteBuffer[WritePosition] = (byte)(v & 0xff);
            WritePosition += 1;
        }

        public void WriteInt16(int v)
        {
            if (WritePosition + sizeof(short) > WriteBuffer.Length)
            {
                for (var i = 0; i < sizeof(short); i++) WriteByte((byte)((v >> (i * 8)) & 0xff));
                return;
            }
            WriteBuffer[WritePosition] = (byte)(v & 0xff);
            WriteBuffer[WritePosition + 1] = (byte)((v >> 8) & 0xff);
            WritePosition += sizeof(short);
        }

        public void WriteUInt32(uint v)
        {
            if (WritePosition + sizeof(uint) > WriteBuffer.Length)
            {
                for (var i = 0; i < sizeof(uint); i++) WriteByte((byte)((v >> (i * 8)) & 0xff));
                return;
            }
            WriteBuffer[WritePosition] = (byte)(v & 0xff);
            WriteBuffer[WritePosition + 1] = (byte)((v >> 8) & 0xff);
            WriteBuffer[WritePosition + 2] = (byte)((v >> 16) & 0xff);
            WriteBuffer[WritePosition + 3] = (byte)((v >> 24) & 0xff);
            WritePosition += sizeof(uint);
        }

        public void WriteInt32(int v)
        {
            if (WritePosition + sizeof(int) > WriteBuffer.Length)
            {
                for (var i = 0; i < sizeof(int); i++) WriteByte((byte)((v >> (i * 8)) & 0xff));
                return;
            }
            WriteBuffer[WritePosition] = (byte)(v & 0xff);
            WriteBuffer[WritePosition + 1] = (byte)((v >> 8) & 0xff);
            WriteBuffer[WritePosition + 2] = (byte)((v >> 16) & 0xff);
            WriteBuffer[WritePosition + 3] = (byte)((v >> 24) & 0xff);
            WritePosition += sizeof(int);
        }

        public void WriteInt64(long v)
        {
            if (WritePosition + sizeof(long) > WriteBuffer.Length)
            {
                for (var i = 0; i < sizeof(long); i++) WriteByte((byte)((v >> (i * 8)) & 0xff));
                return;
            }
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

        public void WriteUInt64(ulong v)
        {
            if (WritePosition + sizeof(ulong) > WriteBuffer.Length)
            {
                for (var i = 0; i < sizeof(ulong); i++) WriteByte((byte)((v >> (i * 8)) & 0xff));
                return;
            }
            WriteBuffer[WritePosition] = (byte)(v & 0xff);
            WriteBuffer[WritePosition + 1] = (byte)((v >> 8) & 0xff);
            WriteBuffer[WritePosition + 2] = (byte)((v >> 16) & 0xff);
            WriteBuffer[WritePosition + 3] = (byte)((v >> 24) & 0xff);
            WriteBuffer[WritePosition + 4] = (byte)((v >> 32) & 0xff);
            WriteBuffer[WritePosition + 5] = (byte)((v >> 40) & 0xff);
            WriteBuffer[WritePosition + 6] = (byte)((v >> 48) & 0xff);
            WriteBuffer[WritePosition + 7] = (byte)((v >> 56) & 0xff);
            WritePosition += sizeof(ulong);
        }

        public void WriteFloat(float v)
        {
            var b = BitConverter.GetBytes(v);
            if (WritePosition + sizeof(float) > WriteBuffer.Length)
            {
                for (var i = 0; i < sizeof(float); i++) WriteByte(b[i]);
                return;
            }
            WriteBuffer[WritePosition] = b[0];
            WriteBuffer[WritePosition + 1] = b[1];
            WriteBuffer[WritePosition + 2] = b[2];
            WriteBuffer[WritePosition + 3] = b[3];
            WritePosition += sizeof(float);
        }

        public void WriteDouble(double v)
        {
            var b = BitConverter.GetBytes(v);
            if (WritePosition + sizeof(double) > WriteBuffer.Length)
            {
                for (var i = 0; i < sizeof(double); i++) WriteByte(b[i]);
                return;
            }
            WriteBuffer[WritePosition] = b[0];
            WriteBuffer[WritePosition + 1] = b[1];
            WriteBuffer[WritePosition + 2] = b[2];
            WriteBuffer[WritePosition + 3] = b[3];
            WriteBuffer[WritePosition + 4] = b[4];
            WriteBuffer[WritePosition + 5] = b[5];
            WriteBuffer[WritePosition + 6] = b[6];
            WriteBuffer[WritePosition + 7] = b[7];
            WritePosition += sizeof(double);
        }

        public void WriteUnicodeString(string v)
        {
            if (v.Length == 0) return;
            if (v.Length * 2 <= WriteBuffer.Length - WritePosition)
            {
                WritePosition += Encoding.Unicode.GetBytes(v, 0, v.Length, WriteBuffer, WritePosition);
            }
            else
            {
                var buffer = new byte[v.Length * 2];
                var bytes = Encoding.Unicode.GetBytes(v, 0, v.Length, buffer, 0);
                WriteByteArray(buffer);
            }
        }

        public void WriteByteArray(byte[] src)
        {
            var length = src.Length;
            var bufferLength = WriteBuffer.Length;
            var srcOffset = 0;
            var bytesLeft = length - srcOffset;
            while (bytesLeft > bufferLength - WritePosition)
            {
                var count = bufferLength - WritePosition;
                Buffer.BlockCopy(src, srcOffset, WriteBuffer, WritePosition, count);
                srcOffset += count;
                bytesLeft -= count;
                SendBatchPackage();
            }

            Buffer.BlockCopy(src, srcOffset, WriteBuffer, WritePosition, bytesLeft);
            WritePosition += bytesLeft;
        }

        public void CheckBuffer(int length)
        {
            if (WritePosition + length > WriteBuffer.Length)
                SendBatchPackage();
        }
    }
}