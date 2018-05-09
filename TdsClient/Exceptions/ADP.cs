using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Medella.TdsClient.Contants;
using Medella.TdsClient.System2;
using ST = System.Transactions;

namespace Medella.TdsClient.Exceptions
{
    internal static partial class ADP
    {
        // global constant strings
        internal const string Parameter = "Parameter";
        internal const string ParameterName = "ParameterName";
        internal const string ParameterSetPosition = "set_Position";

        internal const int DefaultCommandTimeout = 30;
        internal const float FailoverTimeoutStep = 0.08F; // fraction of timeout to use for fast failover connections

        internal const int CharSize = sizeof(char);

        // security issue, don't rely upon public static readonly values
        internal static readonly string StrEmpty = ""; // String.Empty

        private static Version _sSystemDataVersion;


        internal static readonly string[] AzureSqlServerEndpoints =
        {
            SR.GetString(Strings.AZURESQL_GenericEndpoint),
            SR.GetString(Strings.AZURESQL_GermanEndpoint),
            SR.GetString(Strings.AZURESQL_UsGovEndpoint),
            SR.GetString(Strings.AZURESQL_ChinaEndpoint)
        };

        // The class ADP defines the exceptions that are specific to the Adapters.
        // The class contains functions that take the proper informational variables and then construct
        // the appropriate exception with an error string obtained from the resource framework.
        // The exception is then returned to the caller, so that the caller may then throw from its
        // location so that the catcher of the exception will have the appropriate call stack.
        // This class is used so that there will be compile time checking of error messages.
        internal static Exception ExceptionWithStackTrace(Exception e)
        {
            try
            {
                throw e;
            }
            catch (Exception caught)
            {
                return caught;
            }
        }

        //
        // COM+ exceptions
        //
        internal static IndexOutOfRangeException IndexOutOfRange(int value)
        {
            var e = new IndexOutOfRangeException(value.ToString(CultureInfo.InvariantCulture));
            return e;
        }

        internal static IndexOutOfRangeException IndexOutOfRange()
        {
            var e = new IndexOutOfRangeException();
            return e;
        }

        internal static TimeoutException TimeoutException(string error)
        {
            var e = new TimeoutException(error);
            return e;
        }

        internal static InvalidOperationException InvalidOperation(string error, Exception inner)
        {
            var e = new InvalidOperationException(error, inner);
            return e;
        }

        internal static OverflowException Overflow(string error)
        {
            return Overflow(error, null);
        }

        internal static OverflowException Overflow(string error, Exception inner)
        {
            var e = new OverflowException(error, inner);
            return e;
        }

