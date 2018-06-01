using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace TdsPerformanceTester
{
    public class HandCodedOrmPosts : IDisposable
    {
        private List<Post> _result;
        private const int conc = 4;
        private int _i = 1;
        private readonly SqlConnection[] _connection;
        private readonly SqlCommand[] _postCommand;

        public HandCodedOrmPosts()
        {
            _connection= new SqlConnection[100];
            _postCommand= new SqlCommand[100];
            var task = Parallel.For(0, conc, OpenConnection);
        }

        private void OpenConnection(int i)
        {
            _connection[i] = new SqlConnection(Program.ConnectionString);
            _connection[i].Open();
            _postCommand[i] = new SqlCommand
            {
                Connection = _connection[i],
                CommandText = @"select ID,CreationDate,LastChangeDate, counter1,counter2,counter3,counter4,counter5,counter6,counter7,counter8,counter9 from Posts"
            };
        }

        public void Run()
        {
            if (_i > 5000)
                _i = 1;
            var task = Parallel.For(0, conc,
                ctr => SqlCommand(ctr));
        }

        public List<Post> SqlCommand(int i)
        {
            var posts = new List<Post>();
            using (var reader = _postCommand[i].ExecuteReader(CommandBehavior.SequentialAccess))
            {
                while (reader.Read())
                {
                    posts.Add(new Post
                    {
                        Id = reader.GetInt32(0),
                        //Text = GetNullableString(reader, 1),
                        CreationDate = reader.GetDateTime(1),
                        LastChangeDate = reader.GetDateTime(2),
                        Counter1 = GetNullableValue<int>(reader, 3),
                        Counter2 = GetNullableValue<int>(reader, 4),
                        Counter3 = GetNullableValue<int>(reader, 5),
                        Counter4 = GetNullableValue<int>(reader, 6),
                        Counter5 = GetNullableValue<int>(reader, 7),
                        Counter6 = GetNullableValue<int>(reader, 8),
                        Counter7 = GetNullableValue<int>(reader, 9),
                        Counter8 = GetNullableValue<int>(reader, 10),
                        Counter9 = GetNullableValue<int>(reader, 11)
                    });
                }

                return posts;
            }
        }
        public static string GetNullableString(IDataReader reader, int index)
        {
            var tmp = reader.GetValue(index);
            return tmp != DBNull.Value ? (string)tmp : null;
        }

        public static T? GetNullableValue<T>(IDataReader reader, int index) where T : struct
        {
            var tmp = reader.GetValue(index);
            return tmp != DBNull.Value ? (T?)(T)tmp : null;
        }
        public void Dispose()
        {
            for (int i = 0; i < conc; i++)
            {
                _postCommand[i].Dispose();
                _connection[i]?.Dispose();
            }
        }
    }
}