namespace Medella.TdsClient.System2.Net.Security
{
    internal static class GlobalSSPI
    {
        internal static readonly SSPIInterface SSPIAuth = new SSPIAuthType();
        internal static readonly SSPIInterface SSPISecureChannel = new SspiSecureChannelType();
    }
}