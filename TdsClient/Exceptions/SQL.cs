using System;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Transactions;
using Medella.TdsClient.Contants;
using Medella.TdsClient.Resources;
using Medella.TdsClient.SNI.Native;
using IsolationLevel = System.Transactions.IsolationLevel;

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
        internal static Exception CannotGetDTCAddress()
        {
            return ADP.InvalidOperation(SR.GetString(Strings.SQL_CannotGetDTCAddress));
        }

        internal static Exception InvalidInternalPacketSize(string str)
        {
            return ADP.ArgumentOutOfRange(str);
        }

        internal static Exception InvalidPacketSize()
        {
            return ADP.ArgumentOutOfRange(SR.GetString(Strings.SQL_InvalidTDSPacketSize));
        }

        internal static Exception InvalidPacketSizeValue()
        {
            return ADP.Argument(SR.GetString(Strings.SQL_InvalidPacketSizeValue));
        }

        internal static Exception InvalidSSPIPacketSize()
        {
            return ADP.Argument(SR.GetString(Strings.SQL_InvalidSSPIPacketSize));
        }

        internal static Exception NullEmptyTransactionName()
        {
            return ADP.Argument(SR.GetString(Strings.SQL_NullEmptyTransactionName));
        }

        internal static Exception UserInstanceFailoverNotCompatible()
        {
            return ADP.Argument(SR.GetString(Strings.SQL_UserInstanceFailoverNotCompatible));
        }

        internal static Exception InvalidSQLServerVersionUnknown()
        {
            return ADP.DataAdapter(SR.GetString(Strings.SQL_InvalidSQLServerVersionUnknown));
        }

        internal static Exception SynchronousCallMayNotPend()
        {
            return new Exception(SR.GetString(Strings.Sql_InternalError));
        }

        internal static Exception ConnectionLockedForBcpEvent()
        {
            return ADP.InvalidOperation(SR.GetString(Strings.SQL_ConnectionLockedForBcpEvent));
        }

        internal static Exception InstanceFailure()
        {
            return ADP.InvalidOperation(SR.GetString(Strings.SQL_InstanceFailure));
        }

        //
        // Global Transactions.
        //
        internal static Exception GlobalTransactionsNotEnabled()
        {
            return ADP.InvalidOperation(SR.GetString(Strings.GT_Disabled));
        }

        internal static Exception UnknownSysTxIsolationLevel(IsolationLevel isolationLevel)
        {
            return ADP.InvalidOperation(SR.GetString(Strings.SQL_UnknownSysTxIsolationLevel, isolationLevel.ToString()));
        }


        internal static Exception InvalidPartnerConfiguration(string server, string database)
        {
            return ADP.InvalidOperation(SR.GetString(Strings.SQL_InvalidPartnerConfiguration, server, database));
        }

        internal static Exception MARSUnspportedOnConnection()
        {
            return ADP.InvalidOperation(SR.GetString(Strings.SQL_MarsUnsupportedOnConnection));
        }

        internal static Exception CannotModifyPropertyAsyncOperationInProgress([CallerMemberName] string property = "")
        {
            return ADP.InvalidOperation(SR.GetString(Strings.SQL_CannotModifyPropertyAsyncOperationInProgress, property));
        }

        internal static Exception NonLocalSSEInstance()
        {
            return ADP.NotSupported(SR.GetString(Strings.SQL_NonLocalSSEInstance));
        }
        //
        // SQL.DataCommand
        //

        internal static ArgumentOutOfRangeException NotSupportedEnumerationValue(Type type, int value)
        {
            return ADP.ArgumentOutOfRange(SR.GetString(Strings.SQL_NotSupportedEnumerationValue, type.Name, value.ToString(CultureInfo.InvariantCulture)), type.Name);
        }

        internal static ArgumentOutOfRangeException NotSupportedCommandType(CommandType value)
        {
#if DEBUG
            switch (value)
            {
                case CommandType.Text:
                case CommandType.StoredProcedure:
                    Debug.Assert(false, "valid CommandType " + value);
                    break;
                case CommandType.TableDirect:
                    break;
                default:
                    Debug.Assert(false, "invalid CommandType " + value);
                    break;
            }
#endif
            return NotSupportedEnumerationValue(typeof(CommandType), (int) value);
        }

        internal static ArgumentOutOfRangeException NotSupportedIsolationLevel(System.Data.IsolationLevel value)
        {
#if DEBUG
            switch (value)
            {
                case System.Data.IsolationLevel.Unspecified:
                case System.Data.IsolationLevel.ReadCommitted:
                case System.Data.IsolationLevel.ReadUncommitted:
                case System.Data.IsolationLevel.RepeatableRead:
                case System.Data.IsolationLevel.Serializable:
                case System.Data.IsolationLevel.Snapshot:
                    Debug.Assert(false, "valid IsolationLevel " + value);
                    break;
                case System.Data.IsolationLevel.Chaos:
                    break;
                default:
                    Debug.Assert(false, "invalid IsolationLevel " + value);
                    break;
            }
#endif
            return NotSupportedEnumerationValue(typeof(System.Data.IsolationLevel), (int) value);
        }

        internal static Exception OperationCancelled()
        {
            Exception exception = ADP.InvalidOperation(SR.GetString(Strings.SQL_OperationCancelled));
            return exception;
        }

        internal static Exception PendingBeginXXXExists()
        {
            return ADP.InvalidOperation(SR.GetString(Strings.SQL_PendingBeginXXXExists));
        }

        internal static ArgumentOutOfRangeException InvalidSqlDependencyTimeout(string param)
        {
            return ADP.ArgumentOutOfRange(SR.GetString(Strings.SqlDependency_InvalidTimeout), param);
        }

        internal static Exception NonXmlResult()
        {
            return ADP.InvalidOperation(SR.GetString(Strings.SQL_NonXmlResult));
        }

        //
        // SQL.DataParameter
        //
        internal static Exception InvalidUdt3PartNameFormat()
        {
            return ADP.Argument(SR.GetString(Strings.SQL_InvalidUdt3PartNameFormat));
        }

        internal static Exception InvalidParameterTypeNameFormat()
        {
            return ADP.Argument(SR.GetString(Strings.SQL_InvalidParameterTypeNameFormat));
        }

        internal static Exception InvalidParameterNameLength(string value)
        {
            return ADP.Argument(SR.GetString(Strings.SQL_InvalidParameterNameLength, value));
        }

        internal static Exception PrecisionValueOutOfRange(byte precision)
        {
            return ADP.Argument(SR.GetString(Strings.SQL_PrecisionValueOutOfRange, precision.ToString(CultureInfo.InvariantCulture)));
        }

        internal static Exception ScaleValueOutOfRange(byte scale)
        {
            return ADP.Argument(SR.GetString(Strings.SQL_ScaleValueOutOfRange, scale.ToString(CultureInfo.InvariantCulture)));
        }

        internal static Exception TimeScaleValueOutOfRange(byte scale)
        {
            return ADP.Argument(SR.GetString(Strings.SQL_TimeScaleValueOutOfRange, scale.ToString(CultureInfo.InvariantCulture)));
        }

        internal static Exception InvalidSqlDbType(SqlDbType value)
        {
            return ADP.InvalidEnumerationValue(typeof(SqlDbType), (int) value);
        }

        internal static Exception UnsupportedTVPOutputParameter(ParameterDirection direction, string paramName)
        {
            return ADP.NotSupported(SR.GetString(Strings.SqlParameter_UnsupportedTVPOutputParameter,
                direction.ToString(), paramName));
        }

        internal static Exception DBNullNotSupportedForTVPValues(string paramName)
        {
            return ADP.NotSupported(SR.GetString(Strings.SqlParameter_DBNullNotSupportedForTVP, paramName));
        }

        internal static Exception UnexpectedTypeNameForNonStructParams(string paramName)
        {
            return ADP.NotSupported(SR.GetString(Strings.SqlParameter_UnexpectedTypeNameForNonStruct, paramName));
        }

        internal static Exception ParameterInvalidVariant(string paramName)
        {
            return ADP.InvalidOperation(SR.GetString(Strings.SQL_ParameterInvalidVariant, paramName));
        }

        internal static Exception MustSetTypeNameForParam(string paramType, string paramName)
        {
            return ADP.Argument(SR.GetString(Strings.SQL_ParameterTypeNameRequired, paramType, paramName));
        }

        internal static Exception NullSchemaTableDataTypeNotSupported(string columnName)
        {
            return ADP.Argument(SR.GetString(Strings.NullSchemaTableDataTypeNotSupported, columnName));
        }

        internal static Exception InvalidSchemaTableOrdinals()
        {
            return ADP.Argument(SR.GetString(Strings.InvalidSchemaTableOrdinals));
        }

        internal static Exception EnumeratedRecordMetaDataChanged(string fieldName, int recordNumber)
        {
            return ADP.Argument(SR.GetString(Strings.SQL_EnumeratedRecordMetaDataChanged, fieldName, recordNumber));
        }

        internal static Exception EnumeratedRecordFieldCountChanged(int recordNumber)
        {
            return ADP.Argument(SR.GetString(Strings.SQL_EnumeratedRecordFieldCountChanged, recordNumber));
        }

        //
        // SQL.SqlDataAdapter
        //

        //
        // SQL.TDSParser
        //
        internal static Exception InvalidTDSVersion()
        {
            return ADP.InvalidOperation(SR.GetString(Strings.SQL_InvalidTDSVersion));
        }

        internal static Exception ParsingError()
        {
            return ADP.InvalidOperation(SR.GetString(Strings.SQL_ParsingError));
        }

        internal static Exception MoneyOverflow(string moneyValue)
        {
            return ADP.Overflow(SR.GetString(Strings.SQL_MoneyOverflow, moneyValue));
        }

        internal static Exception SmallDateTimeOverflow(string datetime)
        {
            return ADP.Overflow(SR.GetString(Strings.SQL_SmallDateTimeOverflow, datetime));
        }

        internal static Exception SNIPacketAllocationFailure()
        {
            return ADP.InvalidOperation(SR.GetString(Strings.SQL_SNIPacketAllocationFailure));
        }

        internal static Exception TimeOverflow(string time)
        {
            return ADP.Overflow(SR.GetString(Strings.SQL_TimeOverflow, time));
        }

        //
        // SQL.SqlDataReader
        //
        internal static Exception InvalidRead()
        {
            return ADP.InvalidOperation(SR.GetString(Strings.SQL_InvalidRead));
        }

        internal static Exception NonBlobColumn(string columnName)
        {
            return ADP.InvalidCast(SR.GetString(Strings.SQL_NonBlobColumn, columnName));
        }

        internal static Exception NonCharColumn(string columnName)
        {
            return ADP.InvalidCast(SR.GetString(Strings.SQL_NonCharColumn, columnName));
        }

        internal static Exception StreamNotSupportOnColumnType(string columnName)
        {
            return ADP.InvalidCast(SR.GetString(Strings.SQL_StreamNotSupportOnColumnType, columnName));
        }

        internal static Exception TextReaderNotSupportOnColumnType(string columnName)
        {
            return ADP.InvalidCast(SR.GetString(Strings.SQL_TextReaderNotSupportOnColumnType, columnName));
        }

        internal static Exception XmlReaderNotSupportOnColumnType(string columnName)
        {
            return ADP.InvalidCast(SR.GetString(Strings.SQL_XmlReaderNotSupportOnColumnType, columnName));
        }

        internal static Exception UDTUnexpectedResult(string exceptionText)
        {
            return ADP.TypeLoad(SR.GetString(Strings.SQLUDT_Unexpected, exceptionText));
        }

        //
        // SQL.SqlDependency
        //
        internal static Exception SqlCommandHasExistingSqlNotificationRequest()
        {
            return ADP.InvalidOperation(SR.GetString(Strings.SQLNotify_AlreadyHasCommand));
        }

        internal static Exception SqlDepDefaultOptionsButNoStart()
        {
            return ADP.InvalidOperation(SR.GetString(Strings.SqlDependency_DefaultOptionsButNoStart));
        }

        internal static Exception SqlDependencyDatabaseBrokerDisabled()
        {
            return ADP.InvalidOperation(SR.GetString(Strings.SqlDependency_DatabaseBrokerDisabled));
        }

        internal static Exception SqlDependencyEventNoDuplicate()
        {
            return ADP.InvalidOperation(SR.GetString(Strings.SqlDependency_EventNoDuplicate));
        }

        internal static Exception SqlDependencyDuplicateStart()
        {
            return ADP.InvalidOperation(SR.GetString(Strings.SqlDependency_DuplicateStart));
        }

        internal static Exception SqlDependencyIdMismatch()
        {
            return ADP.InvalidOperation(SR.GetString(Strings.SqlDependency_IdMismatch));
        }

        internal static Exception SqlDependencyNoMatchingServerStart()
        {
            return ADP.InvalidOperation(SR.GetString(Strings.SqlDependency_NoMatchingServerStart));
        }

        internal static Exception SqlDependencyNoMatchingServerDatabaseStart()
        {
            return ADP.InvalidOperation(SR.GetString(Strings.SqlDependency_NoMatchingServerDatabaseStart));
        }

        //
        // SQL.SqlDelegatedTransaction
        //
        internal static TransactionPromotionException PromotionFailed(Exception inner)
        {
            var e = new TransactionPromotionException(SR.GetString(Strings.SqlDelegatedTransaction_PromotionFailed), inner);
            ADP.TraceExceptionAsReturnValue(e);
            return e;
        }
        //Failure while attempting to promote transaction.

        //
        // SQL.SqlMetaData
        //
        internal static Exception UnexpectedUdtTypeNameForNonUdtParams()
        {
            return ADP.Argument(SR.GetString(Strings.SQLUDT_UnexpectedUdtTypeName));
        }

        internal static Exception MustSetUdtTypeNameForUdtParams()
        {
            return ADP.Argument(SR.GetString(Strings.SQLUDT_InvalidUdtTypeName));
        }

        internal static Exception UDTInvalidSqlType(string typeName)
        {
            return ADP.Argument(SR.GetString(Strings.SQLUDT_InvalidSqlType, typeName));
        }

        internal static Exception InvalidSqlDbTypeForConstructor(SqlDbType type)
        {
            return ADP.Argument(SR.GetString(Strings.SqlMetaData_InvalidSqlDbTypeForConstructorFormat, type.ToString()));
        }

        internal static Exception NameTooLong(string parameterName)
        {
            return ADP.Argument(SR.GetString(Strings.SqlMetaData_NameTooLong), parameterName);
        }

        internal static Exception InvalidSortOrder(SortOrder order)
        {
            return ADP.InvalidEnumerationValue(typeof(SortOrder), (int) order);
        }

        internal static Exception MustSpecifyBothSortOrderAndOrdinal(SortOrder order, int ordinal)
        {
            return ADP.InvalidOperation(SR.GetString(Strings.SqlMetaData_SpecifyBothSortOrderAndOrdinal, order.ToString(), ordinal));
        }

        internal static Exception UnsupportedColumnTypeForSqlProvider(string columnName, string typeName)
        {
            return ADP.Argument(SR.GetString(Strings.SqlProvider_InvalidDataColumnType, columnName, typeName));
        }

        internal static Exception InvalidColumnMaxLength(string columnName, long maxLength)
        {
            return ADP.Argument(SR.GetString(Strings.SqlProvider_InvalidDataColumnMaxLength, columnName, maxLength));
        }

        internal static Exception InvalidColumnPrecScale()
        {
            return ADP.Argument(SR.GetString(Strings.SqlMisc_InvalidPrecScaleMessage));
        }

        internal static Exception NotEnoughColumnsInStructuredType()
        {
            return ADP.Argument(SR.GetString(Strings.SqlProvider_NotEnoughColumnsInStructuredType));
        }

        internal static Exception DuplicateSortOrdinal(int sortOrdinal)
        {
            return ADP.InvalidOperation(SR.GetString(Strings.SqlProvider_DuplicateSortOrdinal, sortOrdinal));
        }

        internal static Exception MissingSortOrdinal(int sortOrdinal)
        {
            return ADP.InvalidOperation(SR.GetString(Strings.SqlProvider_MissingSortOrdinal, sortOrdinal));
        }

        internal static Exception SortOrdinalGreaterThanFieldCount(int columnOrdinal, int sortOrdinal)
        {
            return ADP.InvalidOperation(SR.GetString(Strings.SqlProvider_SortOrdinalGreaterThanFieldCount, sortOrdinal, columnOrdinal));
        }

        internal static Exception IEnumerableOfSqlDataRecordHasNoRows()
        {
            return ADP.Argument(SR.GetString(Strings.IEnumerableOfSqlDataRecordHasNoRows));
        }


        //
        // SQL.BulkLoad
        //
        internal static Exception BulkLoadMappingInaccessible()
        {
            return ADP.InvalidOperation(SR.GetString(Strings.SQL_BulkLoadMappingInaccessible));
        }

        internal static Exception BulkLoadMappingsNamesOrOrdinalsOnly()
        {
            return ADP.InvalidOperation(SR.GetString(Strings.SQL_BulkLoadMappingsNamesOrOrdinalsOnly));
        }

        internal static Exception BulkLoadCannotConvertValue(Type sourcetype, string typeName, Exception e)
        {
            return ADP.InvalidOperation(SR.GetString(Strings.SQL_BulkLoadCannotConvertValue, sourcetype.Name, typeName), e);
        }

        internal static Exception BulkLoadNonMatchingColumnMapping()
        {
            return ADP.InvalidOperation(SR.GetString(Strings.SQL_BulkLoadNonMatchingColumnMapping));
        }

        internal static Exception BulkLoadNonMatchingColumnName(string columnName)
        {
            return BulkLoadNonMatchingColumnName(columnName, null);
        }

        internal static Exception BulkLoadNonMatchingColumnName(string columnName, Exception e)
        {
            return ADP.InvalidOperation(SR.GetString(Strings.SQL_BulkLoadNonMatchingColumnName, columnName), e);
        }

        internal static Exception BulkLoadStringTooLong()
        {
            return ADP.InvalidOperation(SR.GetString(Strings.SQL_BulkLoadStringTooLong));
        }

        internal static Exception BulkLoadInvalidVariantValue()
        {
            return ADP.InvalidOperation(SR.GetString(Strings.SQL_BulkLoadInvalidVariantValue));
        }

        internal static Exception BulkLoadInvalidTimeout(int timeout)
        {
            return ADP.Argument(SR.GetString(Strings.SQL_BulkLoadInvalidTimeout, timeout.ToString(CultureInfo.InvariantCulture)));
        }

        internal static Exception BulkLoadExistingTransaction()
        {
            return ADP.InvalidOperation(SR.GetString(Strings.SQL_BulkLoadExistingTransaction));
        }

        internal static Exception BulkLoadNoCollation()
        {
            return ADP.InvalidOperation(SR.GetString(Strings.SQL_BulkLoadNoCollation));
        }

        internal static Exception BulkLoadConflictingTransactionOption()
        {
            return ADP.Argument(SR.GetString(Strings.SQL_BulkLoadConflictingTransactionOption));
        }

        internal static Exception BulkLoadLcidMismatch(int sourceLcid, string sourceColumnName, int destinationLcid, string destinationColumnName)
        {
            return ADP.InvalidOperation(SR.GetString(Strings.Sql_BulkLoadLcidMismatch, sourceLcid, sourceColumnName, destinationLcid, destinationColumnName));
        }

        internal static Exception InvalidOperationInsideEvent()
        {
            return ADP.InvalidOperation(SR.GetString(Strings.SQL_BulkLoadInvalidOperationInsideEvent));
        }

        internal static Exception BulkLoadMissingDestinationTable()
        {
            return ADP.InvalidOperation(SR.GetString(Strings.SQL_BulkLoadMissingDestinationTable));
        }

        internal static Exception BulkLoadInvalidDestinationTable(string tableName, Exception inner)
        {
            return ADP.InvalidOperation(SR.GetString(Strings.SQL_BulkLoadInvalidDestinationTable, tableName), inner);
        }

        internal static Exception BulkLoadBulkLoadNotAllowDBNull(string columnName)
        {
            return ADP.InvalidOperation(SR.GetString(Strings.SQL_BulkLoadNotAllowDBNull, columnName));
        }

        internal static Exception BulkLoadPendingOperation()
        {
            return ADP.InvalidOperation(SR.GetString(Strings.SQL_BulkLoadPendingOperation));
        }

        internal static Exception InvalidTableDerivedPrecisionForTvp(string columnName, byte precision)
        {
            return ADP.InvalidOperation(SR.GetString(Strings.SqlParameter_InvalidTableDerivedPrecisionForTvp, precision, columnName, SqlDecimal.MaxPrecision));
        }

        //
        // transactions.
        //
        internal static Exception ConnectionDoomed()
        {
            return ADP.InvalidOperation(SR.GetString(Strings.SQL_ConnectionDoomed));
        }

        internal static Exception OpenResultCountExceeded()
        {
            return ADP.InvalidOperation(SR.GetString(Strings.SQL_OpenResultCountExceeded));
        }

        internal static Exception UnsupportedSysTxForGlobalTransactions()
        {
            return ADP.InvalidOperation(SR.GetString(Strings.SQL_UnsupportedSysTxVersion));
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

        internal static Exception ROR_FailoverNotSupportedServer(SqlInternalConnectionTds internalConnection)
        {
            var errors = new SqlErrorCollection();
            errors.Add(new SqlInfoAndError {Class = TdsEnums.FATAL_ERROR_CLASS, Message = SR.GetString(Strings.SQLROR_FailoverNotSupported)});
            var exc = SqlException.CreateException(errors, null);
            exc._doNotReconnect = true;
            return exc;
        }

        //
        // Connection resiliency
        //

        internal static Exception StreamWriteNotSupported()
        {
            return ADP.NotSupported(SR.GetString(Strings.SQL_StreamWriteNotSupported));
        }

        internal static Exception StreamReadNotSupported()
        {
            return ADP.NotSupported(SR.GetString(Strings.SQL_StreamReadNotSupported));
        }

        internal static Exception StreamSeekNotSupported()
        {
            return ADP.NotSupported(SR.GetString(Strings.SQL_StreamSeekNotSupported));
        }

        internal static SqlNullValueException SqlNullValue()
        {
            var e = new SqlNullValueException();
            return e;
        }

        internal static Exception SubclassMustOverride()
        {
            return ADP.InvalidOperation(SR.GetString(Strings.SqlMisc_SubclassMustOverride));
        }

        // ProjectK\CoreCLR specific errors
        internal static Exception UnsupportedKeyword(string keyword)
        {
            return ADP.NotSupported(SR.GetString(Strings.SQL_UnsupportedKeyword, keyword));
        }

        internal static Exception NetworkLibraryKeywordNotSupported()
        {
            return ADP.NotSupported(SR.GetString(Strings.SQL_NetworkLibraryNotSupported));
        }

        internal static Exception UnsupportedFeatureAndToken(SqlInternalConnectionTds internalConnection, string token)
        {
            var innerException = ADP.NotSupported(SR.GetString(Strings.SQL_UnsupportedToken, token));

            var errors = new SqlErrorCollection();
            errors.Add(new SqlInfoAndError {Class = TdsEnums.FATAL_ERROR_CLASS, Message = SR.GetString(Strings.SQL_UnsupportedFeature)});
            var exc = SqlException.CreateException(errors, "");
            return exc;
        }

        internal static Exception BatchedUpdatesNotAvailableOnContextConnection()
        {
            return ADP.InvalidOperation(SR.GetString(Strings.SQL_BatchedUpdatesNotAvailableOnContextConnection));
        }

        /// <summary>
        ///     gets a message for SNI error (Error must be valid, non-zero error code)
        /// </summary>
        internal static string GetSNIErrorMessage(int sniError)
        {
            Debug.Assert(sniError > 0 && sniError <= (int) SniNativeMethodWrapper.SniSpecialErrors.MaxErrorValue, "SNI error is out of range");

            var errorMessageId = string.Format(null, "SNI_ERROR_{0}", sniError);
            return SR.GetResourceString(errorMessageId, errorMessageId);
        }
    }

    internal class SqlInternalConnectionTds
    {
        public Guid OriginalClientConnectionId { get; set; }
        public string RoutingDestination { get; set; }
    }
}