using System;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;

namespace Medella.TdsClient.SNI.Sspi
{
    public class SspiHelper
    {
        private readonly ConstructorInfo _sspiClientContextStatusConstructor;
        private readonly MethodInfo _genSspiClientContextMethod;
        private readonly object _sniInstance;

        private object _sspiClientContextStatus;
        private readonly byte[] _serverSpn;

        public SspiHelper(string serverSpn)
        {
            _serverSpn = Encoding.UTF8.GetBytes(serverSpn);

            var sspiClientContext = typeof(SqlCommand).Assembly.GetType("System.Data.SqlClient.SNI.SspiClientContextStatus");
            _sspiClientContextStatusConstructor = sspiClientContext.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);

            var sniProxy = typeof(SqlCommand).Assembly.GetType("System.Data.SqlClient.SNI.SNIProxy");
            var sni = sniProxy.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);
            _sniInstance = sni.Invoke(null);
            _genSspiClientContextMethod = sniProxy.GetMethod("GenSspiClientContext");
        }

        public byte[] ClientToken { get; set; }

        public byte[] CreateClientToken(byte[] serverToken)
        {
            if (serverToken == null)
                _sspiClientContextStatus = GetNewSspiClientContextStatus();
            var args = new[] {_sspiClientContextStatus, serverToken, (byte[]) null, _serverSpn};
            _genSspiClientContextMethod.Invoke(_sniInstance, args);
            ClientToken = (byte[]) args[2];
            return ClientToken;
        }

        public object GetNewSspiClientContextStatus()
        {
            var sspiClass = _sspiClientContextStatusConstructor.Invoke(null);
            return sspiClass;
        }
    }
}