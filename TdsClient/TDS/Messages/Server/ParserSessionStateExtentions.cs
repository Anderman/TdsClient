using System.Diagnostics;
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
}