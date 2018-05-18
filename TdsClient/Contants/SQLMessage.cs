using System;
using Medella.TdsClient.Resources;

namespace Medella.TdsClient.Contants
{
    public static class SQLMessage
    {
        // The class SQLMessage defines the error messages that are specific to the SqlDataAdapter
        // that are caused by a netlib error.  The functions will be called and then return the
        // appropriate error message from the resource Framework.txt.  The SqlDataAdapter will then
        // take the error message and then create a SqlError for the message and then place
        // that into a SqlException that is either thrown to the user or cached for throwing at
        // a later time.  This class is used so that there will be compile time checking of error
        // messages.  The resource Framework.txt will ensure proper string text based on the appropriate
        // locale.


        internal static string EncryptionNotSupportedByClient()
        {
            return Strings.SQL_EncryptionNotSupportedByClient;
        }

        internal static string EncryptionNotSupportedByServer()
        {
            return Strings.SQL_EncryptionNotSupportedByServer;
        }
    }
}