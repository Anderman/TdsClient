using System;

namespace Medella.TdsClient.TDS.Package.Reader
{
    public partial class TdsPackageReader
    {
        public decimal ReadSqlMoney(int length)
        {
            if (length == 8)
            {
                var mid = ReadInt32();
                var l = ((long)mid << 0x20) + ReadUInt32();
                if (l > 0)
                    return new decimal((int)(l & 0xffffffff), (int)(l >> 32), 0, false, 4);
                l = -l;
                return new decimal((int)(l & 0xffffffff), (int)(l >> 32), 0, true, 4);
            }

            var lo = ReadInt32();

            return lo >= 0
                ? new decimal((int)(lo & 0xffffffff), 0, 0, false, 4)
                : new decimal((int)(-lo & 0xffffffff), 0, 0, true, 4);
        }

        public decimal ReadSqlDecimal(int length, byte scale)
        {
            var fPositive = 1 == ReadByte();

            switch (length)
            {
                case 5:
                    return new decimal(ReadInt32(), 0, 0, !fPositive, scale);
                case 9:
                    return new decimal(ReadInt32(), ReadInt32(), 0, !fPositive, scale);
                case 13:
                    return new decimal(ReadInt32(), ReadInt32(), ReadInt32(), !fPositive, scale);
                case 17:
                    var v = new decimal(ReadInt32(), ReadInt32(), ReadInt32(), !fPositive, scale);
                    if (ReadInt32() != 0) throw new Exception($"value to big:{length}");
                    return v;
            }

            throw new Exception($"Can not read decimal with length:{length}");
        }
    }
}