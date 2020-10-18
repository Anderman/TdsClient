using System;
using System.Globalization;
using Medella.TdsClient.Constants;
using Medella.TdsClient.Resources;

namespace Medella.TdsClient.Exceptions
{
    internal static class ADP
    {
        internal const int DefaultConnectionTimeout = DbConnectionStringDefaults.ConnectTimeout;

        internal static readonly string[] AzureSqlServerEndpoints =
        {
            Strings.AZURESQL_GenericEndpoint,
            Strings.AZURESQL_GermanEndpoint,
            Strings.AZURESQL_UsGovEndpoint,
            Strings.AZURESQL_ChinaEndpoint
        };

        // The class ADP defines the exceptions that are specific to the Adapters.
        // The class contains functions that take the proper informational variables and then construct
        // the appropriate exception with an error string obtained from the resource framework.
        // The exception is then returned to the caller, so that the caller may then throw from its
        // location so that the catcher of the exception will have the appropriate call stack.
        // This class is used so that there will be compile time checking of error messages.


        internal static InvalidOperationException InvalidOperation(string error)
        {
            var e = new InvalidOperationException(error);
            TraceExceptionAsReturnValue(e);
            return e;
        }


        internal static Exception InternalError(InternalErrorCode internalError) => InvalidOperation(SR.Format(Strings.ADP_InternalProviderError, (int)internalError));


        internal static Exception InvalidConnectionOptionValue(string key) => InvalidConnectionOptionValue(key, null);

        internal static Exception InvalidConnectionOptionValue(string key, Exception inner) => Argument(SR.Format(Strings.ADP_InvalidConnectionOptionValue, key), inner);


        internal static ArgumentException Argument(string error, Exception inner)
        {
            var e = new ArgumentException(error, inner);
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static ArgumentException ConnectionStringSyntax(int index) => Argument(SR.Format(Strings.ADP_ConnectionStringSyntax, index));

        internal static InvalidOperationException DataAdapter(string error) => InvalidOperation(error);

        internal static ArgumentOutOfRangeException InvalidEnumerationValue(Type type, int value) => ArgumentOutOfRange(SR.Format(Strings.ADP_InvalidEnumerationValue, type.Name, value.ToString(CultureInfo.InvariantCulture)), type.Name);


        internal static NotSupportedException NotSupported(string error)
        {
            var e = new NotSupportedException(error);
            TraceExceptionAsReturnValue(e);
            return e;
        }


        internal static OverflowException Overflow(string error, Exception inner)
        {
            var e = new OverflowException(error, inner);
            return e;
        }


        internal static ArgumentException InvalidMinMaxPoolSizeValues() => Argument(Strings.ADP_InvalidMinMaxPoolSizeValues);


        //
        // : DbConnectionOptions, DataAccess, SqlClient
        //
        internal static Exception InvalidConnectionOptionValueLength(string key, int limit) => Argument(SR.GetString(Strings.ADP_InvalidConnectionOptionValueLength, key, limit));

        internal static Exception MissingConnectionOptionValue(string key, string requiredAdditionalKey) => Argument(SR.GetString(Strings.ADP_MissingConnectionOptionValue, key, requiredAdditionalKey));


        internal static Exception InvalidConnectRetryIntervalValue() => Argument(Strings.SQLCR_InvalidConnectRetryIntervalValue);

        //
        // : DbDataReader
        //

        internal static string MachineName() => Environment.MachineName;


        // This method assumes dataSource parameter is in TCP connection string format.
        internal static bool IsAzureSqlServerEndpoint(string dataSource)
        {
            // remove server port
            var i = dataSource.LastIndexOf(',');
            if (i >= 0) dataSource = dataSource.Substring(0, i);

            // check for the instance name
            i = dataSource.LastIndexOf('\\');
            if (i >= 0) dataSource = dataSource.Substring(0, i);

            // trim redundant whitespace
            dataSource = dataSource.Trim();

            // check if servername end with any azure endpoints
            for (i = 0; i < AzureSqlServerEndpoints.Length; i++)
                if (dataSource.EndsWith(AzureSqlServerEndpoints[i], StringComparison.OrdinalIgnoreCase))
                    return true;

            return false;
        }


        private static void TraceException(string trace, Exception e)
        {
            throw new NotImplementedException();
        }

        internal static void TraceExceptionAsReturnValue(Exception e)
        {
            TraceException("<comm.ADP.TraceException|ERR|THROW> '{0}'", e);
        }

        internal static ArgumentException Argument(string error)
        {
            var e = new ArgumentException(error);
            TraceExceptionAsReturnValue(e);
            return e;
        }


        internal static ArgumentOutOfRangeException ArgumentOutOfRange(string message, string parameterName)
        {
            var e = new ArgumentOutOfRangeException(parameterName, message);
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static ArgumentException KeywordNotSupported(string keyword) => Argument(SR.Format(Strings.ADP_KeywordNotSupported, keyword));

        internal static Exception InvalidConnectRetryCountValue() => Argument(Strings.SQLCR_InvalidConnectRetryCountValue);

        internal static void CheckArgumentNull(object value, string parameterName)
        {
            if (null == value) throw ArgumentNull(parameterName);
        }

        internal static ArgumentNullException ArgumentNull(string parameter)
        {
            var e = new ArgumentNullException(parameter);
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static ArgumentException ConvertFailed(Type fromType, Type toType, Exception? innerException) => Argument(SR.Format(Strings.SqlConvert_ConvertFailed, fromType.FullName, toType.FullName), innerException);


        internal enum InternalErrorCode
        {
            UnpooledObjectHasOwner = 0,
            UnpooledObjectHasWrongOwner = 1,
            PushingObjectSecondTime = 2,
            PooledObjectHasOwner = 3,
            PooledObjectInPoolMoreThanOnce = 4,
            CreateObjectReturnedNull = 5,
            NewObjectCannotBePooled = 6,
            NonPooledObjectUsedMoreThanOnce = 7,
            AttemptingToPoolOnRestrictedToken = 8,

            //          ConnectionOptionsInUse                                  =  9,
            ConvertSidToStringSidWReturnedNull = 10,

            //          UnexpectedTransactedObject                              = 11,
            AttemptingToConstructReferenceCollectionOnStaticObject = 12,
            AttemptingToEnlistTwice = 13,
            CreateReferenceCollectionReturnedNull = 14,
            PooledObjectWithoutPool = 15,
            UnexpectedWaitAnyResult = 16,
            SynchronousConnectReturnedPending = 17,
            CompletedConnectReturnedPending = 18,

            NameValuePairNext = 20,
            InvalidParserState1 = 21,
            InvalidParserState2 = 22,
            InvalidParserState3 = 23,

            InvalidBuffer = 30,

            UnimplementedSMIMethod = 40,
            InvalidSmiCall = 41,

            SqlDependencyObtainProcessDispatcherFailureObjectHandle = 50,
            SqlDependencyProcessDispatcherFailureCreateInstance = 51,
            SqlDependencyProcessDispatcherFailureAppDomain = 52,
            SqlDependencyCommandHashIsNotAssociatedWithNotification = 53,

            UnknownTransactionFailure = 60
        }
    }
}