# TdsClient

A Rewrite of SqlClient with improved performance and memory footprint. A cleaned library with is suitable for DI.

Goal a new lib. without connectionstring and SqlDataReader, 


The connectionstring could be a normal Json settings.

## Current status.
* Bulk Insert
* ExecuteNonQuery
* ExecuteQuery<T>
* ExecuteQueryWithParameters<T> (string interpolation send a query to Sqlserver and the arguments in the string are send as sqlparameters)
* Connection pooling
* Transactions

## Todo:
* cleanup

## not implemented
* TransactionScope
* Failover
* restore lost connections
* SqlIntances 

Execute queries and read a single result set into a poco class. Tested all sqltypes and SqlVariant types


```
      var cnn = TdsConnectionPools.GetConnectionPool(ConnectionString);
            var id = new DateTime(2018, 1, 2, 3, 4, 5);
            var r =(await cnn.ExecuteParameterQueryASync<TestId>($"select cDateTime={id}")).ToArray();
            Assert.Equal(new DateTime(2018, 1, 2, 3, 4, 5), r[0].cDateTime);
```
Transaction

```
 public void Can_commit_transaction()
        {
            var guid = Guid.NewGuid();
            var cnn = TdsConnectionPools.GetConnectionPool(ConnectionString);
            using (var transaction = cnn.BeginTransaction())
            {
                transaction.ExecuteNonQuery($"CREATE TABLE [{guid}] (id int)");
                transaction.Commit();
            }

            cnn.ExecuteNonQuery($"DROP TABLE [{guid}]");
        }
```

BulkInsert
```
        public async Task can_upload_int_Column()
        {
            var size = 5531;
            var obj = new TestBulkcopy[size];
            for (int i = 0; i < size; i++)
            {
                obj[i] = new TestBulkcopy { Id = i, Id1 = i + 1 };
            }
            var cnn = TdsConnectionPools.GetConnectionPool(ConnectionString);
            await cnn.ExecuteNonQueryAsync("if OBJECT_ID('bulkcopy') is not null DROP TABLE bulkcopy CREATE TABLE bulkcopy (Id int, Id1 bigint) ");
            await cnn.ExecuteNonQueryAsync("if OBJECT_ID('bulkcopy2') is not null DROP TABLE bulkcopy2 CREATE TABLE bulkcopy2 (Id int, Id1 bigint) ");
            var task1 = cnn.BulkInsertAsync(obj.ToList(), "bulkcopy");
            var task2 = cnn.BulkInsertAsync(obj.ToList(), "bulkcopy2");
            var task3 = cnn.BulkInsertAsync(obj.ToList(), "bulkcopy");
            Task.WaitAll(task1, task2, task3);
        }
```
