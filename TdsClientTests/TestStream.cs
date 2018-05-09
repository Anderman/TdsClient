using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;
using Medella.TdsClient.SNI;
using Medella.TdsClient.SNI.SniNp;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Xunit;

namespace TdsClientTests
{
    public class TestStream : ISniHandle
    {
        public string ServerSpn { get; }
        public string InstanceName { get; }
        public Queue<byte[]> Queue = new Queue<byte[]>();
        public void FlushBuffer(byte[] writeBuffer, int count)
        {
            var package = new byte[count];
            Buffer.BlockCopy(writeBuffer, 0, package, 0, count);
            Queue.Enqueue(package);
        }

        public int Receive(byte[] readBuffer, int offset, int count)
        {
            var package = Queue.Dequeue();
            Buffer.BlockCopy(package, 0, readBuffer, 0, package.Length);
            return package.Length;
        }

        public byte[] GetClientToken(byte[] serverToken)
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void testAccess()
        //SspiClientContextStatus
        //SNIProxy.GenSspiClientContext
        {
            var sspiClientContext = typeof(System.Data.SqlClient.SqlCommand).Assembly.GetType("System.Data.SqlClient.SNI.SspiClientContextStatus");
            var c = sspiClientContext.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);
            var sspiClass = c.Invoke(null);

            var sniProxy = typeof(System.Data.SqlClient.SqlCommand).Assembly.GetType("System.Data.SqlClient.SNI.SNIProxy");
            var sni = sniProxy.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);
            var sniInstance = sni.Invoke(null);
            var methodInfo = sniProxy.GetMethod("GenSspiClientContext");

            var server = new byte[200];
            var client = new byte[200];
            var servername = $"MSSQLSvc/{Dns.GetHostEntry("localhost").HostName}";
            var serverspn = Encoding.Unicode.GetBytes(servername);
            var args = new[] {sspiClass, server, client, serverspn};
            methodInfo.Invoke(sniInstance, args);
            var clienttoken = (byte[]) args[2];
        }
    }
}