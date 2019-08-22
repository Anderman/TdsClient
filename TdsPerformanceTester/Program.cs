using System;
using System.Diagnostics;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;

namespace TdsPerformanceTester
{
    internal class Program
    {
        public const string ConnectionString = @"Server=(localdb)\mssqllocaldb;Database=TdsTest;Trusted_Connection=True;";
        public const string ConnectionString2 = @"Server=.;Database=tempdb;Trusted_Connection=True;";
        private Benchmarks _bm;
        private int _endPos;
        private int _pos;

        private static void Main(string[] args)
        {
            var p = new Program();

            ////StartServer();
            ////testNP();
            var bm = new Benchmarks(Console.WriteLine);
            bm.Run();
        }


        private static void StartServer()
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