using System;
using System.Globalization;
using Medella.TdsClient.Contants;
using Medella.TdsClient.TDS.Messages.Server;
using Medella.TdsClient.TDS.Messages.Server.Environment;

namespace Medella.TdsClient.TDS.Controller
{
    public class TdsStreamParser
    {
        private readonly TdsPackage _tdsPackage;
        private readonly LoginProcessor _loginProcessor;
        private bool _errorReceived;
        public ParseStatus Status { get; set; }

        public TdsStreamParser(TdsPackage tdsPackage, LoginProcessor loginProcessor)
        {
            _tdsPackage = tdsPackage;
            _loginProcessor = loginProcessor;
        }
        public void ParseInput()
        {
            ParseInput(null);
        }

        internal void ParseInput(Action<int> colMetaDataParser)
        {
            Status = ParseStatus.Unknown;
            while (_tdsPackage.Reader.ReadPackage() != 255)
            {
                //_tdsPackage.Reader.GetBytesString("ParseInput: "); // GetBytesString("Invalid token:", _tdsPackage.Reader.ReadBuffer, _tdsPackage.Reader.GetReadPos(), _tdsPackage.Reader.PackageBytesLeft());
                var token = _tdsPackage.Reader.ReadByte();

                var tokenLength = _tdsPackage.Reader.GetTokenLength(token);
                switch (token)
                {
                    case TdsEnums.SQLERROR:
                    case TdsEnums.SQLINFO:
                    {
                        if (token == TdsEnums.SQLERROR) _errorReceived = true;
                        _tdsPackage.Reader.SqlErrorAndInfo(token, tokenLength);
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
                        var isDone = _tdsPackage.Reader.SqlDone();
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
                        _tdsPackage.Reader.EnvChange(tokenLength);
                        // ENVCHANGE must be processed synchronously (since it can modify the state of many objects)
                        break;
                    }
                    case TdsEnums.SQLLOGINACK:
                    {
                        var serverInfo = _tdsPackage.Reader.LoginAck();
                        break;
                    }
                    case TdsEnums.SQLFEATUREEXTACK:
                    {
                        _tdsPackage.Reader.FeatureExtAck();
                        break;
                    }
                    case TdsEnums.SQLSESSIONSTATE:
                    {
                        _tdsPackage.Reader.ParseSessionState(tokenLength);
                        break;
                    }
                    case TdsEnums.SQLCOLMETADATA:
                    {
                        if (colMetaDataParser!=null)
                            colMetaDataParser(tokenLength);
                        else
                            _tdsPackage.Reader.ColMetaData(tokenLength);
                        break;
                    }
                    case TdsEnums.SQLROW:
                    {
                        _tdsPackage.Reader.CurrentRow.IsNbcRow = false;
                        Status = ParseStatus.Row;
                        return;
                    }
                    case TdsEnums.SQLNBCROW:
                    {
                        _tdsPackage.Reader.SaveNbcBitmap();
                        Status = ParseStatus.Row;
                        return;
                    }
                    case TdsEnums.SQLRETURNSTATUS:
                        _tdsPackage.Reader.GetBytes(tokenLength);
                        break;
                    case TdsEnums.SQLRETURNVALUE:
                    {
                        throw new NotImplementedException();
                    }
                    case TdsEnums.SQLSSPI:
                    {
                        _loginProcessor.VerifyToken(tokenLength);
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
        }

    }
}