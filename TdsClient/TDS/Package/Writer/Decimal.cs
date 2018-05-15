// ReSharper disable once CheckNamespace
namespace Medella.TdsClient.TDS.Package.Writer
{
    public partial class TdsPackageWriter
    {
        internal static readonly long[] DecimalToScale =
        {
            1, //0
            10, //1
            100, //2
            1000, //3
            10000, //4
            100000, //5
            1000000, //6
            10000000, //7
            100000000, //8
            1000000000, //9
            10000000000, //10
            100000000000, //11
            1000000000000, //12
            10000000000000, //13
            100000000000000, //14
            1000000000000000 //15
        };

        public void WriteSqlMoney4(decimal value)
        {
            WriteSqlMoney4Unchecked(value);
            CheckBuffer();
        }

        private void WriteSqlMoney4Unchecked(decimal value)
        {
            WriteInt32Unchecked((int) (value * 10000));
        }

        public void WriteSqlMoney(decimal value)
        {
            WriteSqlMoneyUnchecked(value);
            CheckBuffer();
        }

        public void WriteSqlDecimal(decimal value, int length, byte toScale)
        {
            WriteSqlDecimalUnchecked(value, length, toScale);
            CheckBuffer();
        }
        public void WriteSqlDecimal(decimal value, int length)
        {
            WriteSqlDecimalUnchecked(value, length);
            CheckBuffer();
        }

        private void WriteSqlMoneyUnchecked(decimal value)
        {
            var v = (long) (value * 10000);
            WriteInt32Unchecked((int) (v >> 0x20));
            WriteInt32Unchecked((int) (v & 0xFFFF_FFFF));
        }
        private void WriteSqlDecimalUnchecked(decimal value, int length, byte toScale)
        {
            var fromScale = (byte)(decimal.GetBits(value)[3] >> 16);
            value = CorrectScale(value, fromScale, toScale);
            WriteSqlDecimalUnchecked(value, length);
        }

        private void WriteSqlDecimalUnchecked(decimal value, int length)
        {
            var bits = decimal.GetBits(value);
            WriteBuffer[WritePosition++] = (byte)(0x80000000 == (bits[3] & 0x80000000) ? 0 : 1);

            WriteInt32Unchecked(bits[0]);
            if (length >= 9) WriteInt32Unchecked(bits[1]);
            if (length >= 13) WriteInt32Unchecked(bits[2]);
            if (length == 17) WriteInt32Unchecked(0);
        }

        private static decimal CorrectScale(decimal value, byte fromScale, byte toScale)
        {
            var corr = toScale - fromScale;
            if (corr < 0)
                return decimal.Round(value, toScale);
            if (corr <= 15)
                return value * DecimalToScale[corr];
            value = value * DecimalToScale[15];
            return value * DecimalToScale[corr - 15];
        }

    }
}