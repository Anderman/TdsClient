using System;
using Medella.TdsClient.Contants;

// ReSharper disable once CheckNamespace
namespace Medella.TdsClient.TDS.Package
{
    public partial class TdsPackageWriter
    {
        public void WriteNullableSqlDate(DateTime? value)
        {
            WriteBuffer[WritePosition++] = (byte) (value == null ? 0 : 3);
            if (value != null)
                WriteDateUnchecked((DateTime) value);
            CheckBuffer();
        }

        public void WriteNullableSqlTime(TimeSpan? value, byte scale)
        {
            var len = scale <= 2 ? 3 : scale <= 4 ? 4 : 5;
            WriteBuffer[WritePosition++] = (byte) (value == null ? 0 : len);
            if (value != null)
                WriteSqlTimeUnchecked((TimeSpan) value, scale);
            CheckBuffer();
        }

        public void WriteNullableSqlDateTime2(DateTime? value, byte scale)
        {
            var len = scale <= 2 ? 6 : scale <= 4 ? 7 : 8;
            WriteBuffer[WritePosition++] = (byte) (value == null ? 0 : len);
            if (value != null)
            {
                WriteSqlTimeUnchecked(value.Value.TimeOfDay, scale);
                WriteDateUnchecked((DateTime) value);
            }
            CheckBuffer();
        }

        public void WriteNullableSqlDateTimeOffset(DateTimeOffset? value, byte scale)
        {
            var len = scale <= 2 ? 8 : scale <= 4 ? 9 : 10;
            WriteBuffer[WritePosition++] = (byte) (value == null ? 0 : len);
            if (value != null)
                WriteSqlDateTimeOffsetUnchecked((DateTimeOffset) value, scale);
            CheckBuffer();
        }

        public void WriteNullableSqlDateTime(DateTime? value, int len)
        {
            WriteBuffer[WritePosition++] = (byte) (value == null ? 0 : len);
            if (value != null)
                if (len == 4)
                    WriteSqlDateTime4Uncheked((DateTime) value);
                else
                    WriteSqlDateTimeUnchecked((DateTime) value);
            CheckBuffer();
        }
    }
}