using System;
using System.Threading.Tasks;
using Medella.TdsClient.Contants;
using Medella.TdsClient.TdsStream;
using Medella.TdsClient.TDS.Controller;
using Medella.TdsClient.TDS.Package.Reader;

namespace Medella.TdsClient.TDS.Package.AsyncReader
{
    public partial class AsyncPackageReader
    {
        private readonly ITdsStream _tdsStream;
        private const int BufferSize = 16000;
        public byte[] ReadBuffer = new byte[BufferSize];
        private ParseState _status;
        private int _length;
        private int _pos;
        private byte _token;
        private int _packageEnd;
        private int _pos1;
        private int _goto;
        private short _errorLength;
        private SqlInfoAndError _error;
        public TdsSession CurrentSession { get; } = new TdsSession();
        public TdsResultset CurrentResultset { get; } = new TdsResultset();
        public TdsRow CurrentRow { get; } = new TdsRow();


        public AsyncPackageReader(ITdsStream tdsStream)
        {
            _tdsStream = tdsStream;
            _length = 1;
            _status = ParseState.token;
        }

        public async Task ReadAsync()
        {
            var bufferEnd = await _tdsStream.ReceiveAsync(ReadBuffer, 0, ReadBuffer.Length);
            _packageEnd = (ReadBuffer[TdsEnums.HEADER_LEN_FIELD_OFFSET] << 8) | ReadBuffer[TdsEnums.HEADER_LEN_FIELD_OFFSET + 1];
            var status = _status;
            var length = _length;
            switch (_status)
            {
                case ParseState.token:
                    _status = ParseState.tokenLen;
                    var token = _token = ReadBuffer[_pos++];
                    break;
            }
        }
        public void SqlErrorAndInfo()
        {
            if (_goto == 0) goto C0;
            if (_goto == 1) goto C1;
            if (_goto == 2) goto C2;
            if (_goto == 3) goto C3;
            if (_goto == 4) goto C4;
            if (_goto == 5) goto C5;
            C0:
            if (_packageEnd - _pos < 2) { _goto = 1; return; }
            C1:
            _errorLength = ReadInt16();
            if (_packageEnd - _pos < 8) { _goto = 2; return; }
            C2:
            _pos1 = _pos;
            _error = new SqlInfoAndError
            {
                Number = ReadInt32(),
                State = ReadByte(),
                Class = ReadByte(),
            };
            _length = ReadUInt16();
            if (_packageEnd - _pos < _length + 1) { _goto = 3; return; }
            C3:
            //_error.Message = ReadString(_length);
            _length = ReadByte();
            if (_packageEnd - _pos < _length + 1) { _goto = 4; return; }
            C4:
            //_error.Server = ReadString(_length);
            _length = ReadByte();
            if (_packageEnd - _pos < _length + 1) { _goto = 5; return; }
            C5:
            //_error.Procedure = ReadString(_length);
            _error.LineNumber = _errorLength - (2 + 1 + 1 + 2 + 2 * _error.Message.Length + 1 + 2 * _error.Server.Length + 1 + 2 * _error.Procedure.Length) > 2 ? ReadInt32() : ReadInt16();
            if (_error.Class >= TdsEnums.MIN_ERROR_CLASS)
                throw new Exception(_error.Message);
            CurrentSession.Errors.Add(_error);
        }
    }
}