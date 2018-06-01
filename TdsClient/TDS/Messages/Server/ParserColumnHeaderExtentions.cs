using Medella.TdsClient.Contants;
using Medella.TdsClient.TDS.Package.Reader;

namespace Medella.TdsClient.TDS.Messages.Server
{
    public static class ParserTokenExtentions
    {
        public static int GetTokenLength(this TdsPackageReader reader, byte token)
        {
            switch (token & TdsEnums.SQLLenMask)
            {
                case TdsEnums.SQLZeroLen:
                    return 0;
                case TdsEnums.SQLFixedLen:
                    return (0x01 << ((token & 0x0c) >> 2)) ;
                default:
                    switch (token)
                    {
                        case TdsEnums.SQLFEATUREEXTACK:
                            return -1;
                        case TdsEnums.SQLSESSIONSTATE:
                            return reader.ReadInt32(); ;
                        case TdsEnums.SQLRETURNVALUE:
                            return -1; // In Yukon, the RETURNVALUE token stream no longer has length
                        default:
                            return reader.ReadUInt16();
                    }
            }
        }
    }
}