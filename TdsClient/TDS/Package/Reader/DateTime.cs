using System;
using System.Data.SqlTypes;
using Medella.TdsClient.Contants;

namespace Medella.TdsClient.TDS.Package.Reader
{
    public partial class TdsPackageReader
    {
        public static readonly DateTime BaseDate1 = new DateTime(1, 1, 1);

        public DateTime ReadSqlDate()
        {
            var v = ReadByte() + (ReadByte() << 8) + (ReadByte() << 16);
            return BaseDate1.AddDays(v);
        }

        public TimeSpan ReadSqlTime(int length, byte scale)
        {
            var tickUnits = ReadByte() + ((long) ReadByte() << 8) + ((long) ReadByte() << 16);
            if (length > 3)
                tickUnits += (long) ReadByte() << 24;
            if (length > 4)
                tickUnits += (long) ReadByte() << 32;
            var ticks = tickUnits * TdsEnums.TICKS_FROM_SCALE[scale];
            return new TimeSpan(ticks);
        }

        public DateTime ReadSqlDateTime(int length, byte scale)
        {
            var timespan = ReadSqlTime(length - 3, scale);
            var date = ReadSqlDate();
            return date.AddTicks(timespan.Ticks);
        }

        public DateTimeOffset ReadSqlDateTimeOffset(int length, byte scale)
        {
            var timespan = ReadSqlTime(length - 5, scale);
            var date = ReadSqlDate();
            var minutes = ReadInt16();
            var offset = new TimeSpan(0, minutes, 0);
            return new DateTimeOffset(date.AddTicks(timespan.Ticks + offset.Ticks), offset);
        }


        public DateTime ReadSqlDateTime(int length)
        {
            var daypart = length == 4 ? ReadUInt16() : ReadInt32();
            var timepart = length == 4 ? (uint) ReadUInt16() * SqlDateTime.SQLTicksPerMinute : ReadUInt32();
            // Values need to match those from SqlDateTime
            return ConvertSqlDate(daypart, timepart);
        }

        public static DateTime ConvertSqlDate(int daypart, long timepart)
        {
            const double SQLTicksPerMillisecond = 0.3;
            const int SQLTicksPerSecond = 300;
            const int SQLTicksPerMinute = SQLTicksPerSecond * 60;
            const int SQLTicksPerHour = SQLTicksPerMinute * 60;
            const int SQLTicksPerDay = SQLTicksPerHour * 24;
            const int MinDay = -53690; // Jan 1 1753
            const int MaxDay = 2958463; // Dec 31 9999 is this many days from Jan 1 1900
            const int MinTime = 0; // 00:00:0:000PM
            const int MaxTime = SQLTicksPerDay - 1; // = 25919999,  11:59:59:997PM

            if (daypart < MinDay || daypart > MaxDay || timepart < MinTime || timepart > MaxTime) throw new OverflowException(SQLResource.DateTimeOverflowMessage);

            var baseDateTicks = new DateTime(1900, 1, 1).Ticks;
            var dayticks = daypart * TimeSpan.TicksPerDay;
            var timeticks = (long) (timepart / SQLTicksPerMillisecond + 0.5) * TimeSpan.TicksPerMillisecond;

            return new DateTime(baseDateTicks + dayticks + timeticks);
        }
    }
}