using System;
using Medella.TdsClient.Contants;
using Medella.TdsClient.Resources;
using Medella.TdsClient.TDS.Controller;

namespace Medella.TdsClient.Exceptions
{
    internal static class SQL
    {
        // Default values for SqlDependency and SqlNotificationRequest
        internal const int SqlDependencyTimeoutDefault = 0;
        internal const int SqlDependencyServerTimeout = 5 * 24 * 3600; // 5 days - used to compute default TTL of the dependency
        internal const string SqlNotificationServiceDefault = "SqlQueryNotificationService";
        internal const string SqlNotificationStoredProcedureDefault = "SqlQueryNotificationStoredProcedure";

        internal static readonly byte[] AttentionHeader =
        {
            TdsEnums.MT_ATTN, // Message Type
            TdsEnums.ST_EOM, // Status
            TdsEnums.HEADER_LEN >> 8, // length - upper byte
            TdsEnums.HEADER_LEN & 0xff, // length - lower byte
            0, // spid
            0, // spid
            0, // packet (out of band)
            0 // window
        };
        // The class SQL defines the exceptions that are specific to the SQL Adapter.
        // The class contains functions that take the proper informational variables and then construct
        // the appropriate exception with an error string obtained from the resource Framework.txt.
        // The exception is then returned to the caller, so that the caller may then throw from its
        // location so that the catcher of the exception will have the appropriate call stack.
        // This class is used so that there will be compile time checking of error
        // messages.  The resource Framework.txt will ensure proper string text based on the appropriate
        // locale.

        //
        // SQL specific exceptions
        //

        //
        // SQL.Connection
        //

        internal static Exception InvalidPacketSizeValue()
        {
            return ADP.Argument(SR.GetString(Strings.SQL_InvalidPacketSizeValue));
        }

        internal static Exception InvalidSSPIPacketSize()
        {
            return ADP.Argument(SR.GetString(Strings.SQL_InvalidSSPIPacketSize));
        }

        internal static Exception UserInstanceFailoverNotCompatible()
        {
            return ADP.Argument(SR.GetString(Strings.SQL_UserInstanceFailoverNotCompatible));
        }

        internal static Exception InvalidSQLServerVersionUnknown()
        {
            return ADP.DataAdapter(SR.GetString(Strings.SQL_InvalidSQLServerVersionUnknown));
        }

        internal static Exception InvalidTDSVersion()
        {
            return ADP.InvalidOperation(SR.GetString(Strings.SQL_InvalidTDSVersion));
        }

        internal static Exception ParsingError()
        {
            return ADP.InvalidOperation(SR.GetString(Strings.SQL_ParsingError));
        }

        internal static Exception SNIPacketAllocationFailure()
        {
            return ADP.InvalidOperation(SR.GetString(Strings.SQL_SNIPacketAllocationFailure));
        }

        //
        // MultiSubnetFailover
        //

        /// <summary>
        ///     used to block two scenarios if MultiSubnetFailover is true:
        ///     * server-provided failover partner - raising SqlException in this case
        ///     * connection string with failover partner and MultiSubnetFailover=true - raising argument one in this case with the
        ///     same message
        /// </summary>
        internal static Exception MultiSubnetFailoverWithFailoverPartner(bool serverProvidedFailoverPartner, SqlInternalConnectionTds internalConnection)
        {
            var msg = SR.GetString(Strings.SQLMSF_FailoverPartnerNotSupported);
            if (serverProvidedFailoverPartner)
            {
                // Replacing InvalidOperation with SQL exception
                var errors = new SqlErrorCollection();
                errors.Add(new SqlInfoAndError {Class = TdsEnums.FATAL_ERROR_CLASS, Message = msg});
                var exc = SqlException.CreateException(errors, null);
                exc._doNotReconnect = true; // disable open retry logic on this error
                return exc;
            }

            return ADP.Argument(msg);
        }


        //
        // Read-only routing
        //

        internal static Exception ROR_FailoverNotSupportedConnString()
        {
            return ADP.Argument(SR.GetString(Strings.SQLROR_FailoverNotSupported));
        }


        //
        // Connection resiliency
        //

        // ProjectK\CoreCLR specific errors
        internal static Exception UnsupportedKeyword(string keyword)
        {
            return ADP.NotSupported(SR.GetString(Strings.SQL_UnsupportedKeyword, keyword));
        }

        internal static Exception NetworkLibraryKeywordNotSupported()
        {
            return ADP.NotSupported(SR.GetString(Strings.SQL_NetworkLibraryNotSupported));
        }
    }

    internal class SqlInternalConnectionTds
    {
        public Guid OriginalClientConnectionId { get; set; }
        public string RoutingDestination { get; set; }
    }
}