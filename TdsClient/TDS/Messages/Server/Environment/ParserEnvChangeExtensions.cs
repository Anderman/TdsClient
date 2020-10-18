using System;
using System.Diagnostics;
using Medella.TdsClient.Constants;
using Medella.TdsClient.TDS.Package.Reader;

namespace Medella.TdsClient.TDS.Messages.Server.Environment
{
    public static class ParserEnvChangeExtensions
    {
        public static void EnvChange(this TdsPackageReader reader, int tokenLength, Action<int> transactionAction)
        {
            // There could be multiple environment change messages following this token.
            var processedLength = 0;

            var startpos = reader.GetReadPos();
            while (tokenLength > processedLength)
            {
                var type = reader.ReadByte();
                switch (type)
                {
                    case TdsEnums.ENV_DATABASE:
                    case TdsEnums.ENV_LANG:
                        reader.EnvLanguage();
                        break;

                    case TdsEnums.ENV_CHARSET:
                        reader.EnvCharset();
                        break;

                    case TdsEnums.ENV_PACKETSIZE:
                        reader.EnvPackageSize();
                        break;

                    case TdsEnums.ENV_LOCALEID:
                        throw new NotImplementedException();

                    case TdsEnums.ENV_COMPFLAGS:
                        throw new NotImplementedException();

                    case TdsEnums.ENV_COLLATION:
                        reader.EnvCollation();
                        break;

                    case TdsEnums.ENV_BEGINTRAN:
                    case TdsEnums.ENV_COMMITTRAN:
                    case TdsEnums.ENV_ROLLBACKTRAN:
                    case TdsEnums.ENV_ENLISTDTC:
                    case TdsEnums.ENV_DEFECTDTC:
                    case TdsEnums.ENV_TRANSACTIONENDED:
                        if (transactionAction == null)
                            reader.EnvSqlTransaction();
                        else
                            transactionAction(type);
                        break;

                    case TdsEnums.ENV_LOGSHIPNODE:
                        throw new NotImplementedException();

                    case TdsEnums.ENV_PROMOTETRANSACTION:
                        throw new NotImplementedException();

                    case TdsEnums.ENV_TRANSACTIONMANAGERADDRESS:
                    case TdsEnums.ENV_SPRESETCONNECTIONACK:
                        throw new NotImplementedException();

                    case TdsEnums.ENV_USERINSTANCE:
                        throw new NotImplementedException();

                    case TdsEnums.ENV_ROUTING:
                        throw new NotImplementedException();

                    default:
                        Debug.Assert(false, "Unknown environment change token: " + type);
                        break;
                }

                processedLength = reader.GetReadPos() - startpos;
            }
        }
    }
}