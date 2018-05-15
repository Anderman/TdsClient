using System;
using System.Collections.Generic;
using Medella.TdsClient.TDS.Row.Reader.StringHelpers;

namespace Medella.TdsClient.TDS.Messages.Client
{
    public class SessionData
    {
        internal const int MaxNumberOfSessionStates = 256;
        internal SqlCollations Collation;
        internal string Database;
        internal SessionStateRecord[] Delta = new SessionStateRecord[MaxNumberOfSessionStates];
        internal bool DeltaDirty;
        internal bool Encrypted;
        internal SqlCollations InitialCollation;
        internal string InitialDatabase;
        internal string InitialLanguage;
        internal byte[][] InitialState = new byte[MaxNumberOfSessionStates][];
        internal string Language;
        internal Dictionary<string, Tuple<string, string>> ResolvedAliases;
        internal uint TdsVersion;
        internal byte UnrecoverableStatesCount;

        public SessionData(SessionData recoveryData)
        {
            InitialDatabase = recoveryData.InitialDatabase;
            InitialCollation = recoveryData.InitialCollation;
            InitialLanguage = recoveryData.InitialLanguage;
            ResolvedAliases = recoveryData.ResolvedAliases;

            for (var i = 0; i < MaxNumberOfSessionStates; i++)
                if (recoveryData.InitialState[i] != null)
                    InitialState[i] = (byte[]) recoveryData.InitialState[i].Clone();
        }

        public SessionData()
        {
            ResolvedAliases = new Dictionary<string, Tuple<string, string>>(2);
        }

        public void Reset()
        {
            Database = null;
            Collation = null;
            Language = null;
            if (DeltaDirty)
            {
                Delta = new SessionStateRecord[MaxNumberOfSessionStates];
                DeltaDirty = false;
            }

            UnrecoverableStatesCount = 0;
        }
    }
}