        internal static TypeLoadException TypeLoad(string error)
        {
            var e = new TypeLoadException(error);
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static PlatformNotSupportedException DbTypeNotSupported(string dbType)
        {
            var e = new PlatformNotSupportedException(SR.GetString(Strings.SQL_DbTypeNotSupportedOnThisPlatform, dbType));
            return e;
        }

        internal static InvalidCastException InvalidCast()
        {
            var e = new InvalidCastException();
            return e;
        }

        internal static IOException IO(string error)
        {
            var e = new IOException(error);
            return e;
        }

        internal static IOException IO(string error, Exception inner)
        {
            var e = new IOException(error, inner);
            return e;
        }

        internal static ObjectDisposedException ObjectDisposed(object instance)
        {
            var e = new ObjectDisposedException(instance.GetType().Name);
            return e;
        }

        internal static Exception DataTableDoesNotExist(string collectionName)
        {
            return Argument(SR.GetString(Strings.MDF_DataTableDoesNotExist, collectionName));
        }

        internal static InvalidOperationException MethodCalledTwice(string method)
        {
            var e = new InvalidOperationException(SR.GetString(Strings.ADP_CalledTwice, method));
            return e;
        }


        // IDbCommand.CommandType
        internal static ArgumentOutOfRangeException InvalidCommandType(CommandType value)
        {
            return InvalidEnumerationValue(typeof(CommandType), (int) value);
        }

        // IDbConnection.BeginTransaction, OleDbTransaction.Begin
        internal static ArgumentOutOfRangeException InvalidIsolationLevel(IsolationLevel value)
        {
            return InvalidEnumerationValue(typeof(IsolationLevel), (int) value);
        }


        // IDataParameter.Direction
        internal static ArgumentOutOfRangeException InvalidParameterDirection(ParameterDirection value)
        {
            return InvalidEnumerationValue(typeof(ParameterDirection), (int) value);
        }

        internal static Exception TooManyRestrictions(string collectionName)
        {
            return Argument(SR.GetString(Strings.MDF_TooManyRestrictions, collectionName));
        }


        // IDbCommand.UpdateRowSource
        internal static ArgumentOutOfRangeException InvalidUpdateRowSource(UpdateRowSource value)
        {
            return InvalidEnumerationValue(typeof(UpdateRowSource), (int) value);
        }

        //
        // DbConnectionOptions, DataAccess
        //
        internal static ArgumentException InvalidMinMaxPoolSizeValues()
        {
            return Argument(SR.GetString(Strings.ADP_InvalidMinMaxPoolSizeValues));
        }


        //
        // DbConnection
        //
        internal static InvalidOperationException NoConnectionString()
        {
            return InvalidOperation(SR.GetString(Strings.ADP_NoConnectionString));
        }

        internal static Exception MethodNotImplemented([CallerMemberName] string methodName = "")
        {
            return new NotImplementedException();
        }

        internal static Exception QueryFailed(string collectionName, Exception e)
        {
            return InvalidOperation(SR.GetString(Strings.MDF_QueryFailed, collectionName), e);
        }


        //
        // : DbConnectionOptions, DataAccess, SqlClient
        //
        internal static Exception InvalidConnectionOptionValueLength(string key, int limit)
        {
            return Argument(SR.GetString(Strings.ADP_InvalidConnectionOptionValueLength, key, limit));
        }

        internal static Exception MissingConnectionOptionValue(string key, string requiredAdditionalKey)
        {
            return Argument(SR.GetString(Strings.ADP_MissingConnectionOptionValue, key, requiredAdditionalKey));
        }


        //
        // DbConnectionPool and related
        //
        internal static Exception PooledOpenTimeout()
        {
            return InvalidOperation(SR.GetString(Strings.ADP_PooledOpenTimeout));
        }

        internal static Exception NonPooledOpenTimeout()
        {
            return TimeoutException(SR.GetString(Strings.ADP_NonPooledOpenTimeout));
        }

        //
        // DbProviderException
        //
        internal static InvalidOperationException TransactionConnectionMismatch()
        {
            return Provider(SR.GetString(Strings.ADP_TransactionConnectionMismatch));
        }

        internal static InvalidOperationException TransactionRequired(string method)
        {
            return Provider(SR.GetString(Strings.ADP_TransactionRequired, method));
        }


        internal static Exception CommandTextRequired(string method)
        {
            return InvalidOperation(SR.GetString(Strings.ADP_CommandTextRequired, method));
        }

        internal static Exception NoColumns()
        {
            return Argument(SR.GetString(Strings.MDF_NoColumns));
        }

        internal static InvalidOperationException ConnectionRequired(string method)
        {
            return InvalidOperation(SR.GetString(Strings.ADP_ConnectionRequired, method));
        }

        internal static InvalidOperationException OpenConnectionRequired(string method, ConnectionState state)
        {
            return InvalidOperation(SR.GetString(Strings.ADP_OpenConnectionRequired, method, ConnectionStateMsg(state)));
        }

        internal static Exception OpenReaderExists()
        {
            return OpenReaderExists(null);
        }

        internal static Exception OpenReaderExists(Exception e)
        {
            return InvalidOperation(SR.GetString(Strings.ADP_OpenReaderExists), e);
        }


        //
        // DbDataReader
        //
        internal static Exception NonSeqByteAccess(long badIndex, long currIndex, string method)
        {
            return InvalidOperation(SR.GetString(Strings.ADP_NonSeqByteAccess, badIndex.ToString(CultureInfo.InvariantCulture), currIndex.ToString(CultureInfo.InvariantCulture), method));
        }

        internal static Exception InvalidXml()
        {
            return Argument(SR.GetString(Strings.MDF_InvalidXml));
        }

        internal static Exception NegativeParameter(string parameterName)
        {
            return InvalidOperation(SR.GetString(Strings.ADP_NegativeParameter, parameterName));
        }

        internal static Exception InvalidXmlMissingColumn(string collectionName, string columnName)
        {
            return Argument(SR.GetString(Strings.MDF_InvalidXmlMissingColumn, collectionName, columnName));
        }

        //
        // SqlMetaData, SqlTypes, SqlClient
        //
        internal static Exception InvalidMetaDataValue()
        {
            return Argument(SR.GetString(Strings.ADP_InvalidMetaDataValue));
        }

        internal static InvalidOperationException NonSequentialColumnAccess(int badCol, int currCol)
        {
            return InvalidOperation(SR.GetString(Strings.ADP_NonSequentialColumnAccess, badCol.ToString(CultureInfo.InvariantCulture), currCol.ToString(CultureInfo.InvariantCulture)));
        }

        internal static Exception InvalidXmlInvalidValue(string collectionName, string columnName)
        {
            return Argument(SR.GetString(Strings.MDF_InvalidXmlInvalidValue, collectionName, columnName));
        }

        internal static Exception CollectionNameIsNotUnique(string collectionName)
        {
            return Argument(SR.GetString(Strings.MDF_CollectionNameISNotUnique, collectionName));
        }


        //
        // : IDbCommand
        //
        internal static Exception InvalidCommandTimeout(int value, [CallerMemberName] string property = "")
        {
            return Argument(SR.GetString(Strings.ADP_InvalidCommandTimeout, value.ToString(CultureInfo.InvariantCulture)), property);
        }

        internal static Exception UninitializedParameterSize(int index, Type dataType)
        {
            return InvalidOperation(SR.GetString(Strings.ADP_UninitializedParameterSize, index.ToString(CultureInfo.InvariantCulture), dataType.Name));
        }

        internal static Exception UnableToBuildCollection(string collectionName)
        {
            return Argument(SR.GetString(Strings.MDF_UnableToBuildCollection, collectionName));
        }

        internal static Exception PrepareParameterType(DbCommand cmd)
        {
            return InvalidOperation(SR.GetString(Strings.ADP_PrepareParameterType, cmd.GetType().Name));
        }

        internal static Exception UndefinedCollection(string collectionName)
        {
            return Argument(SR.GetString(Strings.MDF_UndefinedCollection, collectionName));
        }

        internal static Exception UnsupportedVersion(string collectionName)
        {
            return Argument(SR.GetString(Strings.MDF_UnsupportedVersion, collectionName));
        }

        internal static Exception AmbigousCollectionName(string collectionName)
        {
            return Argument(SR.GetString(Strings.MDF_AmbigousCollectionName, collectionName));
        }

        internal static Exception PrepareParameterSize(DbCommand cmd)
        {
            return InvalidOperation(SR.GetString(Strings.ADP_PrepareParameterSize, cmd.GetType().Name));
        }

        internal static Exception PrepareParameterScale(DbCommand cmd, string type)
        {
            return InvalidOperation(SR.GetString(Strings.ADP_PrepareParameterScale, cmd.GetType().Name, type));
        }

        internal static Exception MissingDataSourceInformationColumn()
        {
            return Argument(SR.GetString(Strings.MDF_MissingDataSourceInformationColumn));
        }

        internal static Exception IncorrectNumberOfDataSourceInformationRows()
        {
            return Argument(SR.GetString(Strings.MDF_IncorrectNumberOfDataSourceInformationRows));
        }

        internal static Exception MismatchedAsyncResult(string expectedMethod, string gotMethod)
        {
            return InvalidOperation(SR.GetString(Strings.ADP_MismatchedAsyncResult, expectedMethod, gotMethod));
        }

        //
        // : ConnectionUtil
        //
        internal static Exception ClosedConnectionError()
        {
            return InvalidOperation(SR.GetString(Strings.ADP_ClosedConnectionError));
        }

        internal static Exception ConnectionAlreadyOpen(ConnectionState state)
        {
            return InvalidOperation(SR.GetString(Strings.ADP_ConnectionAlreadyOpen, ConnectionStateMsg(state)));
        }

        internal static Exception TransactionPresent()
        {
            return InvalidOperation(SR.GetString(Strings.ADP_TransactionPresent));
        }

        internal static Exception LocalTransactionPresent()
        {
            return InvalidOperation(SR.GetString(Strings.ADP_LocalTransactionPresent));
        }

        internal static Exception OpenConnectionPropertySet(string property, ConnectionState state)
        {
            return InvalidOperation(SR.GetString(Strings.ADP_OpenConnectionPropertySet, property, ConnectionStateMsg(state)));
        }

        internal static Exception EmptyDatabaseName()
        {
            return Argument(SR.GetString(Strings.ADP_EmptyDatabaseName));
        }

        internal static Exception MissingRestrictionColumn()
        {
            return Argument(SR.GetString(Strings.MDF_MissingRestrictionColumn));
        }

        internal static Exception InternalConnectionError(ConnectionError internalError)
        {
            return InvalidOperation(SR.GetString(Strings.ADP_InternalConnectionError, (int) internalError));
        }

        internal static Exception InvalidConnectRetryCountValue()
        {
            return Argument(SR.GetString(Strings.SQLCR_InvalidConnectRetryCountValue));
        }

        internal static Exception MissingRestrictionRow()
        {
            return Argument(SR.GetString(Strings.MDF_MissingRestrictionRow));
        }

        internal static Exception InvalidConnectRetryIntervalValue()
        {
            return Argument(SR.GetString(Strings.SQLCR_InvalidConnectRetryIntervalValue));
        }

        //
        // : DbDataReader
        //
        internal static InvalidOperationException AsyncOperationPending()
        {
            return InvalidOperation(SR.GetString(Strings.ADP_PendingAsyncOperation));
        }

        //
        // : Stream
        //
        internal static IOException ErrorReadingFromStream(Exception internalException)
        {
            return IO(SR.GetString(Strings.SqlMisc_StreamErrorMessage), internalException);
        }

        internal static ArgumentException InvalidDataType(TypeCode typecode)
        {
            return Argument(SR.GetString(Strings.ADP_InvalidDataType, typecode.ToString()));
        }

        internal static ArgumentException UnknownDataType(Type dataType)
        {
            return Argument(SR.GetString(Strings.ADP_UnknownDataType, dataType.FullName));
        }

        internal static ArgumentException DbTypeNotSupported(DbType type, Type enumtype)
        {
            return Argument(SR.GetString(Strings.ADP_DbTypeNotSupported, type.ToString(), enumtype.Name));
        }

        internal static ArgumentException UnknownDataTypeCode(Type dataType, TypeCode typeCode)
        {
            return Argument(SR.GetString(Strings.ADP_UnknownDataTypeCode, ((int) typeCode).ToString(CultureInfo.InvariantCulture), dataType.FullName));
        }

        internal static ArgumentException InvalidOffsetValue(int value)
        {
            return Argument(SR.GetString(Strings.ADP_InvalidOffsetValue, value.ToString(CultureInfo.InvariantCulture)));
        }

        internal static ArgumentException InvalidSizeValue(int value)
        {
            return Argument(SR.GetString(Strings.ADP_InvalidSizeValue, value.ToString(CultureInfo.InvariantCulture)));
        }

        internal static ArgumentException ParameterValueOutOfRange(decimal value)
        {
            return Argument(SR.GetString(Strings.ADP_ParameterValueOutOfRange, value.ToString((IFormatProvider) null)));
        }

        internal static ArgumentException ParameterValueOutOfRange(SqlDecimal value)
        {
            return Argument(SR.GetString(Strings.ADP_ParameterValueOutOfRange, value.ToString()));
        }

        internal static ArgumentException VersionDoesNotSupportDataType(string typeName)
        {
            return Argument(SR.GetString(Strings.ADP_VersionDoesNotSupportDataType, typeName));
        }

        internal static Exception ParameterConversionFailed(object value, Type destType, Exception inner)
        {
            Debug.Assert(null != value, "null value on conversion failure");
            Debug.Assert(null != inner, "null inner on conversion failure");

            Exception e;
            var message = SR.GetString(Strings.ADP_ParameterConversionFailed, value.GetType().Name, destType.Name);
            if (inner is ArgumentException)
                e = new ArgumentException(message, inner);
            else if (inner is FormatException)
                e = new FormatException(message, inner);
            else if (inner is InvalidCastException)
                e = new InvalidCastException(message, inner);
            else if (inner is OverflowException)
                e = new OverflowException(message, inner);
            else
                e = inner;
            return e;
        }

        //
        // : IDataParameterCollection
        //
        internal static Exception ParametersMappingIndex(int index, DbParameterCollection collection)
        {
            return CollectionIndexInt32(index, collection.GetType(), collection.Count);
        }

        internal static Exception ParametersSourceIndex(string parameterName, DbParameterCollection collection, Type parameterType)
        {
            return CollectionIndexString(parameterType, ParameterName, parameterName, collection.GetType());
        }

        internal static Exception ParameterNull(string parameter, DbParameterCollection collection, Type parameterType)
        {
            return CollectionNullValue(parameter, collection.GetType(), parameterType);
        }

        internal static Exception UndefinedPopulationMechanism(string populationMechanism)
        {
            throw new NotImplementedException();
        }

        internal static Exception InvalidParameterType(DbParameterCollection collection, Type parameterType, object invalidValue)
        {
            return CollectionInvalidType(collection.GetType(), parameterType, invalidValue);
        }

        //
        // : IDbTransaction
        //
        internal static Exception ParallelTransactionsNotSupported(DbConnection obj)
        {
            return InvalidOperation(SR.GetString(Strings.ADP_ParallelTransactionsNotSupported, obj.GetType().Name));
        }

        internal static Exception TransactionZombied(DbTransaction obj)
        {
            return InvalidOperation(SR.GetString(Strings.ADP_TransactionZombied, obj.GetType().Name));
        }

        internal static Delegate FindBuilder(MulticastDelegate mcd)
        {
            if (null != mcd)
                foreach (var del in mcd.GetInvocationList())
                    if (del.Target is DbCommandBuilder)
                        return del;

            return null;
        }

        internal static void TimerCurrent(out long ticks)
        {
            ticks = DateTime.UtcNow.ToFileTimeUtc();
        }

        internal static long TimerCurrent()
        {
            return DateTime.UtcNow.ToFileTimeUtc();
        }

        internal static long TimerFromSeconds(int seconds)
        {
            var result = checked(seconds * TimeSpan.TicksPerSecond);
            return result;
        }

        internal static long TimerFromMilliseconds(long milliseconds)
        {
            var result = checked(milliseconds * TimeSpan.TicksPerMillisecond);
            return result;
        }

        internal static bool TimerHasExpired(long timerExpire)
        {
            var result = TimerCurrent() > timerExpire;
            return result;
        }

        internal static long TimerRemaining(long timerExpire)
        {
            var timerNow = TimerCurrent();
            var result = checked(timerExpire - timerNow);
            return result;
        }

        internal static long TimerRemainingMilliseconds(long timerExpire)
        {
            var result = TimerToMilliseconds(TimerRemaining(timerExpire));
            return result;
        }

        internal static long TimerRemainingSeconds(long timerExpire)
        {
            var result = TimerToSeconds(TimerRemaining(timerExpire));
            return result;
        }

        internal static long TimerToMilliseconds(long timerValue)
        {
            var result = timerValue / TimeSpan.TicksPerMillisecond;
            return result;
        }

        private static long TimerToSeconds(long timerValue)
        {
            var result = timerValue / TimeSpan.TicksPerSecond;
            return result;
        }

        internal static string MachineName()
        {
            return Environment.MachineName;
        }

        internal static ST.Transaction GetCurrentTransaction()
        {
            return ST.Transaction.Current;
        }

        internal static bool IsDirection(DbParameter value, ParameterDirection condition)
        {
            // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
            return condition == (condition & value.Direction);
        }

        internal static void IsNullOrSqlType(object value, out bool isNull, out bool isSqlType)
        {
            if (value == null || value == DBNull.Value)
            {
                isNull = true;
                isSqlType = false;
            }
            else
            {
                var nullable = value as INullable;
                if (nullable != null)
                {
                    isNull = nullable.IsNull;
                    // Duplicated from DataStorage.cs
                    // For back-compat, SqlXml is not in this list
                    isSqlType = value is SqlBinary ||
                                value is SqlBoolean ||
                                value is SqlByte ||
                                value is SqlBytes ||
                                value is SqlChars ||
                                value is SqlDateTime ||
                                value is SqlDecimal ||
                                value is SqlDouble ||
                                value is SqlGuid ||
                                value is SqlInt16 ||
                                value is SqlInt32 ||
                                value is SqlInt64 ||
                                value is SqlMoney ||
                                value is SqlSingle ||
                                value is SqlString;
                }
                else
                {
                    isNull = false;
                    isSqlType = false;
                }
            }
        }

        internal static Version GetAssemblyVersion()
        {
            // NOTE: Using lazy thread-safety since we don't care if two threads both happen to update the value at the same time
            if (_sSystemDataVersion == null) _sSystemDataVersion = new Version("0.0.0.0");

            return _sSystemDataVersion;
        }

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

        internal static ArgumentOutOfRangeException InvalidDataRowVersion(DataRowVersion value)
        {
            return InvalidEnumerationValue(typeof(DataRowVersion), (int) value);
        }

        internal static ArgumentException SingleValuedProperty(string propertyName, string value)
        {
            var e = new ArgumentException(SR.GetString(Strings.ADP_SingleValuedProperty, propertyName, value));
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static ArgumentException DoubleValuedProperty(string propertyName, string value1, string value2)
        {
            var e = new ArgumentException(SR.GetString(Strings.ADP_DoubleValuedProperty, propertyName, value1, value2));
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static ArgumentException InvalidPrefixSuffix()
        {
            var e = new ArgumentException(SR.GetString(Strings.ADP_InvalidPrefixSuffix));
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static ArgumentOutOfRangeException InvalidCommandBehavior(CommandBehavior value)
        {
            Debug.Assert(0 > (int) value || (int) value > 0x3F, "valid CommandType " + value);

            return InvalidEnumerationValue(typeof(CommandBehavior), (int) value);
        }

        internal static void ValidateCommandBehavior(CommandBehavior value)
        {
            if ((int) value < 0 || 0x3F < (int) value) throw InvalidCommandBehavior(value);
        }

        internal static ArgumentOutOfRangeException NotSupportedCommandBehavior(CommandBehavior value, string method)
        {
            return NotSupportedEnumerationValue(typeof(CommandBehavior), value.ToString(), method);
        }

        internal static ArgumentException BadParameterName(string parameterName)
        {
            var e = new ArgumentException(SR.GetString(Strings.ADP_BadParameterName, parameterName));
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static Exception DeriveParametersNotSupported(IDbCommand value)
        {
            return DataAdapter(SR.GetString(Strings.ADP_DeriveParametersNotSupported, value.GetType().Name, value.CommandType.ToString()));
        }

        internal static Exception NoStoredProcedureExists(string sproc)
        {
            return InvalidOperation(SR.GetString(Strings.ADP_NoStoredProcedureExists, sproc));
        }

        //
        // DbProviderException
        //
        internal static InvalidOperationException TransactionCompletedButNotDisposed()
        {
            return Provider(SR.GetString(Strings.ADP_TransactionCompletedButNotDisposed));
        }

        internal static ArgumentOutOfRangeException InvalidUserDefinedTypeSerializationFormat(object value)
        {
            return InvalidEnumerationValue(typeof(object), (int) value);
        }

        internal static ArgumentOutOfRangeException NotSupportedUserDefinedTypeSerializationFormat(object value, string method)
        {
            return NotSupportedEnumerationValue(typeof(object), value.ToString(), method);
        }

        internal static ArgumentOutOfRangeException ArgumentOutOfRange(string message, string parameterName, object value)
        {
            var e = new ArgumentOutOfRangeException(parameterName, value, message);
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal enum ConnectionError
        {
            BeginGetConnectionReturnsNull,
            GetConnectionReturnsNull,
            ConnectionOptionsMissing,
            CouldNotSwitchToClosedPreviouslyOpenedState
        }
    }

    internal static partial class ADP
    {
        internal static Timer UnsafeCreateTimer(TimerCallback callback, object state, int dueTime, int period)
        {
            // Don't capture the current ExecutionContext and its AsyncLocals onto 
            // a global timer causing them to live forever
            var restoreFlow = false;
            try
            {
                if (!ExecutionContext.IsFlowSuppressed())
                {
                    ExecutionContext.SuppressFlow();
                    restoreFlow = true;
                }

                return new Timer(callback, state, dueTime, period);
            }
            finally
            {
                // Restore the current ExecutionContext
                if (restoreFlow)
                    ExecutionContext.RestoreFlow();
            }
        }
    }

    internal static partial class ADP
    {
        internal const CompareOptions DefaultCompareOptions = CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth | CompareOptions.IgnoreCase;

        internal const int DefaultConnectionTimeout = DbConnectionStringDefaults.ConnectTimeout;

        // NOTE: Initializing a Task in SQL CLR requires the "UNSAFE" permission set (http://msdn.microsoft.com/en-us/library/ms172338.aspx)
        // Therefore we are lazily initializing these Tasks to avoid forcing customers to use the "UNSAFE" set when they are actually using no Async features
        private static Task<bool> _trueTask;

        private static Task<bool> _falseTask;

        // only StackOverflowException & ThreadAbortException are sealed classes
        private static readonly Type SStackOverflowType = typeof(StackOverflowException);
        private static readonly Type SOutOfMemoryType = typeof(OutOfMemoryException);
        private static readonly Type SThreadAbortType = typeof(ThreadAbortException);
        private static readonly Type SNullReferenceType = typeof(NullReferenceException);
        private static readonly Type SAccessViolationType = typeof(AccessViolationException);
        private static readonly Type SSecurityType = typeof(SecurityException);

        internal static readonly bool IsWindowsNt = PlatformID.Win32NT == Environment.OSVersion.Platform;
        internal static readonly bool IsPlatformNt5 = IsWindowsNt && Environment.OSVersion.Version.Major >= 5;
        internal static Task<bool> TrueTask => _trueTask ?? (_trueTask = Task.FromResult(true));
        internal static Task<bool> FalseTask => _falseTask ?? (_falseTask = Task.FromResult(false));

        static partial void TraceException(string trace, Exception e);

        internal static void TraceExceptionAsReturnValue(Exception e)
        {
            TraceException("<comm.ADP.TraceException|ERR|THROW> '{0}'", e);
        }

        internal static void TraceExceptionWithoutRethrow(Exception e)
        {
            Debug.Assert(IsCatchableExceptionType(e), "Invalid exception type, should have been re-thrown!");
            TraceException("<comm.ADP.TraceException|ERR|CATCH> '%ls'\n", e);
        }

        internal static ArgumentException Argument(string error)
        {
            var e = new ArgumentException(error);
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static ArgumentException Argument(string error, Exception inner)
        {
            var e = new ArgumentException(error, inner);
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static ArgumentException Argument(string error, string parameter)
        {
            var e = new ArgumentException(error, parameter);
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static ArgumentNullException ArgumentNull(string parameter)
        {
            var e = new ArgumentNullException(parameter);
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static ArgumentNullException ArgumentNull(string parameter, string error)
        {
            var e = new ArgumentNullException(parameter, error);
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static ArgumentOutOfRangeException ArgumentOutOfRange(string parameterName)
        {
            var e = new ArgumentOutOfRangeException(parameterName);
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static ArgumentOutOfRangeException ArgumentOutOfRange(string message, string parameterName)
        {
            var e = new ArgumentOutOfRangeException(parameterName, message);
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static IndexOutOfRangeException IndexOutOfRange(string error)
        {
            var e = new IndexOutOfRangeException(error);
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static InvalidCastException InvalidCast(string error)
        {
            return InvalidCast(error, null);
        }

        internal static InvalidCastException InvalidCast(string error, Exception inner)
        {
            var e = new InvalidCastException(error, inner);
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static InvalidOperationException InvalidOperation(string error)
        {
            var e = new InvalidOperationException(error);
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static NotSupportedException NotSupported()
        {
            var e = new NotSupportedException();
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static NotSupportedException NotSupported(string error)
        {
            var e = new NotSupportedException(error);
            TraceExceptionAsReturnValue(e);
            return e;
        }

        // the return value is true if the string was quoted and false if it was not
        // this allows the caller to determine if it is an error or not for the quotedString to not be quoted
        internal static bool RemoveStringQuotes(string quotePrefix, string quoteSuffix, string quotedString, out string unquotedString)
        {
            var prefixLength = quotePrefix?.Length ?? 0;
            var suffixLength = quoteSuffix?.Length ?? 0;

            if (suffixLength + prefixLength == 0)
            {
                unquotedString = quotedString;
                return true;
            }

            if (quotedString == null)
            {
                unquotedString = null;
                return false;
            }

            var quotedStringLength = quotedString.Length;

            // is the source string too short to be quoted
            if (quotedStringLength < prefixLength + suffixLength)
            {
                unquotedString = quotedString;
                return false;
            }

            // is the prefix present?
            if (prefixLength > 0)
                if (!quotedString.StartsWith(quotePrefix, StringComparison.Ordinal))
                {
                    unquotedString = quotedString;
                    return false;
                }

            // is the suffix present?
            if (suffixLength > 0)
            {
                if (!quotedString.EndsWith(quoteSuffix, StringComparison.Ordinal))
                {
                    unquotedString = quotedString;
                    return false;
                }

                unquotedString = quotedString.Substring(prefixLength, quotedStringLength - (prefixLength + suffixLength)).Replace(quoteSuffix + quoteSuffix, quoteSuffix);
            }
            else
            {
                unquotedString = quotedString.Substring(prefixLength, quotedStringLength - prefixLength);
            }

            return true;
        }

        internal static ArgumentOutOfRangeException NotSupportedEnumerationValue(Type type, string value, string method)
        {
            return ArgumentOutOfRange(SR.Format(Strings.ADP_NotSupportedEnumerationValue, type.Name, value, method), type.Name);
        }

        internal static InvalidOperationException DataAdapter(string error)
        {
            return InvalidOperation(error);
        }

        private static InvalidOperationException Provider(string error)
        {
            return InvalidOperation(error);
        }

        internal static ArgumentException InvalidMultipartName(string property, string value)
        {
            var e = new ArgumentException(SR.Format(Strings.ADP_InvalidMultipartName, property, value));
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static ArgumentException InvalidMultipartNameIncorrectUsageOfQuotes(string property, string value)
        {
            var e = new ArgumentException(SR.Format(Strings.ADP_InvalidMultipartNameQuoteUsage, property, value));
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static ArgumentException InvalidMultipartNameToManyParts(string property, string value, int limit)
        {
            var e = new ArgumentException(SR.Format(Strings.ADP_InvalidMultipartNameToManyParts, property, value, limit));
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static void CheckArgumentNull(object value, string parameterName)
        {
            if (null == value) throw ArgumentNull(parameterName);
        }

        internal static bool IsCatchableExceptionType(Exception e)
        {
            // a 'catchable' exception is defined by what it is not.
            Debug.Assert(e != null, "Unexpected null exception!");
            var type = e.GetType();

            return type != SStackOverflowType &&
                   type != SOutOfMemoryType &&
                   type != SThreadAbortType &&
                   type != SNullReferenceType &&
                   type != SAccessViolationType &&
                   !SSecurityType.IsAssignableFrom(type);
        }

        internal static bool IsCatchableOrSecurityExceptionType(Exception e)
        {
            // a 'catchable' exception is defined by what it is not.
            // since IsCatchableExceptionType defined SecurityException as not 'catchable'
            // this method will return true for SecurityException has being catchable.

            // the other way to write this method is, but then SecurityException is checked twice
            // return ((e is SecurityException) || IsCatchableExceptionType(e));

            Debug.Assert(e != null, "Unexpected null exception!");
            var type = e.GetType();

            return type != SStackOverflowType &&
                   type != SOutOfMemoryType &&
                   type != SThreadAbortType &&
                   type != SNullReferenceType &&
                   type != SAccessViolationType;
        }

        // Invalid Enumeration
        internal static ArgumentOutOfRangeException InvalidEnumerationValue(Type type, int value)
        {
            return ArgumentOutOfRange(SR.Format(Strings.ADP_InvalidEnumerationValue, type.Name, value.ToString(CultureInfo.InvariantCulture)), type.Name);
        }

        //
        // DbConnectionOptions, DataAccess
        //
        internal static ArgumentException ConnectionStringSyntax(int index)
        {
            return Argument(SR.Format(Strings.ADP_ConnectionStringSyntax, index));
        }

        internal static ArgumentException KeywordNotSupported(string keyword)
        {
            return Argument(SR.Format(Strings.ADP_KeywordNotSupported, keyword));
        }

        internal static ArgumentException ConvertFailed(Type fromType, Type toType, Exception innerException)
        {
            return Argument(SR.Format(Strings.SqlConvert_ConvertFailed, fromType.FullName, toType.FullName), innerException);
        }

        //
        // DbConnectionOptions, DataAccess, SqlClient
        //
        internal static Exception InvalidConnectionOptionValue(string key)
        {
            return InvalidConnectionOptionValue(key, null);
        }

        internal static Exception InvalidConnectionOptionValue(string key, Exception inner)
        {
            return Argument(SR.Format(Strings.ADP_InvalidConnectionOptionValue, key), inner);
        }

        //
        // Generic Data Provider Collection
        //
        internal static ArgumentException CollectionRemoveInvalidObject(Type itemType, ICollection collection)
        {
            return Argument(SR.Format(Strings.ADP_CollectionRemoveInvalidObject, itemType.Name, collection.GetType().Name));
        }

        internal static ArgumentNullException CollectionNullValue(string parameter, Type collection, Type itemType)
        {
            return ArgumentNull(parameter, SR.Format(Strings.ADP_CollectionNullValue, collection.Name, itemType.Name));
        }

        internal static IndexOutOfRangeException CollectionIndexInt32(int index, Type collection, int count)
        {
            return IndexOutOfRange(SR.Format(Strings.ADP_CollectionIndexInt32, index.ToString(CultureInfo.InvariantCulture), collection.Name, count.ToString(CultureInfo.InvariantCulture)));
        }

        internal static IndexOutOfRangeException CollectionIndexString(Type itemType, string propertyName, string propertyValue, Type collection)
        {
            return IndexOutOfRange(SR.Format(Strings.ADP_CollectionIndexString, itemType.Name, propertyName, propertyValue, collection.Name));
        }

        internal static InvalidCastException CollectionInvalidType(Type collection, Type itemType, object invalidValue)
        {
            return InvalidCast(SR.Format(Strings.ADP_CollectionInvalidType, collection.Name, itemType.Name, invalidValue.GetType().Name));
        }

        //
        // DbConnection
        //
        private static string ConnectionStateMsg(ConnectionState state)
        {
            switch (state)
            {
                case ConnectionState.Closed:
                case ConnectionState.Connecting | ConnectionState.Broken: // treated the same as closed
                    return Strings.ADP_ConnectionStateMsg_Closed;
                case ConnectionState.Connecting:
                    return Strings.ADP_ConnectionStateMsg_Connecting;
                case ConnectionState.Open:
                    return Strings.ADP_ConnectionStateMsg_Open;
                case ConnectionState.Open | ConnectionState.Executing:
                    return Strings.ADP_ConnectionStateMsg_OpenExecuting;
                case ConnectionState.Open | ConnectionState.Fetching:
                    return Strings.ADP_ConnectionStateMsg_OpenFetching;
                default:
                    return SR.Format(Strings.ADP_ConnectionStateMsg, state.ToString());
            }
        }

        //
        // : Stream
        //
        internal static Exception StreamClosed([CallerMemberName] string method = "")
        {
            return InvalidOperation(SR.Format(Strings.ADP_StreamClosed, method));
        }

        internal static string BuildQuotedString(string quotePrefix, string quoteSuffix, string unQuotedString)
        {
            var resultString = new StringBuilder();
            if (!string.IsNullOrEmpty(quotePrefix)) resultString.Append(quotePrefix);

            // Assuming that the suffix is escaped by doubling it. i.e. foo"bar becomes "foo""bar".
            if (!string.IsNullOrEmpty(quoteSuffix))
            {
                resultString.Append(unQuotedString.Replace(quoteSuffix, quoteSuffix + quoteSuffix));
                resultString.Append(quoteSuffix);
            }
            else
            {
                resultString.Append(unQuotedString);
            }

            return resultString.ToString();
        }

        //
        // Generic Data Provider Collection
        //
        internal static ArgumentException ParametersIsNotParent(Type parameterType, ICollection collection)
        {
            return Argument(SR.Format(Strings.ADP_CollectionIsNotParent, parameterType.Name, collection.GetType().Name));
        }

        internal static ArgumentException ParametersIsParent(Type parameterType, ICollection collection)
        {
            return Argument(SR.Format(Strings.ADP_CollectionIsNotParent, parameterType.Name, collection.GetType().Name));
        }

        internal static Exception InternalError(InternalErrorCode internalError)
        {
            return InvalidOperation(SR.Format(Strings.ADP_InternalProviderError, (int) internalError));
        }

        //
        // : DbDataReader
        //
        internal static Exception DataReaderClosed([CallerMemberName] string method = "")
        {
            return InvalidOperation(SR.Format(Strings.ADP_DataReaderClosed, method));
        }

        internal static ArgumentOutOfRangeException InvalidSourceBufferIndex(int maxLen, long srcOffset, string parameterName)
        {
            return ArgumentOutOfRange(SR.Format(Strings.ADP_InvalidSourceBufferIndex, maxLen.ToString(CultureInfo.InvariantCulture), srcOffset.ToString(CultureInfo.InvariantCulture)), parameterName);
        }

        internal static ArgumentOutOfRangeException InvalidDestinationBufferIndex(int maxLen, int dstOffset, string parameterName)
        {
            return ArgumentOutOfRange(SR.Format(Strings.ADP_InvalidDestinationBufferIndex, maxLen.ToString(CultureInfo.InvariantCulture), dstOffset.ToString(CultureInfo.InvariantCulture)), parameterName);
        }

        internal static IndexOutOfRangeException InvalidBufferSizeOrIndex(int numBytes, int bufferIndex)
        {
            return IndexOutOfRange(SR.Format(Strings.SQL_InvalidBufferSizeOrIndex, numBytes.ToString(CultureInfo.InvariantCulture), bufferIndex.ToString(CultureInfo.InvariantCulture)));
        }

        internal static Exception InvalidDataLength(long length)
        {
            return IndexOutOfRange(SR.Format(Strings.SQL_InvalidDataLength, length.ToString(CultureInfo.InvariantCulture)));
        }

        internal static bool CompareInsensitiveInvariant(string strvalue, string strconst)
        {
            return 0 == CultureInfo.InvariantCulture.CompareInfo.Compare(strvalue, strconst, CompareOptions.IgnoreCase);
        }

        internal static int DstCompare(string strA, string strB)
        {
            return CultureInfo.CurrentCulture.CompareInfo.Compare(strA, strB, DefaultCompareOptions);
        }

        internal static bool IsEmptyArray(string[] array)
        {
            return null == array || 0 == array.Length;
        }

        internal static bool IsNull(object value)
        {
            if (null == value || DBNull.Value == value) return true;
            return value is INullable nullable && nullable.IsNull;
        }

        internal static Exception InvalidSeekOrigin(string parameterName)
        {
            return ArgumentOutOfRange(Strings.ADP_InvalidSeekOrigin, parameterName);
        }

        internal static void SetCurrentTransaction(ST.Transaction transaction)
        {
            ST.Transaction.Current = transaction;
        }


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