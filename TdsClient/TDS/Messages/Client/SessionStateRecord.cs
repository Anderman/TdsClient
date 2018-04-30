namespace Medella.TdsClient.TDS.Messages.Client
{
    internal class SessionStateRecord
    {
        internal byte[] Data;
        internal int DataLength;
        internal bool Recoverable;
        internal uint Version;
    }
}