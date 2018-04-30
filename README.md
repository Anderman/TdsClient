# TdsClient

A Rewrite of SqlClient with improved performance and memory footprint. A cleaned library with is suitable for DI.

Goal a new lib. without connectionstring and SqlDataReader, 

Todo:
* BulkInsert
* Connection pooling
* Async
* Create a TdsStream from connection settings and inject this stream in the TdsClient. This will improve the testing and made it possible to 
inject a TestTdsStream. Which will also lead to easy performance testing

The connectionstring could be a normal Json settings.
