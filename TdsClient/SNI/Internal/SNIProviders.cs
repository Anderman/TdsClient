namespace Medella.TdsClient.SNI.Internal
{
    /// <summary>
    ///     SNI error
    /// </summary>
    public enum SniProviders
    {
        HTTP_PROV, // HTTP Provider
        NP_PROV, // Named Pipes Provider
        SESSION_PROV, // Session Provider
        SIGN_PROV, // Sign Provider
        SM_PROV, // Shared Memory Provider
        SMUX_PROV, // SMUX Provider
        SSL_PROV, // SSL Provider
        TCP_PROV, // TCP Provider
        MAX_PROVS, // Number of providers
        INVALID_PROV // SQL Network Interfaces
    }
}