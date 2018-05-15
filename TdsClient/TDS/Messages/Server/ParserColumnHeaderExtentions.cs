using Medella.TdsClient.Contants;
using Medella.TdsClient.TDS.Package.Reader;

namespace Medella.TdsClient.TDS.Messages.Server
{
    public static class ParserTokenExtentions
    {
        public static int GetTokenLength(this TdsPackageReader tdsPackageReader, byte token)
        {
            switch (token)
            {
                case TdsEnums.SQLFEATUREEXTACK:
                    return -1;
                case TdsEnums.SQLUDT:
                    // special case for UDTs
                    return -1; // Should we return -1 or not call GetTokenLength for UDTs?
                case TdsEnums.SQLSESSIONSTATE:
                    return tdsPackageReader.ReadInt32();
                case TdsEnums.SQLRETURNVALUE:
                    return -1; // In Yukon, the RETURNVALUE token stream no longer has length
            }

            switch (token & TdsEnums.SQLLenMask)
            {
                case TdsEnums.SQLZeroLen:
                    return 0;
                case TdsEnums.SQLFixedLen:
                    return (0x01 << ((token & 0x0c) >> 2)) & 0xff;
                default:
                    return tdsPackageReader.ReadUInt16();
            }
        }
    }
}