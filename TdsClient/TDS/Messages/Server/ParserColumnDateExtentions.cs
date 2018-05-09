using System;
using System.Data.SqlTypes;
using System.Diagnostics;
using Medella.TdsClient.Contants;
using Medella.TdsClient.Exceptions;
using Medella.TdsClient.TDS.Messages.Client;
using Medella.TdsClient.TDS.Package;

namespace Medella.TdsClient.TDS.Messages.Server
{
    public static class ParserSessionStateExtentions
    {
        public static void ParseSessionState(this TdsPackageReader reader, int length)
        {
            var sdata = reader.CurrentSession.CurrentSessionData;
            if (length < 5) throw SQL.ParsingError();
            var seqNum = reader.ReadUInt32();

            if (seqNum == uint.MaxValue) reader.CurrentSession.DoNotPoolThisConnection = true;
            var status = reader.ReadByte();
            if (status > 1) throw SQL.ParsingError();
            var recoverable = status != 0;
            length -= 5;

            while (length > 0)
            {
                var stateId = reader.ReadByte();
                var stateLenByte = reader.ReadByte();
                var stateLen = stateLenByte < 0xFF
                    ? stateLenByte
                    : reader.ReadInt32();

                byte[] buffer = null;
                lock (sdata.Delta)
                {
                    if (sdata.Delta[stateId] == null)
                    {
                        buffer = new byte[stateLen];
                        sdata.Delta[stateId] = new SessionStateRecord { Version = seqNum, DataLength = stateLen, Data = buffer, Recoverable = recoverable };
                        sdata.DeltaDirty = true;
                        if (!recoverable)
                            checked
                            {
                                sdata.UnrecoverableStatesCount++;
                            }
                    }
                    else
                    {
                        if (sdata.Delta[stateId].Version <= seqNum)
                        {
                            var sv = sdata.Delta[stateId];
                            sv.Version = seqNum;
                            sv.DataLength = stateLen;
                            if (sv.Recoverable != recoverable)
                            {
                                if (recoverable)
                                {
                                    Debug.Assert(sdata.UnrecoverableStatesCount > 0, "Unrecoverable states count >0");
                                    sdata.UnrecoverableStatesCount--;
                                }
                                else
                                {
                                    checked
                                    {
                                        sdata.UnrecoverableStatesCount++;
                                    }
                                }

                                sv.Recoverable = recoverable;
                            }

                            buffer = sv.Data;
                            if (buffer.Length < stateLen)
                            {
                                buffer = new byte[stateLen];
                                sv.Data = buffer;
                            }
                        }
                    }
                }

                if (buffer != null)
                    reader.ReadByteArray(buffer, 0, stateLen);
                else
                    reader.GetBytes(stateLen);

                length -= stateLenByte < 0xFF
                    ? 2 + stateLen
                    : 6 + stateLen;
            }
        }
    }

    public static class ParserColumnDateExtentions
    {
        public static TimeSpan ReadSqlTime(this TdsPackageReader reader, int length, byte scale)
        {
            var tickUnits = reader.ReadByte() + ((long)reader.ReadByte() << 8) + ((long)reader.ReadByte() << 16);
            if (length > 3)
                tickUnits += (long)reader.ReadByte() << 24;
            if (length > 4)
                tickUnits += (long)reader.ReadByte() << 32;
            var ticks = tickUnits * TdsEnums.TICKS_FROM_SCALE[scale];
            return new TimeSpan(ticks);
        }
        public static readonly DateTime BaseDate1 = new DateTime(1, 1, 1);
        public static DateTime ReadSqlDate(this TdsPackageReader reader)
        {
            var v = reader.ReadByte() + (reader.ReadByte() << 8) + (reader.ReadByte() << 16);
            return BaseDate1.AddDays(v);
        }

        public static DateTime ReadSqlDateTime(this TdsPackageReader reader, int length, byte scale)
        {
            var timespan = reader.ReadSqlTime(length-3, scale);
            var date = reader.ReadSqlDate();
            return date.AddTicks(timespan.Ticks);
        }

        public static DateTimeOffset ReadSqlDateTimeOffset(this TdsPackageReader reader, int length, byte scale)
        {
            var timespan = reader.ReadSqlTime(length-5, scale);
            var date = reader.ReadSqlDate();
            var minutes = reader.ReadInt16();
            var offset = new TimeSpan(0, minutes, 0);
            return new DateTimeOffset(date.AddTicks(timespan.Ticks + offset.Ticks), offset);
        }

        public static DateTime ReadSqlDateTime(this TdsPackageReader reader, int length)
        {
            var daypart = length == 4 ? reader.ReadUInt16() : reader.ReadInt32();
            var timepart = length == 4 ? (uint)reader.ReadUInt16() * SqlDateTime.SQLTicksPerMinute : reader.ReadUInt32();
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
            var timeticks = (long)(timepart / SQLTicksPerMillisecond + 0.5) * TimeSpan.TicksPerMillisecond;

            return new DateTime(baseDateTicks + dayticks + timeticks);
        }
    }
}