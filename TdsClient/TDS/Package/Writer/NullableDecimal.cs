using System;
using Medella.TdsClient.Contants;

// ReSharper disable once CheckNamespace
namespace Medella.TdsClient.TDS.Package.Writer
{
    public partial class TdsPackageWriter
    {
        public void WriteNullableSqlMoney4(decimal? value)
        {
            WriteBuffer[WritePosition++] = (byte)(value == null ? 0 : 4);
            if (value != null)
                WriteSqlMoney4Unchecked((decimal)value);
            CheckBuffer();
        }

        public void WriteNullableSqlMoney(decimal? value)
        {
            WriteBuffer[WritePosition++] = (byte)(value == null ? 0 : 8);
            if (value != null)
                WriteSqlMoneyUnchecked((decimal)value);
            CheckBuffer();
        }
        public void WriteNullableSqlDecimal(decimal? value, byte p, byte scale)
        {
            var len = p <= 9 ? 5
                : p <= 19 ? 9
                : p <= 28 ? 13
                : 17;
            WriteBuffer[WritePosition++] = (byte)(value == null ? 0 : len);
            if (value != null)
                WriteSqlDecimalUnchecked((decimal)value, len, scale);
            CheckBuffer();
        }



    }
}