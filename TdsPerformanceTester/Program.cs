using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TdsPerformanceTester
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //StartServer();
            //testNP();
            var bm = new Benchmarks(Console.WriteLine);
            bm.Run().GetAwaiter().GetResult();
        }

        static void StartServer()
        {
            var buffer = new byte[8000];
            Task.Run(() =>
            {
                var server = new NamedPipeServerStream(@"sql\query2", PipeDirection.InOut);
                server.WaitForConnection();
                Console.WriteLine("start");
                while (true)
                {
                    var endpos = server.Read(buffer, 0, 8000);
                    LogBytesString("read- ", buffer, endpos);
                }
            });
        }
        [Conditional("DEBUG")]
        private static void LogBytesString(string prefix, byte[] buffer, int length)
        {
            var sb = new StringBuilder($"{prefix}lentgh:{length,4:##0} ");
            sb.Append("data: ");
            for (var i = 0; i < length; i++)
                sb.Append($"{buffer[i],2:X2} ");
            Console.WriteLine(sb.ToString());
            sb = new StringBuilder($"{prefix}lentgh:{length,4:##0} ");
            sb.Append("data: ");
            for (var i = 0; i < length; i++)
                if (buffer[i] >= 0x20 && buffer[i] <= 0x7f)
                    sb.Append($"{(char)buffer[i]}");
            Console.WriteLine(sb.ToString());
        }
    }
}

