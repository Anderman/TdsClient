using System;
using System.Globalization;
using System.Text;
using Medella.TdsClient.Contants;
using Medella.TdsClient.SNI;
using Medella.TdsClient.TDS.Controller.Sspi;
using Medella.TdsClient.TDS.Messages.Client;
using Medella.TdsClient.TDS.Messages.Server;
using Medella.TdsClient.TDS.Messages.Server.Environment;
using Medella.TdsClient.TDS.Messages.Server.Internal;
using Medella.TdsClient.TDS.Package;
using SqlClient;
using SqlClient.TDS.Messages.Server;

namespace Medella.TdsClient.TDS
{
    public enum ParseStatus
    {
        Row,
        Done,
        AltRow,
        Unknown,
        BeforeFirstColumn,
        AfterLastColumn
    }

    public class TdsController
    {
        private static readonly byte[] ValidTokens =
        {
            TdsEnums.SQLERROR,
            TdsEnums.SQLINFO,
            TdsEnums.SQLLOGINACK,
            TdsEnums.SQLENVCHANGE,
            TdsEnums.SQLRETURNVALUE,
            TdsEnums.SQLRETURNSTATUS,
            TdsEnums.SQLCOLNAME,
            TdsEnums.SQLCOLFMT,
            TdsEnums.SQLCOLMETADATA,
            TdsEnums.SQLALTMETADATA,
            TdsEnums.SQLTABNAME,
            TdsEnums.SQLCOLINFO,
            TdsEnums.SQLORDER,
            TdsEnums.SQLALTROW,
            TdsEnums.SQLROW,
            TdsEnums.SQLNBCROW,
            TdsEnums.SQLDONE,
            TdsEnums.SQLDONEPROC,
            TdsEnums.SQLDONEINPROC,
            TdsEnums.SQLROWCRC,
            TdsEnums.SQLSECLEVEL,
            TdsEnums.SQLPROCID,
            TdsEnums.SQLOFFSET,
            TdsEnums.SQLSSPI,
            TdsEnums.SQLFEATUREEXTACK,
            TdsEnums.SQLSESSIONSTATE
        };

        private static readonly bool[] ValidTokensLookup = new bool[256];
        private readonly TdsPackageReader _reader;
        private readonly TdsPackageWriter _writer;
        private readonly SspiHelper _sspi;

        public ParseStatus Status;
        private bool _errorReceived;

        static TdsController()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            foreach (var token in ValidTokens)
                ValidTokensLookup[token] = true;
        }

        public TdsController(TdsPackageReader reader, TdsPackageWriter writer, SspiHelper sspi)
        {
            _reader = reader;
            _writer = writer;
            _sspi = sspi;
        }

        private string GetBytesString(string prefix, byte[] buffer, int offset, int length)
        {
            var sb = new StringBuilder($"{prefix}lentgh:{length,4:##0} ");
            sb.Append("data: ");
            for (var i = 0; i < length; i++)
                sb.Append($"{buffer[offset + i],2:X2} ");
            return sb.ToString();
        }

        public void ParseInput()
        {
            Status = ParseStatus.Unknown;
            while (_reader.ReadPackage() > 0)
            {
                _reader.GetBytesString("ParseInput: ");// GetBytesString("Invalid token:", _reader.ReadBuffer, _reader.GetReadPos(), _reader.PackageBytesLeft());
                var token = _reader.ReadByte();
                if (!ValidTokensLookup[token]) throw new Exception($"Invalid Token {token}");

                var tokenLength = _reader.GetTokenLength(token);
                switch (token)
                {
                    case TdsEnums.SQLERROR:
                    case TdsEnums.SQLINFO:
                        {
                            if (token == TdsEnums.SQLERROR) _errorReceived = true;
                            var error = _reader.SqlErrorAndInfo(token, tokenLength);
                            //_pendingInfoEvents.Add(error);
                            break;
                        }

                    case TdsEnums.SQLCOLINFO:
                        {
                            throw new NotImplementedException();
                            //if (null != _dataStream)
                            //{
                            //    var metaDataSet = TryProcessColInfo(_dataStream.MetaData, _dataStream);
                            //    _dataStream.TrySetMetaData(metaDataSet, false);
                            //    _dataStream.BrowseModeInfoConsumed = true;
                            //}
                            //else
                            //{
                            //    SkipBytes(tokenLength);
                            //}

                            //break;
                        }

                    case TdsEnums.SQLDONE:
                    case TdsEnums.SQLDONEPROC:
                    case TdsEnums.SQLDONEINPROC:
                        {
                            var isDone = _reader.SqlDone();
                            if (isDone)
                            {
                                Status = ParseStatus.Done;
                                return;
                            }
                            break;
                        }

                    case TdsEnums.SQLORDER:
                        {
                            throw new NotImplementedException();
                        }

                    case TdsEnums.SQLALTMETADATA:
                        {
                            throw new NotImplementedException();
                        }

                    case TdsEnums.SQLALTROW:
                        {
                            throw new NotImplementedException();
                        }

                    case TdsEnums.SQLENVCHANGE:
                        {
                            _reader.EnvChange(tokenLength);
                            // ENVCHANGE must be processed synchronously (since it can modify the state of many objects)
                            break;
                        }
                    case TdsEnums.SQLLOGINACK:
                        {
                            var serverInfo = _reader.LoginAck();
                            break;
                        }
                    case TdsEnums.SQLFEATUREEXTACK:
                        {
                            _reader.FeatureExtAck();
                            break;
                        }
                    case TdsEnums.SQLSESSIONSTATE:
                    {
                        _reader.GetBytes(tokenLength);
                        break;
                            throw new NotImplementedException();
                        }
                    case TdsEnums.SQLCOLMETADATA:
                        {
                            _reader.CurrentResultset.ColumnsMetadata = _reader.ColMetaData(tokenLength);
                            break;
                        }
                    case TdsEnums.SQLROW:
                        {
                            _reader.CurrentRow.IsNbcRow = false;
                            Status = ParseStatus.Row;
                            return;
                        }
                    case TdsEnums.SQLNBCROW:
                        {
                            _reader.SaveNbcBitmap();
                            Status = ParseStatus.Row;
                            return;
                        }
                    case TdsEnums.SQLRETURNSTATUS:
                        throw new NotImplementedException();
                    case TdsEnums.SQLRETURNVALUE:
                        {
                            throw new NotImplementedException();
                        }
                    case TdsEnums.SQLSSPI:
                        {
                            _reader.ParseToken(_sspi, tokenLength);
                            if (_sspi.ClientToken.Length == 0) break;
                            _writer.SendSspi(_sspi.ClientToken);
                            break;
                        }
                    case TdsEnums.SQLTABNAME:
                        {
                            throw new NotImplementedException(nameof(TdsEnums.SQLTABNAME));
                        }

                    default:
                        throw new Exception("Unhandled token:  " + token.ToString(CultureInfo.InvariantCulture));
                }
            }
            Status = ParseStatus.Done;
            return;
        }
    }
}