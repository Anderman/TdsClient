﻿using System;
using Medella.TdsClient.Contants;
using Medella.TdsClient.Exceptions;
using Medella.TdsClient.TDS.Messages.Client;
using Medella.TdsClient.TDS.Package;
using Medella.TdsClient.TDS.Package.Reader;

namespace Medella.TdsClient.TDS.Messages.Server
{
    public static class ParserFeatureExtAck
    {
        private const int MaxNumberOfSessionStates = 256;

        // read feature ID
        public static void FeatureExtAck(this TdsPackageReader reader)
        {
            while (true)
            {
                var featureId = reader.ReadByte();

                if (featureId == TdsEnums.FEATUREEXT_TERMINATOR)
                    break;
                var dataLen = reader.ReadUInt32();
                var data = reader.GetBytes(checked((int)dataLen));
                if (dataLen > 0)
                    OnFeatureExtAck(featureId, data);
            }
        }

        public static void OnFeatureExtAck(byte featureId, byte[] data)
        {
            var initialState = new byte[MaxNumberOfSessionStates][];
            var delta = new SessionStateRecord[MaxNumberOfSessionStates];
            object recoverySessionData = null;
            var isGlobalTransaction = false;
            var isGlobalTransactionsEnabledForServer = false;
            var DeltaDirty = false;
            switch (featureId)
            {
                case TdsEnums.FEATUREEXT_SRECOVERY:
                    {
                        var i = 0;
                        while (i < data.Length)
                        {
                            var stateId = data[i++];
                            var bLen = data[i++];
                            var len = bLen == 0xFF ? BitConverter.ToInt32(data, i) : bLen;
                            i += bLen == 0xFF ? 4 : 0;

                            var stateData = new byte[len];
                            Buffer.BlockCopy(data, i, stateData, 0, len);
                            i += len;
                            if (recoverySessionData == null)
                            {
                                initialState[stateId] = stateData;
                            }
                            else
                            {
                                delta[stateId] = new SessionStateRecord { Data = stateData, DataLength = len, Recoverable = true, Version = 0 };
                                DeltaDirty = true;
                            }
                        }

                        break;
                    }

                case TdsEnums.FEATUREEXT_GLOBALTRANSACTIONS:
                    {
                        if (data.Length < 1) throw SQL.ParsingError();

                        isGlobalTransaction = true;
                        if (1 == data[0]) isGlobalTransactionsEnabledForServer = true;
                        break;
                    }

                default:
                    throw SQL.ParsingError();
            }
        }
    }
}