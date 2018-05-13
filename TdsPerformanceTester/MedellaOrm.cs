﻿using System.Linq;
using System.Threading.Tasks;
using Medella.TdsClient.TDS;
using Medella.TdsClient.TDS.Processes;

namespace TdsPerformanceTester
{
    public class MedellaOrm 
    {
        private readonly TdsConnectionPool _tds;
        private int i = 1;
        private const string ConnectionString = @"Server=(localdb)\mssqllocaldb;Database=tempdb;Trusted_Connection=True;";

        public MedellaOrm()
        {
            _tds = TdsClient.GetConnection(ConnectionString);
            OrmTester.EnsureDbSetup(ConnectionString);
        }

        public void Run()
        {
            if (i++ > 5000)
                i = 1;

            //using (var tds = TdsClient.GetConnection(ConnectionString))
            {
                var x = _tds.ExecuteParameterQuery<Post>($@"select * from Posts where Id = {i}");
            }
        }
    }
}