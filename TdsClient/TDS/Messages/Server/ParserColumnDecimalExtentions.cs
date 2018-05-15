using System;
using Medella.TdsClient.TDS.Package;
using Medella.TdsClient.TDS.Package.Reader;

namespace Medella.TdsClient.TDS.Messages.Server
{
    public static class ParserColumnDecimalExtentions
    {
        public static decimal ReadSqlDecimal(this TdsPackageReader reader, int length, byte scale)
        {
            var fPositive = 1 == reader.ReadByte();

            switch (length)
            {
                case 5:
                    return new decimal(reader.ReadInt32(), 0, 0, !fPositive, scale);
                case 9:
                    return new decimal(reader.ReadInt32(), reader.ReadInt32(), 0, !fPositive, scale);
                case 13:
                    return new decimal(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), !fPositive, scale);
                case 17:
                    var v = new decimal(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), !fPositive, scale);
                    if (reader.ReadInt32() != 0) throw new Exception($"value to big:{length}");
                    return v;
            }

            throw new Exception($"Can not read decimal with length:{length}");
        }

        public static decimal ReadSqlMoney(this TdsPackageReader reader, int length)
        {
            if (length == 8)
            {
                var mid = reader.ReadInt32();
                var l =((long)mid << 0x20) + reader.ReadUInt32();
                if (l > 0)
                    return new decimal((int) (l & 0xffffffff), (int) (l >> 32), 0, false, 4);
                l = -l;
                return new decimal((int)(l & 0xffffffff), (int)(l >> 32), 0, true, 4);
            }

            var lo =  reader.ReadInt32();

            return lo >= 0
                ? new decimal((int)(lo & 0xffffffff), 0, 0, false, 4)
                : new decimal((int)(-lo & 0xffffffff), 0, 0, true, 4);
        }
    }
}