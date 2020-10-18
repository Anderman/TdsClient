using Medella.TdsClient.Constants;
using Medella.TdsClient.Exceptions;
using Medella.TdsClient.TDS.Package.Reader;

namespace Medella.TdsClient.TDS.Messages.Server
{
    public static class ParserSqlLogin
    {
        public static SqlServerInfo LoginAck(this TdsPackageReader reader)
        {
            var sqlServerInfo = new SqlServerInfo();

            // read past interface type and version
            reader.ReadByte();

            var b = reader.GetBytes(TdsEnums.VERSION_SIZE);
            var tdsVersion = (uint)((((((b[0] << 8) | b[1]) << 8) | b[2]) << 8) | b[3]); // bytes are in motorola order (high byte first)
            var majorMinor = tdsVersion & 0xff00ffff;
            var increment = (tdsVersion >> 16) & 0xff;

            // Server responds:
            // 0x07000000 -> Sphinx         // Notice server response format is different for bwd compat
            // 0x07010000 -> Shiloh RTM     // Notice server response format is different for bwd compat
            // 0x71000001 -> Shiloh SP1
            // 0x72090002 -> Yukon RTM
            // 0x730B0003 -> Katmai RTM
            // 0x74000004 -> DENALI RTM
            // information provided by S. Ashwin

            switch (majorMinor)
            {
                case (TdsEnums.YUKON_MAJOR << 24) | TdsEnums.YUKON_RTM_MINOR when increment == TdsEnums.YUKON_INCREMENT:
                    sqlServerInfo.IsYukon = true;
                    break;
                case (TdsEnums.KATMAI_MAJOR << 24) | TdsEnums.KATMAI_MINOR when increment == TdsEnums.KATMAI_INCREMENT:
                    sqlServerInfo.IsYukon = true;
                    sqlServerInfo.IsKatmaiOrNewer = true;
                    break;
                case (TdsEnums.DENALI_MAJOR << 24) | TdsEnums.DENALI_MINOR when increment == TdsEnums.DENALI_INCREMENT:
                    sqlServerInfo.IsYukon = true;
                    sqlServerInfo.IsKatmaiOrNewer = true;
                    sqlServerInfo.IsDenali = true;
                    break;
                default:
                    throw SQL.InvalidTdsVersion();
            }
            //isYukon is always true otherwise we send an exception

            var len = reader.ReadByte();
            sqlServerInfo.Name = reader.ReadUnicodeChars(len * 2);
            sqlServerInfo.MajorVersion = reader.ReadByte();
            sqlServerInfo.MinorVersion = reader.ReadByte();
            sqlServerInfo.BuildNum = (short)((reader.ReadByte() << 8) + reader.ReadByte());
            return sqlServerInfo;
        }

        public class SqlServerInfo
        {
            public short BuildNum;
            public byte MajorVersion;
            public byte MinorVersion;
            public string? Name;
            public bool IsYukon { get; set; }
            public bool IsKatmaiOrNewer { get; set; }
            public bool IsDenali { get; set; }
        }
    }
}