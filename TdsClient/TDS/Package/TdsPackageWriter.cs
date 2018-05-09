using System;
using System.Text;
using Medella.TdsClient.Contants;
using Medella.TdsClient.SNI;
using Medella.TdsClient.SNI.SniNp;
using Medella.TdsClient.TDS.Messages.Server.Internal;
using Medella.TdsClient.TDS.Reader.StringHelpers;

namespace Medella.TdsClient.TDS.Package
{
    public class TdsPackageWriter
    {
        private const int BufferSize = 8000;
        private readonly ISniHandle _sniHandle;
        private byte _packageNumber;
        private int _packageStart;
        public byte[] WriteBuffer = new byte[BufferSize];
        public int WritePosition;
        public SqlCollations SqlCollation = new SqlCollations();

        public TdsPackageWriter(ISniHandle sniHandle)
        {
            _sniHandle = sniHandle;
        }

        //public string ServerSpn => _sniHandle.ServerSpn;
        public string InstanceName => _sniHandle.InstanceName;
        public byte[] GetClientToken(byte[] servertoken) => _sniHandle.GetClientToken(servertoken);
        public MetadataBulkCopy[] ColumnsMetadata { get; set; }

        public void SendBatchPackage()
        {
            var length = WritePosition - _packageStart;
            WriteBuffer[_packageStart + 1] = TdsEnums.ST_BATCH;
            WriteBuffer[_packageStart + 2] = (byte)(length >> 8); // length - upper byte
            WriteBuffer[_packageStart + 3] = (byte)(length & 0xff); // length - lower byte
            FlushBuffer();
        }
        public void SetHeader(byte status, byte messageType)
        {
            var length = WritePosition - _packageStart;
            WriteBuffer[_packageStart + 0] = messageType;
            WriteBuffer[_packageStart + 1] = status;
            WriteBuffer[_packageStart + 2] = (byte)(length >> 8); // length - upper byte
            WriteBuffer[_packageStart + 3] = (byte)(length & 0xff); // length - lower byte
            WriteBuffer[_packageStart + 4] = 0; // channel
            WriteBuffer[_packageStart + 5] = 0;
            WriteBuffer[_packageStart + 6] = _packageNumber; // packet
            WriteBuffer[_packageStart + 7] = 0; // window
        }

        //Do we need this or can we create a new connection
        //private void SetSmpPackage()
        //{
        //    byte SMID = 83;
        //    var flags = (byte)SNISMUXFlags.SMUX_DATA;
        //    short sessionId = 1;
        //    var sequenceNumber = ((flags == (byte)SNISMUXFlags.SMUX_FIN) || (flags == (byte)SNISMUXFlags.SMUX_ACK)) ? _sequenceNumber - 1 : _sequenceNumber++;
        //    var length = WritePosition - _packageStart;
        //    WriteBuffer[_packageStart] = SMID;         // Message Type
        //    WriteBuffer[_packageStart + 1] = flags;
        //    WriteBuffer[_packageStart + 2] = (byte)(sessionId >> 8); // length - upper byte
        //    WriteBuffer[_packageStart + 3] = (byte)(sessionId & 0xff); // length - lower byte
        //    var pos = WritePosition;
        //    WritePosition = 4;
        //    WriteInt32(length);
        //    WriteInt32(sequenceNumber);
        //    WriteInt32(_highwater);
        //    WritePosition = pos;
        //    _packageStart += 0x10;
        //}

        //public void SendCtrlPackage()
        //{
        //    WritePosition = 16;
        //    _packageStart = 0;
        //    SetSmpPackage();
        //    _sniHandle.FlushBuffer(WriteBuffer, WritePosition);
        //}

        public void NewPackage()
        {
            _packageNumber++;
            _packageStart = WritePosition;
            WritePosition += 8;
        }

        public void FlushBuffer()
        {
            _sniHandle.FlushBuffer(WriteBuffer, WritePosition);
            if (WriteBuffer[_packageStart + 1] == TdsEnums.ST_EOM) _packageNumber = 0;
            WritePosition = 0;
        }

        public void WriteUInt32(uint v)
        {
            WriteBuffer[WritePosition] = (byte)(v & 0xff);
            WriteBuffer[WritePosition + 1] = (byte)((v >> 8) & 0xff);
            WriteBuffer[WritePosition + 2] = (byte)((v >> 16) & 0xff);
            WriteBuffer[WritePosition + 3] = (byte)((v >> 24) & 0xff);
            WritePosition += 4;
        }

        public void WriteInt32(int v)
        {
            WriteBuffer[WritePosition] = (byte)(v & 0xff);
            WriteBuffer[WritePosition + 1] = (byte)((v >> 8) & 0xff);
            WriteBuffer[WritePosition + 2] = (byte)((v >> 16) & 0xff);
            WriteBuffer[WritePosition + 3] = (byte)((v >> 24) & 0xff);
            WritePosition += 4;
        }

        public void WriteInt64(long v)
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
            WritePosition += 8;
        }
        public void WriteFloat(float v)
        {
            var b = BitConverter.GetBytes(v);
            WriteBuffer[WritePosition] = b[0];
            WriteBuffer[WritePosition + 1] = b[1];
            WriteBuffer[WritePosition + 2] = b[2];
            WriteBuffer[WritePosition + 3] = b[3];
            WritePosition += 4;
        }
        public void WriteDouble(double v)
        {
            var b = BitConverter.GetBytes(v);
            WriteBuffer[WritePosition] = b[0];
            WriteBuffer[WritePosition + 1] = b[1];
            WriteBuffer[WritePosition + 2] = b[2];
            WriteBuffer[WritePosition + 3] = b[3];
            WriteBuffer[WritePosition + 4] = b[4];
            WriteBuffer[WritePosition + 5] = b[5];
            WriteBuffer[WritePosition + 6] = b[6];
            WriteBuffer[WritePosition + 7] = b[7];
            WritePosition += 8;
        }

        public void WriteInt16(int v)
        {
            WriteBuffer[WritePosition] = (byte)(v & 0xff);
            WriteBuffer[WritePosition + 1] = (byte)((v >> 8) & 0xff);
            WritePosition += 2;
        }

        public void WriteByte(int v)
        {
            WriteBuffer[WritePosition] = (byte)(v & 0xff);
            WritePosition += 1;
        }

        public void WriteString(string v)
        {
            WritePosition += Encoding.Unicode.GetBytes(v, 0, v.Length, WriteBuffer, WritePosition);
        }


        public void WriteByteArray(byte[] src)
        {
            var length = src.Length;
            var bufferLength = WriteBuffer.Length;
            var srcOffset = 0;
            var bytesLeft= length- srcOffset;
            while (bytesLeft > bufferLength - WritePosition)
            {
                var count = (bufferLength - WritePosition);
                Buffer.BlockCopy(src, srcOffset, WriteBuffer, WritePosition, count);
                srcOffset += count;
                bytesLeft -= count;
                SendBatchPackage();
            }
            Buffer.BlockCopy(src, srcOffset, WriteBuffer, WritePosition, bytesLeft);
            WritePosition += bytesLeft;
        }
    }
}