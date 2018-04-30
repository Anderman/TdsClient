namespace Medella.TdsClient.TDS.Messages.Client
{
    internal enum PreLoginOptions
    {
        VERSION,
        ENCRYPT,
        INSTANCE,
        THREADID,
        MARS,
        TRACEID,
        NUMOPT,
        LASTOPT = 255
    }
}