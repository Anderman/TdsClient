using System.Runtime.CompilerServices;
using Medella.TdsClient.Contants;
using Medella.TdsClient.TdsStream;
using Medella.TdsClient.TDS.Messages.Server.Internal;

namespace Medella.TdsClient.TDS.Package.Writer
{
    public partial class TdsPackageWriter
    {
        private const int BufferSize = 8000;
        private readonly ITdsStream _tdsStream;
        private byte _packageNumber;
        private int _packageStart;
        public byte[] WriteBuffer = new byte[BufferSize + TdsEnums.MaxSizeSqlValue]; // expand the buffer so that we can make correction after write
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CheckBuffer()
        {
            if (WritePosition >= BufferSize)
                SendBatchPackage();
        }

        public void SendBatchPackage()
        {
            var length = WritePosition - BufferSize;
            WritePosition = BufferSize;
            SetHeader(TdsEnums.ST_BATCH);

            _tdsStream.FlushBuffer(WriteBuffer, BufferSize);
            for (var i = 0; i < length; i++)
                WriteBuffer[i + 8] = WriteBuffer[BufferSize + i];
            WritePosition = 8 + (length > 0 ? length : 0);
            _packageStart = 0;
            _packageNumber++;
        }

        public void SetHeader(byte status)
        {
            var length = WritePosition - _packageStart;

            WriteBuffer[_packageStart + 1] = status;
            WriteBuffer[_packageStart + 2] = (byte) (length >> 8); // length - upper byte
            WriteBuffer[_packageStart + 3] = (byte) (length & 0xff); // length - lower byte
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
    }
}