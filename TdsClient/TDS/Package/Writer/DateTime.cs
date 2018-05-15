using System;
using Medella.TdsClient.Contants;

// ReSharper disable once CheckNamespace
namespace Medella.TdsClient.TDS.Package.Writer
{
    public partial class TdsPackageWriter
    {
        public static readonly DateTime BaseDate1900 = new DateTime(1900, 1, 1);
        public static readonly DateTime BaseDate1 = new DateTime(1, 1, 1);

        public void WriteDate(DateTime value)
        {
            WriteDateUnchecked(value);
            CheckBuffer();
        }

        public void WriteSqlTime(TimeSpan value, byte scale)
        {
            WriteSqlTimeUnchecked(value, scale);
            CheckBuffer();
        }
        public void WriteSqlDateTime4(DateTime value)
        {
            WriteSqlDateTime4Uncheked(value);
            CheckBuffer();
        }
        public void WriteSqlDateTime(DateTime value)
        {
            WriteSqlDateTimeUnchecked(value);
            CheckBuffer();
        }

        public void WriteSqlDateTimeOffset(DateTimeOffset value, byte scale)
        {
            WriteSqlDateTimeOffsetUnchecked(value, scale);
            CheckBuffer();
        }

        private void WriteSqlDateTimeOffsetUnchecked(DateTimeOffset value, byte scale)
        {
            value = value.Subtract(value.Offset);
            WriteSqlTimeUnchecked(value.TimeOfDay, scale);
            WriteDateUnchecked(value.DateTime);
            WriteInt16Unchecked(value.Offset.Minutes);
        }

        private void WriteSqlTimeUnchecked(TimeSpan value, byte scale)
        {
            var v = (ulong)(value.Ticks / TdsEnums.TICKS_FROM_SCALE[scale]);
            WriteBuffer[WritePosition++] = (byte)v;
            WriteBuffer[WritePosition++] = (byte)(v >> 8);
            WriteBuffer[WritePosition++] = (byte)(v >> 16);
            if (scale > 2) WriteBuffer[WritePosition++] = (byte)(v >> 24);
            if (scale > 4) WriteBuffer[WritePosition++] = (byte)(v >> 32);
        }

        private void WriteSqlDateTime4Uncheked(DateTime value)
        {
            var datepart = (ushort)value.Subtract(BaseDate1900).Days;
            var timepart = (ushort)value.TimeOfDay.TotalMinutes;
            WriteInt16Unchecked(datepart);
            WriteInt16Unchecked(timepart);
        }


        private void WriteSqlDateTimeUnchecked(DateTime value)
        {
            var datepart = value.Subtract(BaseDate1900).Days;
            var timepart = (int)value.TimeOfDay.TotalSeconds * 300;
            WriteInt32Unchecked(datepart);
            WriteInt32Unchecked(timepart);
        }

        private void WriteDateUnchecked(DateTime value)
        {
            var datepart = value.Subtract(BaseDate1).Days;
            WriteBuffer[WritePosition++] = (byte)datepart;
            WriteBuffer[WritePosition++] = (byte)(datepart >> 8);
            WriteBuffer[WritePosition++] = (byte)(datepart >> 16);
        }

        public void WriteDateTime(DateTime value)
        {
            int dayPart, timepart;
            (dayPart, timepart) = ConverDateTime(value);
            WriteInt32Unchecked(dayPart);
            WriteInt32Unchecked(timepart);
        }

        public static (int dayPart, int timepart) ConverDateTime(DateTime value)
        {
            const double sqlTicksPerMillisecond = 0.3;
            var baseDate = BaseDate1900;
            var dayPart = value.Subtract(baseDate).Days;
            var timepart = (int)(value.TimeOfDay.Ticks / (double)TimeSpan.TicksPerMillisecond * sqlTicksPerMillisecond);
            return (dayPart, timepart);
        }
    }
}