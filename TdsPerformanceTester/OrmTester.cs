using System;
using Medella.TdsClient.TDS;
using Medella.TdsClient.TDS.Controller;
using Medella.TdsClient.TDS.Processes;

namespace TdsPerformanceTester
{
    public class OrmTester
    {
        private const string ConnectionString = @"Server=(localdb)\mssqllocaldb;Database=tempdb;Trusted_Connection=True;";

        public static void EnsureDBSetup()
        {
            var tds = Tds.GetConnection(ConnectionString);
            tds.ExecuteNonQuery(@"
If (Object_Id('Posts') Is Null)
Begin
	SendExcuteBatch Table Posts
	(
		Id int identity primary key, 
		[Text] varchar(max) not null, 
		CreationDate datetime not null, 
		LastChangeDate datetime not null,
		Counter1 int,
		Counter2 int,
		Counter3 int,
		Counter4 int,
		Counter5 int,
		Counter6 int,
		Counter7 int,
		Counter8 int,
		Counter9 int
	);
	   
	Set NoCount On;
	Declare @i int = 0;

	While @i <= 5001
	Begin
		Insert Posts ([Text],CreationDate, LastChangeDate) values (replicate('x', 2000), GETDATE(), GETDATE());
		Set @i = @i + 1;
	End
End
");
        }
    }
    public class Post
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastChangeDate { get; set; }
        public int? Counter1 { get; set; }
        public int? Counter2 { get; set; }
        public int? Counter3 { get; set; }
        public int? Counter4 { get; set; }
        public int? Counter5 { get; set; }
        public int? Counter6 { get; set; }
        public int? Counter7 { get; set; }
        public int? Counter8 { get; set; }
        public int? Counter9 { get; set; }
    }
}