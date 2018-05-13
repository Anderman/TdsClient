using System;
using System.Data;
using System.Data.SqlClient;

namespace TdsPerformanceTester
{
    public class HandCodedOrm : IDisposable
    {
        private readonly SqlParameter _idParam;
        private readonly SqlCommand _postCommand;
        private Post _result;
        private int i = 1;
        private SqlConnection _connection;

        public HandCodedOrm()
        {
            //_connection = new LoginProcessor("context connection=true");
            //_connection = new SqlConnection(@"Server=127.0.0.1;Database=tempdb;Trusted_Connection=True;");
            _connection = new SqlConnection(@"Server=(localdb)\mssqllocaldb;Database=tempdb;Trusted_Connection=True;");
            _connection.Open();
            _postCommand = new SqlCommand
            {
                Connection = _connection,
                CommandText = @"select * from Posts where Id = @Id"
            };
            _idParam = _postCommand.Parameters.Add("@Id", SqlDbType.Int);
        }

        public void Run()
        {
            if (i > 5000)
                i = 1;
            _result = SqlCommand();
        }

        public Post SqlCommand()
        {
            _idParam.Value = i;

            using (var reader = _postCommand.ExecuteReader(CommandBehavior.SequentialAccess| CommandBehavior.SingleResult))
            {
                reader.Read();
                var post = new Post
                {
                    Id = reader.GetInt32(0),
                    Text = GetNullableString(reader, 1),
                    CreationDate = reader.GetDateTime(2),
                    LastChangeDate = reader.GetDateTime(3),
                    Counter1 = GetNullableValue<int>(reader, 4),
                    Counter2 = GetNullableValue<int>(reader, 5),
                    Counter3 = GetNullableValue<int>(reader, 6),
                    Counter4 = GetNullableValue<int>(reader, 7),
                    Counter5 = GetNullableValue<int>(reader, 8),
                    Counter6 = GetNullableValue<int>(reader, 9),
                    Counter7 = GetNullableValue<int>(reader, 10),
                    Counter8 = GetNullableValue<int>(reader, 11),
                    Counter9 = GetNullableValue<int>(reader, 12)
                };

                return post;
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
            _postCommand?.Dispose();
            _connection?.Dispose();
        }
    }
}