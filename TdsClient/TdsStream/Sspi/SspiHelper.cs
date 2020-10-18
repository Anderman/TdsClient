using System;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;

namespace Medella.TdsClient.TdsStream.Sspi
{
    public class SspiHelper
    {
        private readonly MethodInfo? _genSspiClientContextMethod;
        private readonly byte[] _serverSpn;
        private readonly object _sniInstance;
        private readonly ConstructorInfo? _sspiClientContextStatusConstructor;

        private object? _sspiClientContextStatus;

        // setup call to internal api GenSspiClientContext(SspiClientContextStatus ctx, byte[] serverToken, out byte[] clientToken, byte[] serverSpn)
        public SspiHelper(string serverSpn)
        {
            _serverSpn = Encoding.UTF8.GetBytes(serverSpn);

            var sspiClientContext = typeof(SqlCommand).Assembly.GetType("System.Data.SqlClient.SNI.SspiClientContextStatus");
            _sspiClientContextStatusConstructor = sspiClientContext!.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);

            var sniProxy = typeof(SqlCommand).Assembly.GetType("System.Data.SqlClient.SNI.SNIProxy");
            var sni = sniProxy!.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);
            _sniInstance = sni!.Invoke(null);
            _genSspiClientContextMethod = sniProxy!.GetMethod("GenSspiClientContext");
        }

        public byte[] CreateClientToken(byte[] serverToken)
        {
            if (serverToken == null)
                _sspiClientContextStatus = _sspiClientContextStatusConstructor!.Invoke(null);

            var args = new[] { _sspiClientContextStatus, serverToken, null, _serverSpn };
            _genSspiClientContextMethod!.Invoke(_sniInstance, args);
            return (byte[])args[2]!; ;
        }
    }
}