using System.Resources;
using System.Runtime.CompilerServices;
using Medella.TdsClient.System2;

namespace System
{
    internal static partial class SR
    {
        internal static string GetString(string value)
        {
            return value;
        }

        internal static string GetString(string format, params object[] args)
        {
            return Format(format, args);
        }
    }

    internal partial class SR
    {
        private static ResourceManager s_resourceManager;

        private static ResourceManager ResourceManager => s_resourceManager ??
                                                          (s_resourceManager = new ResourceManager("SqlClient.Strings.resources", typeof(SR).Assembly));

        // This method is used to decide if we need to append the exception message parameters to the message when calling SR.Format.
        // by default it returns false.
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static bool UsingResourceKeys()
        {
            return false;
        }

        internal static string GetResourceString(string resourceKey, string defaultString)
        {
            string resourceString = null;
            try
            {
                resourceString = Strings.ResourceManager.GetString(resourceKey);
            }
            catch (MissingManifestResourceException)
            {
            }

            if (defaultString != null && resourceKey.Equals(resourceString, StringComparison.Ordinal)) return defaultString;

            return resourceString;
        }

        internal static string Format(string resourceFormat, params object[] args)
        {
            if (args != null)
            {
                if (UsingResourceKeys()) return resourceFormat + string.Join(", ", args);

                return string.Format(resourceFormat, args);
            }

            return resourceFormat;
        }

        internal static string Format(string resourceFormat, object p1)
        {
            if (UsingResourceKeys()) return string.Join(", ", resourceFormat, p1);

            return string.Format(resourceFormat, p1);
        }

        internal static string Format(string resourceFormat, object p1, object p2)
        {
            if (UsingResourceKeys()) return string.Join(", ", resourceFormat, p1, p2);

            return string.Format(resourceFormat, p1, p2);
        }

        internal static string Format(string resourceFormat, object p1, object p2, object p3)
        {
            if (UsingResourceKeys()) return string.Join(", ", resourceFormat, p1, p2, p3);

            return string.Format(resourceFormat, p1, p2, p3);
        }
    }

    internal static partial class SR
    {
#pragma warning disable 0414
        private const string s_resourcesName = "FxResources.System.Data.SqlClient.SR";
#pragma warning restore 0414

#if !DEBUGRESOURCES
        internal static string ADP_CollectionIndexInt32 => GetResourceString("ADP_CollectionIndexInt32", null);

        internal static string ADP_CollectionIndexString => GetResourceString("ADP_CollectionIndexString", null);

        internal static string ADP_CollectionInvalidType => GetResourceString("ADP_CollectionInvalidType", null);

        internal static string ADP_CollectionIsNotParent => GetResourceString("ADP_CollectionIsNotParent", null);

        internal static string ADP_CollectionNullValue => GetResourceString("ADP_CollectionNullValue", null);

        internal static string ADP_CollectionRemoveInvalidObject => GetResourceString("ADP_CollectionRemoveInvalidObject", null);

        internal static string ADP_ConnectionAlreadyOpen => GetResourceString("ADP_ConnectionAlreadyOpen", null);

        internal static string ADP_ConnectionStateMsg_Closed => GetResourceString("ADP_ConnectionStateMsg_Closed", null);

        internal static string ADP_ConnectionStateMsg_Connecting => GetResourceString("ADP_ConnectionStateMsg_Connecting", null);

        internal static string ADP_ConnectionStateMsg_Open => GetResourceString("ADP_ConnectionStateMsg_Open", null);

        internal static string ADP_ConnectionStateMsg_OpenExecuting => GetResourceString("ADP_ConnectionStateMsg_OpenExecuting", null);

        internal static string ADP_ConnectionStateMsg_OpenFetching => GetResourceString("ADP_ConnectionStateMsg_OpenFetching", null);

        internal static string ADP_ConnectionStateMsg => GetResourceString("ADP_ConnectionStateMsg", null);

        internal static string ADP_ConnectionStringSyntax => GetResourceString("ADP_ConnectionStringSyntax", null);

        internal static string ADP_DataReaderClosed => GetResourceString("ADP_DataReaderClosed", null);

        internal static string ADP_InternalConnectionError => GetResourceString("ADP_InternalConnectionError", null);

        internal static string ADP_InvalidEnumerationValue => GetResourceString("ADP_InvalidEnumerationValue", null);

        internal static string ADP_NotSupportedEnumerationValue => GetResourceString("ADP_NotSupportedEnumerationValue", null);

        internal static string ADP_InvalidOffsetValue => GetResourceString("ADP_InvalidOffsetValue", null);

        internal static string ADP_TransactionPresent => GetResourceString("ADP_TransactionPresent", null);

        internal static string ADP_LocalTransactionPresent => GetResourceString("ADP_LocalTransactionPresent", null);

        internal static string ADP_NoConnectionString => GetResourceString("ADP_NoConnectionString", null);

        internal static string ADP_OpenConnectionPropertySet => GetResourceString("ADP_OpenConnectionPropertySet", null);

        internal static string ADP_PendingAsyncOperation => GetResourceString("ADP_PendingAsyncOperation", null);

        internal static string ADP_PooledOpenTimeout => GetResourceString("ADP_PooledOpenTimeout", null);

        internal static string ADP_NonPooledOpenTimeout => GetResourceString("ADP_NonPooledOpenTimeout", null);

        internal static string ADP_SingleValuedProperty => GetResourceString("ADP_SingleValuedProperty", null);

        internal static string ADP_DoubleValuedProperty => GetResourceString("ADP_DoubleValuedProperty", null);

        internal static string ADP_InvalidPrefixSuffix => GetResourceString("ADP_InvalidPrefixSuffix", null);

        internal static string Arg_ArrayPlusOffTooSmall => GetResourceString("Arg_ArrayPlusOffTooSmall", null);

        internal static string Arg_RankMultiDimNotSupported => GetResourceString("Arg_RankMultiDimNotSupported", null);

        internal static string Arg_RemoveArgNotFound => GetResourceString("Arg_RemoveArgNotFound", null);

        internal static string ArgumentOutOfRange_NeedNonNegNum => GetResourceString("ArgumentOutOfRange_NeedNonNegNum", null);

        internal static string Data_InvalidOffsetLength => GetResourceString("Data_InvalidOffsetLength", null);

        internal static string SqlConvert_ConvertFailed => GetResourceString("SqlConvert_ConvertFailed", null);

        internal static string SQL_WrongType => GetResourceString("SQL_WrongType", null);

        internal static string ADP_DeriveParametersNotSupported => GetResourceString("ADP_DeriveParametersNotSupported", null);

        internal static string ADP_NoStoredProcedureExists => GetResourceString("ADP_NoStoredProcedureExists", null);

        internal static string ADP_InvalidConnectionOptionValue => GetResourceString("ADP_InvalidConnectionOptionValue", null);

        internal static string ADP_MissingConnectionOptionValue => GetResourceString("ADP_MissingConnectionOptionValue", null);

        internal static string ADP_InvalidConnectionOptionValueLength => GetResourceString("ADP_InvalidConnectionOptionValueLength", null);

        internal static string ADP_KeywordNotSupported => GetResourceString("ADP_KeywordNotSupported", null);

        internal static string ADP_InternalProviderError => GetResourceString("ADP_InternalProviderError", null);

        internal static string ADP_InvalidMultipartName => GetResourceString("ADP_InvalidMultipartName", null);

        internal static string ADP_InvalidMultipartNameQuoteUsage => GetResourceString("ADP_InvalidMultipartNameQuoteUsage", null);

        internal static string ADP_InvalidMultipartNameToManyParts => GetResourceString("ADP_InvalidMultipartNameToManyParts", null);

        internal static string SQL_SqlCommandCommandText => GetResourceString("SQL_SqlCommandCommandText", null);

        internal static string SQL_BatchedUpdatesNotAvailableOnContextConnection => GetResourceString("SQL_BatchedUpdatesNotAvailableOnContextConnection", null);

        internal static string SQL_BulkCopyDestinationTableName => GetResourceString("SQL_BulkCopyDestinationTableName", null);

        internal static string SQL_TDSParserTableName => GetResourceString("SQL_TDSParserTableName", null);

        internal static string SQL_TypeName => GetResourceString("SQL_TypeName", null);

        internal static string SQLMSF_FailoverPartnerNotSupported => GetResourceString("SQLMSF_FailoverPartnerNotSupported", null);

        internal static string SQL_NotSupportedEnumerationValue => GetResourceString("SQL_NotSupportedEnumerationValue", null);

        internal static string ADP_CommandTextRequired => GetResourceString("ADP_CommandTextRequired", null);

        internal static string ADP_ConnectionRequired => GetResourceString("ADP_ConnectionRequired", null);

        internal static string ADP_OpenConnectionRequired => GetResourceString("ADP_OpenConnectionRequired", null);

        internal static string ADP_TransactionConnectionMismatch => GetResourceString("ADP_TransactionConnectionMismatch", null);

        internal static string ADP_TransactionRequired => GetResourceString("ADP_TransactionRequired", null);

        internal static string ADP_OpenReaderExists => GetResourceString("ADP_OpenReaderExists", null);

        internal static string ADP_CalledTwice => GetResourceString("ADP_CalledTwice", null);

        internal static string ADP_InvalidCommandTimeout => GetResourceString("ADP_InvalidCommandTimeout", null);

        internal static string ADP_UninitializedParameterSize => GetResourceString("ADP_UninitializedParameterSize", null);

        internal static string ADP_PrepareParameterType => GetResourceString("ADP_PrepareParameterType", null);

        internal static string ADP_PrepareParameterSize => GetResourceString("ADP_PrepareParameterSize", null);

        internal static string ADP_PrepareParameterScale => GetResourceString("ADP_PrepareParameterScale", null);

        internal static string ADP_MismatchedAsyncResult => GetResourceString("ADP_MismatchedAsyncResult", null);

        internal static string ADP_ClosedConnectionError => GetResourceString("ADP_ClosedConnectionError", null);

        internal static string ADP_ConnectionIsDisabled => GetResourceString("ADP_ConnectionIsDisabled", null);

        internal static string ADP_EmptyDatabaseName => GetResourceString("ADP_EmptyDatabaseName", null);

        internal static string ADP_InvalidSourceBufferIndex => GetResourceString("ADP_InvalidSourceBufferIndex", null);

        internal static string ADP_InvalidDestinationBufferIndex => GetResourceString("ADP_InvalidDestinationBufferIndex", null);

        internal static string ADP_StreamClosed => GetResourceString("ADP_StreamClosed", null);

        internal static string ADP_InvalidSeekOrigin => GetResourceString("ADP_InvalidSeekOrigin", null);

        internal static string ADP_NonSequentialColumnAccess => GetResourceString("ADP_NonSequentialColumnAccess", null);

        internal static string ADP_InvalidDataType => GetResourceString("ADP_InvalidDataType", null);

        internal static string ADP_UnknownDataType => GetResourceString("ADP_UnknownDataType", null);

        internal static string ADP_UnknownDataTypeCode => GetResourceString("ADP_UnknownDataTypeCode", null);

        internal static string ADP_DbTypeNotSupported => GetResourceString("ADP_DbTypeNotSupported", null);

        internal static string ADP_VersionDoesNotSupportDataType => GetResourceString("ADP_VersionDoesNotSupportDataType", null);

        internal static string ADP_ParameterValueOutOfRange => GetResourceString("ADP_ParameterValueOutOfRange", null);

        internal static string ADP_BadParameterName => GetResourceString("ADP_BadParameterName", null);

        internal static string ADP_InvalidSizeValue => GetResourceString("ADP_InvalidSizeValue", null);

        internal static string ADP_NegativeParameter => GetResourceString("ADP_NegativeParameter", null);

        internal static string ADP_InvalidMetaDataValue => GetResourceString("ADP_InvalidMetaDataValue", null);

        internal static string ADP_ParameterConversionFailed => GetResourceString("ADP_ParameterConversionFailed", null);

        internal static string ADP_ParallelTransactionsNotSupported => GetResourceString("ADP_ParallelTransactionsNotSupported", null);

        internal static string ADP_TransactionZombied => GetResourceString("ADP_TransactionZombied", null);

        internal static string ADP_InvalidDataLength2 => GetResourceString("ADP_InvalidDataLength2", null);

        internal static string ADP_NonSeqByteAccess => GetResourceString("ADP_NonSeqByteAccess", null);

        internal static string ADP_InvalidMinMaxPoolSizeValues => GetResourceString("ADP_InvalidMinMaxPoolSizeValues", null);

        internal static string SQL_InvalidPacketSizeValue => GetResourceString("SQL_InvalidPacketSizeValue", null);

        internal static string SQL_NullEmptyTransactionName => GetResourceString("SQL_NullEmptyTransactionName", null);

        internal static string SQL_UserInstanceFailoverNotCompatible => GetResourceString("SQL_UserInstanceFailoverNotCompatible", null);

        internal static string SQL_EncryptionNotSupportedByClient => GetResourceString("SQL_EncryptionNotSupportedByClient", null);

        internal static string SQL_EncryptionNotSupportedByServer => GetResourceString("SQL_EncryptionNotSupportedByServer", null);

        internal static string SQL_InvalidSQLServerVersionUnknown => GetResourceString("SQL_InvalidSQLServerVersionUnknown", null);

        internal static string SQL_CannotCreateNormalizer => GetResourceString("SQL_CannotCreateNormalizer", null);

        internal static string SQL_CannotModifyPropertyAsyncOperationInProgress => GetResourceString("SQL_CannotModifyPropertyAsyncOperationInProgress", null);

        internal static string SQL_InstanceFailure => GetResourceString("SQL_InstanceFailure", null);

        internal static string SQL_InvalidPartnerConfiguration => GetResourceString("SQL_InvalidPartnerConfiguration", null);

        internal static string SQL_MarsUnsupportedOnConnection => GetResourceString("SQL_MarsUnsupportedOnConnection", null);

        internal static string SQL_NonLocalSSEInstance => GetResourceString("SQL_NonLocalSSEInstance", null);

        internal static string SQL_PendingBeginXXXExists => GetResourceString("SQL_PendingBeginXXXExists", null);

        internal static string SQL_NonXmlResult => GetResourceString("SQL_NonXmlResult", null);

        internal static string SQL_InvalidParameterTypeNameFormat => GetResourceString("SQL_InvalidParameterTypeNameFormat", null);

        internal static string SQL_InvalidParameterNameLength => GetResourceString("SQL_InvalidParameterNameLength", null);

        internal static string SQL_PrecisionValueOutOfRange => GetResourceString("SQL_PrecisionValueOutOfRange", null);

        internal static string SQL_ScaleValueOutOfRange => GetResourceString("SQL_ScaleValueOutOfRange", null);

        internal static string SQL_TimeScaleValueOutOfRange => GetResourceString("SQL_TimeScaleValueOutOfRange", null);

        internal static string SQL_ParameterInvalidVariant => GetResourceString("SQL_ParameterInvalidVariant", null);

        internal static string SQL_ParameterTypeNameRequired => GetResourceString("SQL_ParameterTypeNameRequired", null);

        internal static string SQL_InvalidInternalPacketSize => GetResourceString("SQL_InvalidInternalPacketSize", null);

        internal static string SQL_InvalidTDSVersion => GetResourceString("SQL_InvalidTDSVersion", null);

        internal static string SQL_InvalidTDSPacketSize => GetResourceString("SQL_InvalidTDSPacketSize", null);

        internal static string SQL_ParsingError => GetResourceString("SQL_ParsingError", null);

        internal static string SQL_ConnectionLockedForBcpEvent => GetResourceString("SQL_ConnectionLockedForBcpEvent", null);

        internal static string SQL_SNIPacketAllocationFailure => GetResourceString("SQL_SNIPacketAllocationFailure", null);

        internal static string SQL_SmallDateTimeOverflow => GetResourceString("SQL_SmallDateTimeOverflow", null);

        internal static string SQL_TimeOverflow => GetResourceString("SQL_TimeOverflow", null);

        internal static string SQL_MoneyOverflow => GetResourceString("SQL_MoneyOverflow", null);

        internal static string SQL_CultureIdError => GetResourceString("SQL_CultureIdError", null);

        internal static string SQL_OperationCancelled => GetResourceString("SQL_OperationCancelled", null);

        internal static string SQL_SevereError => GetResourceString("SQL_SevereError", null);

        internal static string SQL_SSPIGenerateError => GetResourceString("SQL_SSPIGenerateError", null);

        internal static string SQL_KerberosTicketMissingError => GetResourceString("SQL_KerberosTicketMissingError", null);

        internal static string SQL_SqlServerBrowserNotAccessible => GetResourceString("SQL_SqlServerBrowserNotAccessible", null);

        internal static string SQL_InvalidSSPIPacketSize => GetResourceString("SQL_InvalidSSPIPacketSize", null);

        internal static string SQL_SSPIInitializeError => GetResourceString("SQL_SSPIInitializeError", null);

        internal static string SQL_Timeout => GetResourceString("SQL_Timeout", null);

        internal static string SQL_Timeout_PreLogin_Begin => GetResourceString("SQL_Timeout_PreLogin_Begin", null);

        internal static string SQL_Timeout_PreLogin_InitializeConnection => GetResourceString("SQL_Timeout_PreLogin_InitializeConnection", null);

        internal static string SQL_Timeout_PreLogin_SendHandshake => GetResourceString("SQL_Timeout_PreLogin_SendHandshake", null);

        internal static string SQL_Timeout_PreLogin_ConsumeHandshake => GetResourceString("SQL_Timeout_PreLogin_ConsumeHandshake", null);

        internal static string SQL_Timeout_Login_Begin => GetResourceString("SQL_Timeout_Login_Begin", null);

        internal static string SQL_Timeout_Login_ProcessConnectionAuth => GetResourceString("SQL_Timeout_Login_ProcessConnectionAuth", null);

        internal static string SQL_Timeout_PostLogin => GetResourceString("SQL_Timeout_PostLogin", null);

        internal static string SQL_Timeout_FailoverInfo => GetResourceString("SQL_Timeout_FailoverInfo", null);

        internal static string SQL_Timeout_RoutingDestinationInfo => GetResourceString("SQL_Timeout_RoutingDestinationInfo", null);

        internal static string SQL_Duration_PreLogin_Begin => GetResourceString("SQL_Duration_PreLogin_Begin", null);

        internal static string SQL_Duration_PreLoginHandshake => GetResourceString("SQL_Duration_PreLoginHandshake", null);

        internal static string SQL_Duration_Login_Begin => GetResourceString("SQL_Duration_Login_Begin", null);

        internal static string SQL_Duration_Login_ProcessConnectionAuth => GetResourceString("SQL_Duration_Login_ProcessConnectionAuth", null);

        internal static string SQL_Duration_PostLogin => GetResourceString("SQL_Duration_PostLogin", null);

        internal static string SQL_UserInstanceFailure => GetResourceString("SQL_UserInstanceFailure", null);

        internal static string SQL_InvalidRead => GetResourceString("SQL_InvalidRead", null);

        internal static string SQL_NonBlobColumn => GetResourceString("SQL_NonBlobColumn", null);

        internal static string SQL_NonCharColumn => GetResourceString("SQL_NonCharColumn", null);

        internal static string SQL_StreamNotSupportOnColumnType => GetResourceString("SQL_StreamNotSupportOnColumnType", null);

        internal static string SQL_TextReaderNotSupportOnColumnType => GetResourceString("SQL_TextReaderNotSupportOnColumnType", null);

        internal static string SQL_XmlReaderNotSupportOnColumnType => GetResourceString("SQL_XmlReaderNotSupportOnColumnType", null);

        internal static string SqlDelegatedTransaction_PromotionFailed => GetResourceString("SqlDelegatedTransaction_PromotionFailed", null);

        internal static string SQL_InvalidBufferSizeOrIndex => GetResourceString("SQL_InvalidBufferSizeOrIndex", null);

        internal static string SQL_InvalidDataLength => GetResourceString("SQL_InvalidDataLength", null);

        internal static string SQL_BulkLoadMappingInaccessible => GetResourceString("SQL_BulkLoadMappingInaccessible", null);

        internal static string SQL_BulkLoadMappingsNamesOrOrdinalsOnly => GetResourceString("SQL_BulkLoadMappingsNamesOrOrdinalsOnly", null);

        internal static string SQL_BulkLoadCannotConvertValue => GetResourceString("SQL_BulkLoadCannotConvertValue", null);

        internal static string SQL_BulkLoadNonMatchingColumnMapping => GetResourceString("SQL_BulkLoadNonMatchingColumnMapping", null);

        internal static string SQL_BulkLoadNonMatchingColumnName => GetResourceString("SQL_BulkLoadNonMatchingColumnName", null);

        internal static string SQL_BulkLoadStringTooLong => GetResourceString("SQL_BulkLoadStringTooLong", null);

        internal static string SQL_BulkLoadInvalidTimeout => GetResourceString("SQL_BulkLoadInvalidTimeout", null);

        internal static string SQL_BulkLoadInvalidVariantValue => GetResourceString("SQL_BulkLoadInvalidVariantValue", null);

        internal static string SQL_BulkLoadExistingTransaction => GetResourceString("SQL_BulkLoadExistingTransaction", null);

        internal static string SQL_BulkLoadNoCollation => GetResourceString("SQL_BulkLoadNoCollation", null);

        internal static string SQL_BulkLoadConflictingTransactionOption => GetResourceString("SQL_BulkLoadConflictingTransactionOption", null);

        internal static string SQL_BulkLoadInvalidOperationInsideEvent => GetResourceString("SQL_BulkLoadInvalidOperationInsideEvent", null);

        internal static string SQL_BulkLoadMissingDestinationTable => GetResourceString("SQL_BulkLoadMissingDestinationTable", null);

        internal static string SQL_BulkLoadInvalidDestinationTable => GetResourceString("SQL_BulkLoadInvalidDestinationTable", null);

        internal static string SQL_BulkLoadNotAllowDBNull => GetResourceString("SQL_BulkLoadNotAllowDBNull", null);

        internal static string Sql_BulkLoadLcidMismatch => GetResourceString("Sql_BulkLoadLcidMismatch", null);

        internal static string SQL_BulkLoadPendingOperation => GetResourceString("SQL_BulkLoadPendingOperation", null);

        internal static string SQL_CannotGetDTCAddress => GetResourceString("SQL_CannotGetDTCAddress", null);

        internal static string SQL_ConnectionDoomed => GetResourceString("SQL_ConnectionDoomed", null);

        internal static string SQL_OpenResultCountExceeded => GetResourceString("SQL_OpenResultCountExceeded", null);

        internal static string SQL_StreamWriteNotSupported => GetResourceString("SQL_StreamWriteNotSupported", null);

        internal static string SQL_StreamReadNotSupported => GetResourceString("SQL_StreamReadNotSupported", null);

        internal static string SQL_StreamSeekNotSupported => GetResourceString("SQL_StreamSeekNotSupported", null);

        internal static string SQL_ExClientConnectionId => GetResourceString("SQL_ExClientConnectionId", null);

        internal static string SQL_ExErrorNumberStateClass => GetResourceString("SQL_ExErrorNumberStateClass", null);

        internal static string SQL_ExOriginalClientConnectionId => GetResourceString("SQL_ExOriginalClientConnectionId", null);

        internal static string SQL_ExRoutingDestination => GetResourceString("SQL_ExRoutingDestination", null);

        internal static string SQL_UnsupportedSysTxVersion => GetResourceString("SQL_UnsupportedSysTxVersion", null);

        internal static string SqlMisc_NullString => GetResourceString("SqlMisc_NullString", null);

        internal static string SqlMisc_MessageString => GetResourceString("SqlMisc_MessageString", null);

        internal static string SqlMisc_ArithOverflowMessage => GetResourceString("SqlMisc_ArithOverflowMessage", null);

        internal static string SqlMisc_DivideByZeroMessage => GetResourceString("SqlMisc_DivideByZeroMessage", null);

        internal static string SqlMisc_NullValueMessage => GetResourceString("SqlMisc_NullValueMessage", null);

        internal static string SqlMisc_TruncationMessage => GetResourceString("SqlMisc_TruncationMessage", null);

        internal static string SqlMisc_DateTimeOverflowMessage => GetResourceString("SqlMisc_DateTimeOverflowMessage", null);

        internal static string SqlMisc_ConcatDiffCollationMessage => GetResourceString("SqlMisc_ConcatDiffCollationMessage", null);

        internal static string SqlMisc_CompareDiffCollationMessage => GetResourceString("SqlMisc_CompareDiffCollationMessage", null);

        internal static string SqlMisc_InvalidFlagMessage => GetResourceString("SqlMisc_InvalidFlagMessage", null);

        internal static string SqlMisc_NumeToDecOverflowMessage => GetResourceString("SqlMisc_NumeToDecOverflowMessage", null);

        internal static string SqlMisc_ConversionOverflowMessage => GetResourceString("SqlMisc_ConversionOverflowMessage", null);

        internal static string SqlMisc_InvalidDateTimeMessage => GetResourceString("SqlMisc_InvalidDateTimeMessage", null);

        internal static string SqlMisc_TimeZoneSpecifiedMessage => GetResourceString("SqlMisc_TimeZoneSpecifiedMessage", null);

        internal static string SqlMisc_InvalidArraySizeMessage => GetResourceString("SqlMisc_InvalidArraySizeMessage", null);

        internal static string SqlMisc_InvalidPrecScaleMessage => GetResourceString("SqlMisc_InvalidPrecScaleMessage", null);

        internal static string SqlMisc_FormatMessage => GetResourceString("SqlMisc_FormatMessage", null);

        internal static string SqlMisc_StreamErrorMessage => GetResourceString("SqlMisc_StreamErrorMessage", null);

        internal static string SqlMisc_TruncationMaxDataMessage => GetResourceString("SqlMisc_TruncationMaxDataMessage", null);

        internal static string SqlMisc_NotFilledMessage => GetResourceString("SqlMisc_NotFilledMessage", null);

        internal static string SqlMisc_AlreadyFilledMessage => GetResourceString("SqlMisc_AlreadyFilledMessage", null);

        internal static string SqlMisc_ClosedXmlReaderMessage => GetResourceString("SqlMisc_ClosedXmlReaderMessage", null);

        internal static string SqlMisc_InvalidOpStreamClosed => GetResourceString("SqlMisc_InvalidOpStreamClosed", null);

        internal static string SqlMisc_InvalidOpStreamNonWritable => GetResourceString("SqlMisc_InvalidOpStreamNonWritable", null);

        internal static string SqlMisc_InvalidOpStreamNonReadable => GetResourceString("SqlMisc_InvalidOpStreamNonReadable", null);

        internal static string SqlMisc_InvalidOpStreamNonSeekable => GetResourceString("SqlMisc_InvalidOpStreamNonSeekable", null);

        internal static string SqlMisc_SubclassMustOverride => GetResourceString("SqlMisc_SubclassMustOverride", null);

        internal static string SqlUdtReason_NoUdtAttribute => GetResourceString("SqlUdtReason_NoUdtAttribute", null);

        internal static string SQLUDT_InvalidSqlType => GetResourceString("SQLUDT_InvalidSqlType", null);

        internal static string Sql_InternalError => GetResourceString("Sql_InternalError", null);

        internal static string ADP_OperationAborted => GetResourceString("ADP_OperationAborted", null);

        internal static string ADP_OperationAbortedExceptionMessage => GetResourceString("ADP_OperationAbortedExceptionMessage", null);

        internal static string ADP_TransactionCompletedButNotDisposed => GetResourceString("ADP_TransactionCompletedButNotDisposed", null);

        internal static string SqlParameter_UnsupportedTVPOutputParameter => GetResourceString("SqlParameter_UnsupportedTVPOutputParameter", null);

        internal static string SqlParameter_DBNullNotSupportedForTVP => GetResourceString("SqlParameter_DBNullNotSupportedForTVP", null);

        internal static string SqlParameter_UnexpectedTypeNameForNonStruct => GetResourceString("SqlParameter_UnexpectedTypeNameForNonStruct", null);

        internal static string NullSchemaTableDataTypeNotSupported => GetResourceString("NullSchemaTableDataTypeNotSupported", null);

        internal static string InvalidSchemaTableOrdinals => GetResourceString("InvalidSchemaTableOrdinals", null);

        internal static string SQL_EnumeratedRecordMetaDataChanged => GetResourceString("SQL_EnumeratedRecordMetaDataChanged", null);

        internal static string SQL_EnumeratedRecordFieldCountChanged => GetResourceString("SQL_EnumeratedRecordFieldCountChanged", null);

        internal static string GT_Disabled => GetResourceString("GT_Disabled", null);

        internal static string SQL_UnknownSysTxIsolationLevel => GetResourceString("SQL_UnknownSysTxIsolationLevel", null);

        internal static string SQLNotify_AlreadyHasCommand => GetResourceString("SQLNotify_AlreadyHasCommand", null);

        internal static string SqlDependency_DatabaseBrokerDisabled => GetResourceString("SqlDependency_DatabaseBrokerDisabled", null);

        internal static string SqlDependency_DefaultOptionsButNoStart => GetResourceString("SqlDependency_DefaultOptionsButNoStart", null);

        internal static string SqlDependency_NoMatchingServerStart => GetResourceString("SqlDependency_NoMatchingServerStart", null);

        internal static string SqlDependency_NoMatchingServerDatabaseStart => GetResourceString("SqlDependency_NoMatchingServerDatabaseStart", null);

        internal static string SqlDependency_EventNoDuplicate => GetResourceString("SqlDependency_EventNoDuplicate", null);

        internal static string SqlDependency_IdMismatch => GetResourceString("SqlDependency_IdMismatch", null);

        internal static string SqlDependency_InvalidTimeout => GetResourceString("SqlDependency_InvalidTimeout", null);

        internal static string SqlDependency_DuplicateStart => GetResourceString("SqlDependency_DuplicateStart", null);

        internal static string SqlMetaData_InvalidSqlDbTypeForConstructorFormat => GetResourceString("SqlMetaData_InvalidSqlDbTypeForConstructorFormat", null);

        internal static string SqlMetaData_NameTooLong => GetResourceString("SqlMetaData_NameTooLong", null);

        internal static string SqlMetaData_SpecifyBothSortOrderAndOrdinal => GetResourceString("SqlMetaData_SpecifyBothSortOrderAndOrdinal", null);

        internal static string SqlProvider_InvalidDataColumnType => GetResourceString("SqlProvider_InvalidDataColumnType", null);

        internal static string SqlProvider_NotEnoughColumnsInStructuredType => GetResourceString("SqlProvider_NotEnoughColumnsInStructuredType", null);

        internal static string SqlProvider_DuplicateSortOrdinal => GetResourceString("SqlProvider_DuplicateSortOrdinal", null);

        internal static string SqlProvider_MissingSortOrdinal => GetResourceString("SqlProvider_MissingSortOrdinal", null);

        internal static string SqlProvider_SortOrdinalGreaterThanFieldCount => GetResourceString("SqlProvider_SortOrdinalGreaterThanFieldCount", null);

        internal static string SQLUDT_MaxByteSizeValue => GetResourceString("SQLUDT_MaxByteSizeValue", null);

        internal static string SQLUDT_Unexpected => GetResourceString("SQLUDT_Unexpected", null);

        internal static string SQLUDT_UnexpectedUdtTypeName => GetResourceString("SQLUDT_UnexpectedUdtTypeName", null);

        internal static string SQLUDT_InvalidUdtTypeName => GetResourceString("SQLUDT_InvalidUdtTypeName", null);

        internal static string SqlUdt_InvalidUdtMessage => GetResourceString("SqlUdt_InvalidUdtMessage", null);

        internal static string SQL_UDTTypeName => GetResourceString("SQL_UDTTypeName", null);

        internal static string SQL_InvalidUdt3PartNameFormat => GetResourceString("SQL_InvalidUdt3PartNameFormat", null);

        internal static string IEnumerableOfSqlDataRecordHasNoRows => GetResourceString("IEnumerableOfSqlDataRecordHasNoRows", null);

        internal static string SNI_ERROR_1 => GetResourceString("SNI_ERROR_1", null);

        internal static string SNI_ERROR_2 => GetResourceString("SNI_ERROR_2", null);

        internal static string SNI_ERROR_3 => GetResourceString("SNI_ERROR_3", null);

        internal static string SNI_ERROR_5 => GetResourceString("SNI_ERROR_5", null);

        internal static string SNI_ERROR_6 => GetResourceString("SNI_ERROR_6", null);

        internal static string SNI_ERROR_7 => GetResourceString("SNI_ERROR_7", null);

        internal static string SNI_ERROR_8 => GetResourceString("SNI_ERROR_8", null);

        internal static string SNI_ERROR_9 => GetResourceString("SNI_ERROR_9", null);

        internal static string SNI_ERROR_11 => GetResourceString("SNI_ERROR_11", null);

        internal static string SNI_ERROR_12 => GetResourceString("SNI_ERROR_12", null);

        internal static string SNI_ERROR_13 => GetResourceString("SNI_ERROR_13", null);

        internal static string SNI_ERROR_14 => GetResourceString("SNI_ERROR_14", null);

        internal static string SNI_ERROR_15 => GetResourceString("SNI_ERROR_15", null);

        internal static string SNI_ERROR_16 => GetResourceString("SNI_ERROR_16", null);

        internal static string SNI_ERROR_17 => GetResourceString("SNI_ERROR_17", null);

        internal static string SNI_ERROR_18 => GetResourceString("SNI_ERROR_18", null);

        internal static string SNI_ERROR_19 => GetResourceString("SNI_ERROR_19", null);

        internal static string SNI_ERROR_20 => GetResourceString("SNI_ERROR_20", null);

        internal static string SNI_ERROR_21 => GetResourceString("SNI_ERROR_21", null);

        internal static string SNI_ERROR_22 => GetResourceString("SNI_ERROR_22", null);

        internal static string SNI_ERROR_23 => GetResourceString("SNI_ERROR_23", null);

        internal static string SNI_ERROR_24 => GetResourceString("SNI_ERROR_24", null);

        internal static string SNI_ERROR_25 => GetResourceString("SNI_ERROR_25", null);

        internal static string SNI_ERROR_26 => GetResourceString("SNI_ERROR_26", null);

        internal static string SNI_ERROR_27 => GetResourceString("SNI_ERROR_27", null);

        internal static string SNI_ERROR_28 => GetResourceString("SNI_ERROR_28", null);

        internal static string SNI_ERROR_29 => GetResourceString("SNI_ERROR_29", null);

        internal static string SNI_ERROR_30 => GetResourceString("SNI_ERROR_30", null);

        internal static string SNI_ERROR_31 => GetResourceString("SNI_ERROR_31", null);

        internal static string SNI_ERROR_32 => GetResourceString("SNI_ERROR_32", null);

        internal static string SNI_ERROR_33 => GetResourceString("SNI_ERROR_33", null);

        internal static string SNI_ERROR_34 => GetResourceString("SNI_ERROR_34", null);

        internal static string SNI_ERROR_35 => GetResourceString("SNI_ERROR_35", null);

        internal static string SNI_ERROR_36 => GetResourceString("SNI_ERROR_36", null);

        internal static string SNI_ERROR_37 => GetResourceString("SNI_ERROR_37", null);

        internal static string SNI_ERROR_38 => GetResourceString("SNI_ERROR_38", null);

        internal static string SNI_ERROR_39 => GetResourceString("SNI_ERROR_39", null);

        internal static string SNI_ERROR_40 => GetResourceString("SNI_ERROR_40", null);

        internal static string SNI_ERROR_41 => GetResourceString("SNI_ERROR_41", null);

        internal static string SNI_ERROR_42 => GetResourceString("SNI_ERROR_42", null);

        internal static string SNI_ERROR_43 => GetResourceString("SNI_ERROR_43", null);

        internal static string SNI_ERROR_44 => GetResourceString("SNI_ERROR_44", null);

        internal static string SNI_ERROR_47 => GetResourceString("SNI_ERROR_47", null);

        internal static string SNI_ERROR_48 => GetResourceString("SNI_ERROR_48", null);

        internal static string SNI_ERROR_49 => GetResourceString("SNI_ERROR_49", null);

        internal static string SNI_ERROR_50 => GetResourceString("SNI_ERROR_50", null);

        internal static string SNI_ERROR_51 => GetResourceString("SNI_ERROR_51", null);

        internal static string SNI_ERROR_52 => GetResourceString("SNI_ERROR_52", null);

        internal static string SNI_ERROR_53 => GetResourceString("SNI_ERROR_53", null);

        internal static string SNI_ERROR_54 => GetResourceString("SNI_ERROR_54", null);

        internal static string SNI_ERROR_55 => GetResourceString("SNI_ERROR_55", null);

        internal static string SNI_ERROR_56 => GetResourceString("SNI_ERROR_56", null);

        internal static string SNI_ERROR_57 => GetResourceString("SNI_ERROR_57", null);

        internal static string Snix_Connect => GetResourceString("Snix_Connect", null);

        internal static string Snix_PreLoginBeforeSuccessfulWrite => GetResourceString("Snix_PreLoginBeforeSuccessfulWrite", null);

        internal static string Snix_PreLogin => GetResourceString("Snix_PreLogin", null);

        internal static string Snix_LoginSspi => GetResourceString("Snix_LoginSspi", null);

        internal static string Snix_Login => GetResourceString("Snix_Login", null);

        internal static string Snix_EnableMars => GetResourceString("Snix_EnableMars", null);

        internal static string Snix_AutoEnlist => GetResourceString("Snix_AutoEnlist", null);

        internal static string Snix_GetMarsSession => GetResourceString("Snix_GetMarsSession", null);

        internal static string Snix_Execute => GetResourceString("Snix_Execute", null);

        internal static string Snix_Read => GetResourceString("Snix_Read", null);

        internal static string Snix_Close => GetResourceString("Snix_Close", null);

        internal static string Snix_SendRows => GetResourceString("Snix_SendRows", null);

        internal static string Snix_ProcessSspi => GetResourceString("Snix_ProcessSspi", null);

        internal static string LocalDB_FailedGetDLLHandle => GetResourceString("LocalDB_FailedGetDLLHandle", null);

        internal static string LocalDB_MethodNotFound => GetResourceString("LocalDB_MethodNotFound", null);

        internal static string LocalDB_UnobtainableMessage => GetResourceString("LocalDB_UnobtainableMessage", null);

        internal static string SQLROR_RecursiveRoutingNotSupported => GetResourceString("SQLROR_RecursiveRoutingNotSupported", null);

        internal static string SQLROR_FailoverNotSupported => GetResourceString("SQLROR_FailoverNotSupported", null);

        internal static string SQLROR_UnexpectedRoutingInfo => GetResourceString("SQLROR_UnexpectedRoutingInfo", null);

        internal static string SQLROR_InvalidRoutingInfo => GetResourceString("SQLROR_InvalidRoutingInfo", null);

        internal static string SQLROR_TimeoutAfterRoutingInfo => GetResourceString("SQLROR_TimeoutAfterRoutingInfo", null);

        internal static string SQLCR_InvalidConnectRetryCountValue => GetResourceString("SQLCR_InvalidConnectRetryCountValue", null);

        internal static string SQLCR_InvalidConnectRetryIntervalValue => GetResourceString("SQLCR_InvalidConnectRetryIntervalValue", null);

        internal static string SQLCR_NextAttemptWillExceedQueryTimeout => GetResourceString("SQLCR_NextAttemptWillExceedQueryTimeout", null);

        internal static string SQLCR_EncryptionChanged => GetResourceString("SQLCR_EncryptionChanged", null);

        internal static string SQLCR_TDSVestionNotPreserved => GetResourceString("SQLCR_TDSVestionNotPreserved", null);

        internal static string SQLCR_AllAttemptsFailed => GetResourceString("SQLCR_AllAttemptsFailed", null);

        internal static string SQLCR_UnrecoverableServer => GetResourceString("SQLCR_UnrecoverableServer", null);

        internal static string SQLCR_UnrecoverableClient => GetResourceString("SQLCR_UnrecoverableClient", null);

        internal static string SQLCR_NoCRAckAtReconnection => GetResourceString("SQLCR_NoCRAckAtReconnection", null);

        internal static string SQL_UnsupportedKeyword => GetResourceString("SQL_UnsupportedKeyword", null);

        internal static string SQL_UnsupportedFeature => GetResourceString("SQL_UnsupportedFeature", null);

        internal static string SQL_UnsupportedToken => GetResourceString("SQL_UnsupportedToken", null);

        internal static string SQL_DbTypeNotSupportedOnThisPlatform => GetResourceString("SQL_DbTypeNotSupportedOnThisPlatform", null);

        internal static string SQL_NetworkLibraryNotSupported => GetResourceString("SQL_NetworkLibraryNotSupported", null);

        internal static string SNI_PN0 => GetResourceString("SNI_PN0", null);

        internal static string SNI_PN1 => GetResourceString("SNI_PN1", null);

        internal static string SNI_PN2 => GetResourceString("SNI_PN2", null);

        internal static string SNI_PN3 => GetResourceString("SNI_PN3", null);

        internal static string SNI_PN4 => GetResourceString("SNI_PN4", null);

        internal static string SNI_PN5 => GetResourceString("SNI_PN5", null);

        internal static string SNI_PN6 => GetResourceString("SNI_PN6", null);

        internal static string SNI_PN7 => GetResourceString("SNI_PN7", null);

        internal static string SNI_PN8 => GetResourceString("SNI_PN8", null);

        internal static string SNI_PN9 => GetResourceString("SNI_PN9", null);

        internal static string AZURESQL_GenericEndpoint => GetResourceString("AZURESQL_GenericEndpoint", null);

        internal static string AZURESQL_GermanEndpoint => GetResourceString("AZURESQL_GermanEndpoint", null);

        internal static string AZURESQL_UsGovEndpoint => GetResourceString("AZURESQL_UsGovEndpoint", null);

        internal static string AZURESQL_ChinaEndpoint => GetResourceString("AZURESQL_ChinaEndpoint", null);

        internal static string net_nego_channel_binding_not_supported => GetResourceString("net_nego_channel_binding_not_supported", null);

        internal static string net_gssapi_operation_failed_detailed => GetResourceString("net_gssapi_operation_failed_detailed", null);

        internal static string net_gssapi_operation_failed => GetResourceString("net_gssapi_operation_failed", null);

        internal static string net_ntlm_not_possible_default_cred => GetResourceString("net_ntlm_not_possible_default_cred", null);

        internal static string net_nego_not_supported_empty_target_with_defaultcreds => GetResourceString("net_nego_not_supported_empty_target_with_defaultcreds", null);

        internal static string net_nego_server_not_supported => GetResourceString("net_nego_server_not_supported", null);

        internal static string net_nego_protection_level_not_supported => GetResourceString("net_nego_protection_level_not_supported", null);

        internal static string net_context_buffer_too_small => GetResourceString("net_context_buffer_too_small", null);

        internal static string net_auth_message_not_encrypted => GetResourceString("net_auth_message_not_encrypted", null);

        internal static string net_securitypackagesupport => GetResourceString("net_securitypackagesupport", null);

        internal static string net_log_operation_failed_with_error => GetResourceString("net_log_operation_failed_with_error", null);

        internal static string net_MethodNotImplementedException => GetResourceString("net_MethodNotImplementedException", null);

        internal static string event_OperationReturnedSomething => GetResourceString("event_OperationReturnedSomething", null);

        internal static string net_invalid_enum => GetResourceString("net_invalid_enum", null);

        internal static string SSPIInvalidHandleType => GetResourceString("SSPIInvalidHandleType", null);

        internal static string LocalDBNotSupported => GetResourceString("LocalDBNotSupported", null);

        internal static string PlatformNotSupported_DataSqlClient => GetResourceString("PlatformNotSupported_DataSqlClient", null);

        internal static string SqlParameter_InvalidTableDerivedPrecisionForTvp => GetResourceString("SqlParameter_InvalidTableDerivedPrecisionForTvp", null);

        internal static string SqlProvider_InvalidDataColumnMaxLength => GetResourceString("SqlProvider_InvalidDataColumnMaxLength", null);

        internal static string MDF_InvalidXmlInvalidValue => GetResourceString("MDF_InvalidXmlInvalidValue", null);

        internal static string MDF_CollectionNameISNotUnique => GetResourceString("MDF_CollectionNameISNotUnique", null);

        internal static string MDF_InvalidXmlMissingColumn => GetResourceString("MDF_InvalidXmlMissingColumn", null);

        internal static string MDF_InvalidXml => GetResourceString("MDF_InvalidXml", null);

        internal static string MDF_NoColumns => GetResourceString("MDF_NoColumns", null);

        internal static string MDF_QueryFailed => GetResourceString("MDF_QueryFailed", null);

        internal static string MDF_TooManyRestrictions => GetResourceString("MDF_TooManyRestrictions", null);

        internal static string MDF_DataTableDoesNotExist => GetResourceString("MDF_DataTableDoesNotExist", null);

        internal static string MDF_UndefinedCollection => GetResourceString("MDF_UndefinedCollection", null);

        internal static string MDF_UnsupportedVersion => GetResourceString("MDF_UnsupportedVersion", null);

        internal static string MDF_MissingRestrictionColumn => GetResourceString("MDF_MissingRestrictionColumn", null);

        internal static string MDF_MissingRestrictionRow => GetResourceString("MDF_MissingRestrictionRow", null);

        internal static string MDF_IncorrectNumberOfDataSourceInformationRows => GetResourceString("MDF_IncorrectNumberOfDataSourceInformationRows", null);

        internal static string MDF_MissingDataSourceInformationColumn => GetResourceString("MDF_MissingDataSourceInformationColumn", null);

        internal static string MDF_AmbigousCollectionName => GetResourceString("MDF_AmbigousCollectionName", null);
        internal static string MDF_UnableToBuildCollection => GetResourceString("MDF_UnableToBuildCollection", null);
#else
        internal static string ADP_CollectionIndexInt32 {
              get { return SR.GetResourceString("ADP_CollectionIndexInt32", @"Invalid index {0} for this {1} with Count={2}."); }
        }
        internal static string ADP_CollectionIndexString {
              get { return SR.GetResourceString("ADP_CollectionIndexString", @"An {0} with {1} '{2}' is not contained by this {3}."); }
        }
        internal static string ADP_CollectionInvalidType {
              get { return SR.GetResourceString("ADP_CollectionInvalidType", @"The {0} only accepts non-null {1} type objects, not {2} objects."); }
        }
        internal static string ADP_CollectionIsNotParent {
              get { return SR.GetResourceString("ADP_CollectionIsNotParent", @"The {0} is already contained by another {1}."); }
        }
        internal static string ADP_CollectionNullValue {
              get { return SR.GetResourceString("ADP_CollectionNullValue", @"The {0} only accepts non-null {1} type objects."); }
        }
        internal static string ADP_CollectionRemoveInvalidObject {
              get { return SR.GetResourceString("ADP_CollectionRemoveInvalidObject", @"Attempted to remove an {0} that is not contained by this {1}."); }
        }
        internal static string ADP_ConnectionAlreadyOpen {
              get { return SR.GetResourceString("ADP_ConnectionAlreadyOpen", @"The connection was not closed. {0}"); }
        }
        internal static string ADP_ConnectionStateMsg_Closed {
              get { return SR.GetResourceString("ADP_ConnectionStateMsg_Closed", @"The connection's current state is closed."); }
        }
        internal static string ADP_ConnectionStateMsg_Connecting {
              get { return SR.GetResourceString("ADP_ConnectionStateMsg_Connecting", @"The connection's current state is connecting."); }
        }
        internal static string ADP_ConnectionStateMsg_Open {
              get { return SR.GetResourceString("ADP_ConnectionStateMsg_Open", @"The connection's current state is open."); }
        }
        internal static string ADP_ConnectionStateMsg_OpenExecuting {
              get { return SR.GetResourceString("ADP_ConnectionStateMsg_OpenExecuting", @"The connection's current state is executing."); }
        }
        internal static string ADP_ConnectionStateMsg_OpenFetching {
              get { return SR.GetResourceString("ADP_ConnectionStateMsg_OpenFetching", @"The connection's current state is fetching."); }
        }
        internal static string ADP_ConnectionStateMsg {
              get { return SR.GetResourceString("ADP_ConnectionStateMsg", @"The connection's current state: {0}."); }
        }
        internal static string ADP_ConnectionStringSyntax {
              get { return SR.GetResourceString("ADP_ConnectionStringSyntax", @"Format of the initialization string does not conform to specification starting at index {0}."); }
        }
        internal static string ADP_DataReaderClosed {
              get { return SR.GetResourceString("ADP_DataReaderClosed", @"Invalid attempt to call {0} when reader is closed."); }
        }
        internal static string ADP_InternalConnectionError {
              get { return SR.GetResourceString("ADP_InternalConnectionError", @"Internal DbConnection Error: {0}"); }
        }
        internal static string ADP_InvalidEnumerationValue {
              get { return SR.GetResourceString("ADP_InvalidEnumerationValue", @"The {0} enumeration value, {1}, is invalid."); }
        }
        internal static string ADP_NotSupportedEnumerationValue {
              get { return SR.GetResourceString("ADP_NotSupportedEnumerationValue", @"The {0} enumeration value, {1}, is not supported by the {2} method."); }
        }
        internal static string ADP_InvalidOffsetValue {
              get { return SR.GetResourceString("ADP_InvalidOffsetValue", @"Invalid parameter Offset value '{0}'. The value must be greater than or equal to 0."); }
        }
        internal static string ADP_TransactionPresent {
              get { return SR.GetResourceString("ADP_TransactionPresent", @"Connection currently has transaction enlisted.  Finish current transaction and retry."); }
        }
        internal static string ADP_LocalTransactionPresent {
              get { return SR.GetResourceString("ADP_LocalTransactionPresent", @"Cannot enlist in the transaction because a local transaction is in progress on the connection.  Finish local transaction and retry."); }
        }
        internal static string ADP_NoConnectionString {
              get { return SR.GetResourceString("ADP_NoConnectionString", @"The ConnectionString property has not been initialized."); }
        }
        internal static string ADP_OpenConnectionPropertySet {
              get { return SR.GetResourceString("ADP_OpenConnectionPropertySet", @"Not allowed to change the '{0}' property. {1}"); }
        }
        internal static string ADP_PendingAsyncOperation {
              get { return SR.GetResourceString("ADP_PendingAsyncOperation", @"Can not start another operation while there is an asynchronous operation pending."); }
        }
        internal static string ADP_PooledOpenTimeout {
              get { return SR.GetResourceString("ADP_PooledOpenTimeout", @"Timeout expired.  The timeout period elapsed prior to obtaining a connection from the pool.  This may have occurred because all pooled connections were in use and max pool size was reached."); }
        }
        internal static string ADP_NonPooledOpenTimeout {
              get { return SR.GetResourceString("ADP_NonPooledOpenTimeout", @"Timeout attempting to open the connection.  The time period elapsed prior to attempting to open the connection has been exceeded.  This may have occurred because of too many simultaneous non-pooled connection attempts."); }
        }
        internal static string ADP_SingleValuedProperty {
              get { return SR.GetResourceString("ADP_SingleValuedProperty", @"The only acceptable value for the property '{0}' is '{1}'."); }
        }
        internal static string ADP_DoubleValuedProperty {
              get { return SR.GetResourceString("ADP_DoubleValuedProperty", @"The acceptable values for the property '{0}' are '{1}' or '{2}'."); }
        }
        internal static string ADP_InvalidPrefixSuffix {
              get { return SR.GetResourceString("ADP_InvalidPrefixSuffix", @"Specified QuotePrefix and QuoteSuffix values do not match."); }
        }
        internal static string Arg_ArrayPlusOffTooSmall {
              get { return SR.GetResourceString("Arg_ArrayPlusOffTooSmall", @"Destination array is not long enough to copy all the items in the collection. Check array index and length."); }
        }
        internal static string Arg_RankMultiDimNotSupported {
              get { return SR.GetResourceString("Arg_RankMultiDimNotSupported", @"Only single dimensional arrays are supported for the requested action."); }
        }
        internal static string Arg_RemoveArgNotFound {
              get { return SR.GetResourceString("Arg_RemoveArgNotFound", @"Cannot remove the specified item because it was not found in the specified Collection."); }
        }
        internal static string ArgumentOutOfRange_NeedNonNegNum {
              get { return SR.GetResourceString("ArgumentOutOfRange_NeedNonNegNum", @"Non-negative number required."); }
        }
        internal static string Data_InvalidOffsetLength {
              get { return SR.GetResourceString("Data_InvalidOffsetLength", @"Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection."); }
        }
        internal static string SqlConvert_ConvertFailed {
              get { return SR.GetResourceString("SqlConvert_ConvertFailed", @" Cannot convert object of type '{0}' to object of type '{1}'."); }
        }
        internal static string SQL_WrongType {
              get { return SR.GetResourceString("SQL_WrongType", @"Expecting argument of type {1}, but received type {0}."); }
        }
        internal static string ADP_DeriveParametersNotSupported {
              get { return SR.GetResourceString("ADP_DeriveParametersNotSupported", @"{0} DeriveParameters only supports CommandType.StoredProcedure, not CommandType. {1}."); }
        }
        internal static string ADP_NoStoredProcedureExists {
              get { return SR.GetResourceString("ADP_NoStoredProcedureExists", @"The stored procedure '{0}' doesn't exist."); }
        }
        internal static string ADP_InvalidConnectionOptionValue {
              get { return SR.GetResourceString("ADP_InvalidConnectionOptionValue", @"Invalid value for key '{0}'."); }
        }
        internal static string ADP_MissingConnectionOptionValue {
              get { return SR.GetResourceString("ADP_MissingConnectionOptionValue", @"Use of key '{0}' requires the key '{1}' to be present."); }
        }
        internal static string ADP_InvalidConnectionOptionValueLength {
              get { return SR.GetResourceString("ADP_InvalidConnectionOptionValueLength", @"The value's length for key '{0}' exceeds it's limit of '{1}'."); }
        }
        internal static string ADP_KeywordNotSupported {
              get { return SR.GetResourceString("ADP_KeywordNotSupported", @"Keyword not supported: '{0}'."); }
        }
        internal static string ADP_InternalProviderError {
              get { return SR.GetResourceString("ADP_InternalProviderError", @"Internal .Net Framework Data Provider error {0}."); }
        }
        internal static string ADP_InvalidMultipartName {
              get { return SR.GetResourceString("ADP_InvalidMultipartName", @"{0} '{1}'."); }
        }
        internal static string ADP_InvalidMultipartNameQuoteUsage {
              get { return SR.GetResourceString("ADP_InvalidMultipartNameQuoteUsage", @"{0} '{1}', incorrect usage of quotes."); }
        }
        internal static string ADP_InvalidMultipartNameToManyParts {
              get { return SR.GetResourceString("ADP_InvalidMultipartNameToManyParts", @"{0} '{1}', the current limit of '{2}' is insufficient."); }
        }
        internal static string SQL_SqlCommandCommandText {
              get { return SR.GetResourceString("SQL_SqlCommandCommandText", @"SqlCommand.DeriveParameters failed because the SqlCommand.CommandText property value is an invalid multipart name"); }
        }
        internal static string SQL_BatchedUpdatesNotAvailableOnContextConnection {
              get { return SR.GetResourceString("SQL_BatchedUpdatesNotAvailableOnContextConnection", @"Batching updates is not supported on the context connection."); }
        }
        internal static string SQL_BulkCopyDestinationTableName {
              get { return SR.GetResourceString("SQL_BulkCopyDestinationTableName", @"SqlBulkCopy.WriteToServer failed because the SqlBulkCopy.DestinationTableName is an invalid multipart name"); }
        }
        internal static string SQL_TDSParserTableName {
              get { return SR.GetResourceString("SQL_TDSParserTableName", @"Processing of results from SQL Server failed because of an invalid multipart name"); }
        }
        internal static string SQL_TypeName {
              get { return SR.GetResourceString("SQL_TypeName", @"SqlParameter.TypeName is an invalid multipart name"); }
        }
        internal static string SQLMSF_FailoverPartnerNotSupported {
              get { return SR.GetResourceString("SQLMSF_FailoverPartnerNotSupported", @"Connecting to a mirrored SQL Server instance using the MultiSubnetFailover connection option is not supported."); }
        }
        internal static string SQL_NotSupportedEnumerationValue {
              get { return SR.GetResourceString("SQL_NotSupportedEnumerationValue", @"The {0} enumeration value, {1}, is not supported by the .Net Framework SqlClient Data Provider."); }
        }
        internal static string ADP_CommandTextRequired {
              get { return SR.GetResourceString("ADP_CommandTextRequired", @"{0}: CommandText property has not been initialized"); }
        }
        internal static string ADP_ConnectionRequired {
              get { return SR.GetResourceString("ADP_ConnectionRequired", @"{0}: Connection property has not been initialized."); }
        }
        internal static string ADP_OpenConnectionRequired {
              get { return SR.GetResourceString("ADP_OpenConnectionRequired", @"{0} requires an open and available Connection. {1}"); }
        }
        internal static string ADP_TransactionConnectionMismatch {
              get { return SR.GetResourceString("ADP_TransactionConnectionMismatch", @"The transaction is either not associated with the current connection or has been completed."); }
        }
        internal static string ADP_TransactionRequired {
              get { return SR.GetResourceString("ADP_TransactionRequired", @"{0} requires the command to have a transaction when the connection assigned to the command is in a pending local transaction.  The Transaction property of the command has not been initialized."); }
        }
        internal static string ADP_OpenReaderExists {
              get { return SR.GetResourceString("ADP_OpenReaderExists", @"There is already an open DataReader associated with this Command which must be closed first."); }
        }
        internal static string ADP_CalledTwice {
              get { return SR.GetResourceString("ADP_CalledTwice", @"The method '{0}' cannot be called more than once for the same execution."); }
        }
        internal static string ADP_InvalidCommandTimeout {
              get { return SR.GetResourceString("ADP_InvalidCommandTimeout", @"Invalid CommandTimeout value {0}; the value must be >= 0."); }
        }
        internal static string ADP_UninitializedParameterSize {
              get { return SR.GetResourceString("ADP_UninitializedParameterSize", @"{1}[{0}]: the Size property has an invalid size of 0."); }
        }
        internal static string ADP_PrepareParameterType {
              get { return SR.GetResourceString("ADP_PrepareParameterType", @"{0}.Prepare method requires all parameters to have an explicitly set type."); }
        }
        internal static string ADP_PrepareParameterSize {
              get { return SR.GetResourceString("ADP_PrepareParameterSize", @"{0}.Prepare method requires all variable length parameters to have an explicitly set non-zero Size."); }
        }
        internal static string ADP_PrepareParameterScale {
              get { return SR.GetResourceString("ADP_PrepareParameterScale", @"{0}.Prepare method requires parameters of type '{1}' have an explicitly set Precision and Scale."); }
        }
        internal static string ADP_MismatchedAsyncResult {
              get { return SR.GetResourceString("ADP_MismatchedAsyncResult", @"Mismatched end method call for asyncResult.  Expected call to {0} but {1} was called instead."); }
        }
        internal static string ADP_ClosedConnectionError {
              get { return SR.GetResourceString("ADP_ClosedConnectionError", @"Invalid operation. The connection is closed."); }
        }
        internal static string ADP_ConnectionIsDisabled {
              get { return SR.GetResourceString("ADP_ConnectionIsDisabled", @"The connection has been disabled."); }
        }
        internal static string ADP_EmptyDatabaseName {
              get { return SR.GetResourceString("ADP_EmptyDatabaseName", @"Database cannot be null, the empty string, or string of only whitespace."); }
        }
        internal static string ADP_InvalidSourceBufferIndex {
              get { return SR.GetResourceString("ADP_InvalidSourceBufferIndex", @"Invalid source buffer (size of {0}) offset: {1}"); }
        }
        internal static string ADP_InvalidDestinationBufferIndex {
              get { return SR.GetResourceString("ADP_InvalidDestinationBufferIndex", @"Invalid destination buffer (size of {0}) offset: {1}"); }
        }
        internal static string ADP_StreamClosed {
              get { return SR.GetResourceString("ADP_StreamClosed", @"Invalid attempt to {0} when stream is closed."); }
        }
        internal static string ADP_InvalidSeekOrigin {
              get { return SR.GetResourceString("ADP_InvalidSeekOrigin", @"Specified SeekOrigin value is invalid."); }
        }
        internal static string ADP_NonSequentialColumnAccess {
              get { return SR.GetResourceString("ADP_NonSequentialColumnAccess", @"Invalid attempt to read from column ordinal '{0}'.  With CommandBehavior.SequentialAccess, you may only read from column ordinal '{1}' or greater."); }
        }
        internal static string ADP_InvalidDataType {
              get { return SR.GetResourceString("ADP_InvalidDataType", @"The parameter data type of {0} is invalid."); }
        }
        internal static string ADP_UnknownDataType {
              get { return SR.GetResourceString("ADP_UnknownDataType", @"No mapping exists from object type {0} to a known managed provider native type."); }
        }
        internal static string ADP_UnknownDataTypeCode {
              get { return SR.GetResourceString("ADP_UnknownDataTypeCode", @"Unable to handle an unknown TypeCode {0} returned by Type {1}."); }
        }
        internal static string ADP_DbTypeNotSupported {
              get { return SR.GetResourceString("ADP_DbTypeNotSupported", @"No mapping exists from DbType {0} to a known {1}."); }
        }
        internal static string ADP_VersionDoesNotSupportDataType {
              get { return SR.GetResourceString("ADP_VersionDoesNotSupportDataType", @"The version of SQL Server in use does not support datatype '{0}'."); }
        }
        internal static string ADP_ParameterValueOutOfRange {
              get { return SR.GetResourceString("ADP_ParameterValueOutOfRange", @"Parameter value '{0}' is out of range."); }
        }
        internal static string ADP_BadParameterName {
              get { return SR.GetResourceString("ADP_BadParameterName", @"Specified parameter name '{0}' is not valid."); }
        }
        internal static string ADP_InvalidSizeValue {
              get { return SR.GetResourceString("ADP_InvalidSizeValue", @"Invalid parameter Size value '{0}'. The value must be greater than or equal to 0."); }
        }
        internal static string ADP_NegativeParameter {
              get { return SR.GetResourceString("ADP_NegativeParameter", @"Invalid value for argument '{0}'. The value must be greater than or equal to 0."); }
        }
        internal static string ADP_InvalidMetaDataValue {
              get { return SR.GetResourceString("ADP_InvalidMetaDataValue", @"Invalid value for this metadata."); }
        }
        internal static string ADP_ParameterConversionFailed {
              get { return SR.GetResourceString("ADP_ParameterConversionFailed", @"Failed to convert parameter value from a {0} to a {1}."); }
        }
        internal static string ADP_ParallelTransactionsNotSupported {
              get { return SR.GetResourceString("ADP_ParallelTransactionsNotSupported", @"{0} does not support parallel transactions."); }
        }
        internal static string ADP_TransactionZombied {
              get { return SR.GetResourceString("ADP_TransactionZombied", @"This {0} has completed; it is no longer usable."); }
        }
        internal static string ADP_InvalidDataLength2 {
              get { return SR.GetResourceString("ADP_InvalidDataLength2", @"Specified length '{0}' is out of range."); }
        }
        internal static string ADP_NonSeqByteAccess {
              get { return SR.GetResourceString("ADP_NonSeqByteAccess", @"Invalid {2} attempt at dataIndex '{0}'.  With CommandBehavior.SequentialAccess, you may only read from dataIndex '{1}' or greater."); }
        }
        internal static string ADP_InvalidMinMaxPoolSizeValues {
              get { return SR.GetResourceString("ADP_InvalidMinMaxPoolSizeValues", @"Invalid min or max pool size values, min pool size cannot be greater than the max pool size."); }
        }
        internal static string SQL_InvalidPacketSizeValue {
              get { return SR.GetResourceString("SQL_InvalidPacketSizeValue", @"Invalid 'Packet Size'.  The value must be an integer >= 512 and <= 32768."); }
        }
        internal static string SQL_NullEmptyTransactionName {
              get { return SR.GetResourceString("SQL_NullEmptyTransactionName", @"Invalid transaction or invalid name for a point at which to save within the transaction."); }
        }
        internal static string SQL_UserInstanceFailoverNotCompatible {
              get { return SR.GetResourceString("SQL_UserInstanceFailoverNotCompatible", @"User Instance and Failover are not compatible options.  Please choose only one of the two in the connection string."); }
        }
        internal static string SQL_EncryptionNotSupportedByClient {
              get { return SR.GetResourceString("SQL_EncryptionNotSupportedByClient", @"The instance of SQL Server you attempted to connect to requires encryption but this machine does not support it."); }
        }
        internal static string SQL_EncryptionNotSupportedByServer {
              get { return SR.GetResourceString("SQL_EncryptionNotSupportedByServer", @"The instance of SQL Server you attempted to connect to does not support encryption."); }
        }
        internal static string SQL_InvalidSQLServerVersionUnknown {
              get { return SR.GetResourceString("SQL_InvalidSQLServerVersionUnknown", @"Unsupported SQL Server version.  The .Net Framework SqlClient Data Provider can only be used with SQL Server versions 7.0 and later."); }
        }
        internal static string SQL_CannotCreateNormalizer {
              get { return SR.GetResourceString("SQL_CannotCreateNormalizer", @"Cannot create normalizer for '{0}'."); }
        }
        internal static string SQL_CannotModifyPropertyAsyncOperationInProgress {
              get { return SR.GetResourceString("SQL_CannotModifyPropertyAsyncOperationInProgress", @"{0} cannot be changed while async operation is in progress."); }
        }
        internal static string SQL_InstanceFailure {
              get { return SR.GetResourceString("SQL_InstanceFailure", @"Instance failure."); }
        }
        internal static string SQL_InvalidPartnerConfiguration {
              get { return SR.GetResourceString("SQL_InvalidPartnerConfiguration", @"Server {0}, database {1} is not configured for database mirroring."); }
        }
        internal static string SQL_MarsUnsupportedOnConnection {
              get { return SR.GetResourceString("SQL_MarsUnsupportedOnConnection", @"The connection does not support MultipleActiveResultSets."); }
        }
        internal static string SQL_NonLocalSSEInstance {
              get { return SR.GetResourceString("SQL_NonLocalSSEInstance", @"SSE Instance re-direction is not supported for non-local user instances."); }
        }
        internal static string SQL_PendingBeginXXXExists {
              get { return SR.GetResourceString("SQL_PendingBeginXXXExists", @"The command execution cannot proceed due to a pending asynchronous operation already in progress."); }
        }
        internal static string SQL_NonXmlResult {
              get { return SR.GetResourceString("SQL_NonXmlResult", @"Invalid command sent to ExecuteXmlReader.  The command must return an Xml result."); }
        }
        internal static string SQL_InvalidParameterTypeNameFormat {
              get { return SR.GetResourceString("SQL_InvalidParameterTypeNameFormat", @"Invalid 3 part name format for TypeName."); }
        }
        internal static string SQL_InvalidParameterNameLength {
              get { return SR.GetResourceString("SQL_InvalidParameterNameLength", @"The length of the parameter '{0}' exceeds the limit of 128 characters."); }
        }
        internal static string SQL_PrecisionValueOutOfRange {
              get { return SR.GetResourceString("SQL_PrecisionValueOutOfRange", @"Precision value '{0}' is either less than 0 or greater than the maximum allowed precision of 38."); }
        }
        internal static string SQL_ScaleValueOutOfRange {
              get { return SR.GetResourceString("SQL_ScaleValueOutOfRange", @"Scale value '{0}' is either less than 0 or greater than the maximum allowed scale of 38."); }
        }
        internal static string SQL_TimeScaleValueOutOfRange {
              get { return SR.GetResourceString("SQL_TimeScaleValueOutOfRange", @"Scale value '{0}' is either less than 0 or greater than the maximum allowed scale of 7."); }
        }
        internal static string SQL_ParameterInvalidVariant {
              get { return SR.GetResourceString("SQL_ParameterInvalidVariant", @"Parameter '{0}' exceeds the size limit for the sql_variant datatype."); }
        }
        internal static string SQL_ParameterTypeNameRequired {
              get { return SR.GetResourceString("SQL_ParameterTypeNameRequired", @"The {0} type parameter '{1}' must have a valid type name."); }
        }
        internal static string SQL_InvalidInternalPacketSize {
              get { return SR.GetResourceString("SQL_InvalidInternalPacketSize", @"Invalid internal packet size:"); }
        }
        internal static string SQL_InvalidTDSVersion {
              get { return SR.GetResourceString("SQL_InvalidTDSVersion", @"The SQL Server instance returned an invalid or unsupported protocol version during login negotiation."); }
        }
        internal static string SQL_InvalidTDSPacketSize {
              get { return SR.GetResourceString("SQL_InvalidTDSPacketSize", @"Invalid Packet Size."); }
        }
        internal static string SQL_ParsingError {
              get { return SR.GetResourceString("SQL_ParsingError", @"Internal connection fatal error."); }
        }
        internal static string SQL_ConnectionLockedForBcpEvent {
              get { return SR.GetResourceString("SQL_ConnectionLockedForBcpEvent", @"The connection cannot be used because there is an ongoing operation that must be finished."); }
        }
        internal static string SQL_SNIPacketAllocationFailure {
              get { return SR.GetResourceString("SQL_SNIPacketAllocationFailure", @"Memory allocation for internal connection failed."); }
        }
        internal static string SQL_SmallDateTimeOverflow {
              get { return SR.GetResourceString("SQL_SmallDateTimeOverflow", @"SqlDbType.SmallDateTime overflow.  Value '{0}' is out of range.  Must be between 1/1/1900 12:00:00 AM and 6/6/2079 11:59:59 PM."); }
        }
        internal static string SQL_TimeOverflow {
              get { return SR.GetResourceString("SQL_TimeOverflow", @"SqlDbType.Time overflow.  Value '{0}' is out of range.  Must be between 00:00:00.0000000 and 23:59:59.9999999."); }
        }
        internal static string SQL_MoneyOverflow {
              get { return SR.GetResourceString("SQL_MoneyOverflow", @"SqlDbType.SmallMoney overflow.  Value '{0}' is out of range.  Must be between -214,748.3648 and 214,748.3647."); }
        }
        internal static string SQL_CultureIdError {
              get { return SR.GetResourceString("SQL_CultureIdError", @"The Collation specified by SQL Server is not supported."); }
        }
        internal static string SQL_OperationCancelled {
              get { return SR.GetResourceString("SQL_OperationCancelled", @"Operation cancelled by user."); }
        }
        internal static string SQL_SevereError {
              get { return SR.GetResourceString("SQL_SevereError", @"A severe error occurred on the current command.  The results, if any, should be discarded."); }
        }
        internal static string SQL_SSPIGenerateError {
              get { return SR.GetResourceString("SQL_SSPIGenerateError", @"Failed to generate SSPI context."); }
        }
        internal static string SQL_KerberosTicketMissingError {
              get { return SR.GetResourceString("SQL_KerberosTicketMissingError", @"Cannot access Kerberos ticket. Ensure Kerberos has been initialized with 'kinit'."); }
        }
        internal static string SQL_SqlServerBrowserNotAccessible {
              get { return SR.GetResourceString("SQL_SqlServerBrowserNotAccessible", @"Cannot connect to SQL Server Browser. Ensure SQL Server Browser has been started."); }
        }
        internal static string SQL_InvalidSSPIPacketSize {
              get { return SR.GetResourceString("SQL_InvalidSSPIPacketSize", @"Invalid SSPI packet size."); }
        }
        internal static string SQL_SSPIInitializeError {
              get { return SR.GetResourceString("SQL_SSPIInitializeError", @"Cannot initialize SSPI package."); }
        }
        internal static string SQL_Timeout {
              get { return SR.GetResourceString("SQL_Timeout", @"Timeout expired.  The timeout period elapsed prior to completion of the operation or the server is not responding."); }
        }
        internal static string SQL_Timeout_PreLogin_Begin {
              get { return SR.GetResourceString("SQL_Timeout_PreLogin_Begin", @"Connection Timeout Expired.  The timeout period elapsed at the start of the pre-login phase.  This could be because of insufficient time provided for connection timeout."); }
        }
        internal static string SQL_Timeout_PreLogin_InitializeConnection {
              get { return SR.GetResourceString("SQL_Timeout_PreLogin_InitializeConnection", @"Connection Timeout Expired.  The timeout period elapsed while attempting to create and initialize a socket to the server.  This could be either because the server was unreachable or unable to respond back in time."); }
        }
        internal static string SQL_Timeout_PreLogin_SendHandshake {
              get { return SR.GetResourceString("SQL_Timeout_PreLogin_SendHandshake", @"Connection Timeout Expired.  The timeout period elapsed while making a pre-login handshake request.  This could be because the server was unable to respond back in time."); }
        }
        internal static string SQL_Timeout_PreLogin_ConsumeHandshake {
              get { return SR.GetResourceString("SQL_Timeout_PreLogin_ConsumeHandshake", @"Connection Timeout Expired.  The timeout period elapsed while attempting to consume the pre-login handshake acknowledgement.  This could be because the pre-login handshake failed or the server was unable to respond back in time."); }
        }
        internal static string SQL_Timeout_Login_Begin {
              get { return SR.GetResourceString("SQL_Timeout_Login_Begin", @"Connection Timeout Expired.  The timeout period elapsed at the start of the login phase.  This could be because of insufficient time provided for connection timeout."); }
        }
        internal static string SQL_Timeout_Login_ProcessConnectionAuth {
              get { return SR.GetResourceString("SQL_Timeout_Login_ProcessConnectionAuth", @"Connection Timeout Expired.  The timeout period elapsed while attempting to authenticate the login.  This could be because the server failed to authenticate the user or the server was unable to respond back in time."); }
        }
        internal static string SQL_Timeout_PostLogin {
              get { return SR.GetResourceString("SQL_Timeout_PostLogin", @"Connection Timeout Expired.  The timeout period elapsed during the post-login phase.  The connection could have timed out while waiting for server to complete the login process and respond; Or it could have timed out while attempting to create multiple active connections."); }
        }
        internal static string SQL_Timeout_FailoverInfo {
              get { return SR.GetResourceString("SQL_Timeout_FailoverInfo", @"This failure occurred while attempting to connect to the {0} server."); }
        }
        internal static string SQL_Timeout_RoutingDestinationInfo {
              get { return SR.GetResourceString("SQL_Timeout_RoutingDestinationInfo", @"This failure occurred while attempting to connect to the routing destination. The duration spent while attempting to connect to the original server was - [Pre-Login] initialization={0}; handshake={1}; [Login] initialization={2}; authentication={3}; [Post-Login] complete={4};  "); }
        }
        internal static string SQL_Duration_PreLogin_Begin {
              get { return SR.GetResourceString("SQL_Duration_PreLogin_Begin", @"The duration spent while attempting to connect to this server was - [Pre-Login] initialization={0};"); }
        }
        internal static string SQL_Duration_PreLoginHandshake {
              get { return SR.GetResourceString("SQL_Duration_PreLoginHandshake", @"The duration spent while attempting to connect to this server was - [Pre-Login] initialization={0}; handshake={1}; "); }
        }
        internal static string SQL_Duration_Login_Begin {
              get { return SR.GetResourceString("SQL_Duration_Login_Begin", @"The duration spent while attempting to connect to this server was - [Pre-Login] initialization={0}; handshake={1}; [Login] initialization={2}; "); }
        }
        internal static string SQL_Duration_Login_ProcessConnectionAuth {
              get { return SR.GetResourceString("SQL_Duration_Login_ProcessConnectionAuth", @"The duration spent while attempting to connect to this server was - [Pre-Login] initialization={0}; handshake={1}; [Login] initialization={2}; authentication={3}; "); }
        }
        internal static string SQL_Duration_PostLogin {
              get { return SR.GetResourceString("SQL_Duration_PostLogin", @"The duration spent while attempting to connect to this server was - [Pre-Login] initialization={0}; handshake={1}; [Login] initialization={2}; authentication={3}; [Post-Login] complete={4}; "); }
        }
        internal static string SQL_UserInstanceFailure {
              get { return SR.GetResourceString("SQL_UserInstanceFailure", @"A user instance was requested in the connection string but the server specified does not support this option."); }
        }
        internal static string SQL_InvalidRead {
              get { return SR.GetResourceString("SQL_InvalidRead", @"Invalid attempt to read when no data is present."); }
        }
        internal static string SQL_NonBlobColumn {
              get { return SR.GetResourceString("SQL_NonBlobColumn", @"Invalid attempt to GetBytes on column '{0}'.  The GetBytes function can only be used on columns of type Text, NText, or Image."); }
        }
        internal static string SQL_NonCharColumn {
              get { return SR.GetResourceString("SQL_NonCharColumn", @"Invalid attempt to GetChars on column '{0}'.  The GetChars function can only be used on columns of type Text, NText, Xml, VarChar or NVarChar."); }
        }
        internal static string SQL_StreamNotSupportOnColumnType {
              get { return SR.GetResourceString("SQL_StreamNotSupportOnColumnType", @"Invalid attempt to GetStream on column '{0}'. The GetStream function can only be used on columns of type Binary, Image, Udt or VarBinary."); }
        }
        internal static string SQL_TextReaderNotSupportOnColumnType {
              get { return SR.GetResourceString("SQL_TextReaderNotSupportOnColumnType", @"Invalid attempt to GetTextReader on column '{0}'. The GetTextReader function can only be used on columns of type Char, NChar, NText, NVarChar, Text or VarChar."); }
        }
        internal static string SQL_XmlReaderNotSupportOnColumnType {
              get { return SR.GetResourceString("SQL_XmlReaderNotSupportOnColumnType", @"Invalid attempt to GetXmlReader on column '{0}'. The GetXmlReader function can only be used on columns of type Xml."); }
        }
        internal static string SqlDelegatedTransaction_PromotionFailed {
              get { return SR.GetResourceString("SqlDelegatedTransaction_PromotionFailed", @"Failure while attempting to promote transaction."); }
        }
        internal static string SQL_InvalidBufferSizeOrIndex {
              get { return SR.GetResourceString("SQL_InvalidBufferSizeOrIndex", @"Buffer offset '{1}' plus the bytes available '{0}' is greater than the length of the passed in buffer."); }
        }
        internal static string SQL_InvalidDataLength {
              get { return SR.GetResourceString("SQL_InvalidDataLength", @"Data length '{0}' is less than 0."); }
        }
        internal static string SQL_BulkLoadMappingInaccessible {
              get { return SR.GetResourceString("SQL_BulkLoadMappingInaccessible", @"The mapped collection is in use and cannot be accessed at this time;"); }
        }
        internal static string SQL_BulkLoadMappingsNamesOrOrdinalsOnly {
              get { return SR.GetResourceString("SQL_BulkLoadMappingsNamesOrOrdinalsOnly", @"Mappings must be either all name or all ordinal based."); }
        }
        internal static string SQL_BulkLoadCannotConvertValue {
              get { return SR.GetResourceString("SQL_BulkLoadCannotConvertValue", @"The given value of type {0} from the data source cannot be converted to type {1} of the specified target column."); }
        }
        internal static string SQL_BulkLoadNonMatchingColumnMapping {
              get { return SR.GetResourceString("SQL_BulkLoadNonMatchingColumnMapping", @"The given ColumnMapping does not match up with any column in the source or destination."); }
        }
        internal static string SQL_BulkLoadNonMatchingColumnName {
              get { return SR.GetResourceString("SQL_BulkLoadNonMatchingColumnName", @"The given ColumnName '{0}' does not match up with any column in data source."); }
        }
        internal static string SQL_BulkLoadStringTooLong {
              get { return SR.GetResourceString("SQL_BulkLoadStringTooLong", @"String or binary data would be truncated."); }
        }
        internal static string SQL_BulkLoadInvalidTimeout {
              get { return SR.GetResourceString("SQL_BulkLoadInvalidTimeout", @"Timeout Value '{0}' is less than 0."); }
        }
        internal static string SQL_BulkLoadInvalidVariantValue {
              get { return SR.GetResourceString("SQL_BulkLoadInvalidVariantValue", @"Value cannot be converted to SqlVariant."); }
        }
        internal static string SQL_BulkLoadExistingTransaction {
              get { return SR.GetResourceString("SQL_BulkLoadExistingTransaction", @"Unexpected existing transaction."); }
        }
        internal static string SQL_BulkLoadNoCollation {
              get { return SR.GetResourceString("SQL_BulkLoadNoCollation", @"Failed to obtain column collation information for the destination table. If the table is not in the current database the name must be qualified using the database name (e.g. [mydb]..[mytable](e.g. [mydb]..[mytable]); this also applies to temporary-tables (e.g. #mytable would be specified as tempdb..#mytable)."); }
        }
        internal static string SQL_BulkLoadConflictingTransactionOption {
              get { return SR.GetResourceString("SQL_BulkLoadConflictingTransactionOption", @"Must not specify SqlBulkCopyOption.UseInternalTransaction and pass an external Transaction at the same time."); }
        }
        internal static string SQL_BulkLoadInvalidOperationInsideEvent {
              get { return SR.GetResourceString("SQL_BulkLoadInvalidOperationInsideEvent", @"Function must not be called during event."); }
        }
        internal static string SQL_BulkLoadMissingDestinationTable {
              get { return SR.GetResourceString("SQL_BulkLoadMissingDestinationTable", @"The DestinationTableName property must be set before calling this method."); }
        }
        internal static string SQL_BulkLoadInvalidDestinationTable {
              get { return SR.GetResourceString("SQL_BulkLoadInvalidDestinationTable", @"Cannot access destination table '{0}'."); }
        }
        internal static string SQL_BulkLoadNotAllowDBNull {
              get { return SR.GetResourceString("SQL_BulkLoadNotAllowDBNull", @"Column '{0}' does not allow DBNull.Value."); }
        }
        internal static string Sql_BulkLoadLcidMismatch {
              get { return SR.GetResourceString("Sql_BulkLoadLcidMismatch", @"The locale id '{0}' of the source column '{1}' and the locale id '{2}' of the destination column '{3}' do not match."); }
        }
        internal static string SQL_BulkLoadPendingOperation {
              get { return SR.GetResourceString("SQL_BulkLoadPendingOperation", @"Attempt to invoke bulk copy on an object that has a pending operation."); }
        }
        internal static string SQL_CannotGetDTCAddress {
              get { return SR.GetResourceString("SQL_CannotGetDTCAddress", @"Unable to get the address of the distributed transaction coordinator for the server, from the server.  Is DTC enabled on the server?"); }
        }
        internal static string SQL_ConnectionDoomed {
              get { return SR.GetResourceString("SQL_ConnectionDoomed", @"The requested operation cannot be completed because the connection has been broken."); }
        }
        internal static string SQL_OpenResultCountExceeded {
              get { return SR.GetResourceString("SQL_OpenResultCountExceeded", @"Open result count exceeded."); }
        }
        internal static string SQL_StreamWriteNotSupported {
              get { return SR.GetResourceString("SQL_StreamWriteNotSupported", @"The Stream does not support writing."); }
        }
        internal static string SQL_StreamReadNotSupported {
              get { return SR.GetResourceString("SQL_StreamReadNotSupported", @"The Stream does not support reading."); }
        }
        internal static string SQL_StreamSeekNotSupported {
              get { return SR.GetResourceString("SQL_StreamSeekNotSupported", @"The Stream does not support seeking."); }
        }
        internal static string SQL_ExClientConnectionId {
              get { return SR.GetResourceString("SQL_ExClientConnectionId", @"ClientConnectionId:{0}"); }
        }
        internal static string SQL_ExErrorNumberStateClass {
              get { return SR.GetResourceString("SQL_ExErrorNumberStateClass", @"Error Number:{0},State:{1},Class:{2}"); }
        }
        internal static string SQL_ExOriginalClientConnectionId {
              get { return SR.GetResourceString("SQL_ExOriginalClientConnectionId", @"ClientConnectionId before routing:{0}"); }
        }
        internal static string SQL_ExRoutingDestination {
              get { return SR.GetResourceString("SQL_ExRoutingDestination", @"Routing Destination:{0}"); }
        }
        internal static string SQL_UnsupportedSysTxVersion {
              get { return SR.GetResourceString("SQL_UnsupportedSysTxVersion", @"The currently loaded System.Transactions.dll does not support Global Transactions."); }
        }
        internal static string SqlMisc_NullString {
              get { return SR.GetResourceString("SqlMisc_NullString", @"Null"); }
        }
        internal static string SqlMisc_MessageString {
              get { return SR.GetResourceString("SqlMisc_MessageString", @"Message"); }
        }
        internal static string SqlMisc_ArithOverflowMessage {
              get { return SR.GetResourceString("SqlMisc_ArithOverflowMessage", @"Arithmetic Overflow."); }
        }
        internal static string SqlMisc_DivideByZeroMessage {
              get { return SR.GetResourceString("SqlMisc_DivideByZeroMessage", @"Divide by zero error encountered."); }
        }
        internal static string SqlMisc_NullValueMessage {
              get { return SR.GetResourceString("SqlMisc_NullValueMessage", @"Data is Null. This method or property cannot be called on Null values."); }
        }
        internal static string SqlMisc_TruncationMessage {
              get { return SR.GetResourceString("SqlMisc_TruncationMessage", @"Numeric arithmetic causes truncation."); }
        }
        internal static string SqlMisc_DateTimeOverflowMessage {
              get { return SR.GetResourceString("SqlMisc_DateTimeOverflowMessage", @"SqlDateTime overflow. Must be between 1/1/1753 12:00:00 AM and 12/31/9999 11:59:59 PM."); }
        }
        internal static string SqlMisc_ConcatDiffCollationMessage {
              get { return SR.GetResourceString("SqlMisc_ConcatDiffCollationMessage", @"Two strings to be concatenated have different collation."); }
        }
        internal static string SqlMisc_CompareDiffCollationMessage {
              get { return SR.GetResourceString("SqlMisc_CompareDiffCollationMessage", @"Two strings to be compared have different collation."); }
        }
        internal static string SqlMisc_InvalidFlagMessage {
              get { return SR.GetResourceString("SqlMisc_InvalidFlagMessage", @"Invalid flag value."); }
        }
        internal static string SqlMisc_NumeToDecOverflowMessage {
              get { return SR.GetResourceString("SqlMisc_NumeToDecOverflowMessage", @"Conversion from SqlDecimal to Decimal overflows."); }
        }
        internal static string SqlMisc_ConversionOverflowMessage {
              get { return SR.GetResourceString("SqlMisc_ConversionOverflowMessage", @"Conversion overflows."); }
        }
        internal static string SqlMisc_InvalidDateTimeMessage {
              get { return SR.GetResourceString("SqlMisc_InvalidDateTimeMessage", @"Invalid SqlDateTime."); }
        }
        internal static string SqlMisc_TimeZoneSpecifiedMessage {
              get { return SR.GetResourceString("SqlMisc_TimeZoneSpecifiedMessage", @"A time zone was specified. SqlDateTime does not support time zones."); }
        }
        internal static string SqlMisc_InvalidArraySizeMessage {
              get { return SR.GetResourceString("SqlMisc_InvalidArraySizeMessage", @"Invalid array size."); }
        }
        internal static string SqlMisc_InvalidPrecScaleMessage {
              get { return SR.GetResourceString("SqlMisc_InvalidPrecScaleMessage", @"Invalid numeric precision/scale."); }
        }
        internal static string SqlMisc_FormatMessage {
              get { return SR.GetResourceString("SqlMisc_FormatMessage", @"The input wasn't in a correct format."); }
        }
        internal static string SqlMisc_StreamErrorMessage {
              get { return SR.GetResourceString("SqlMisc_StreamErrorMessage", @"An error occurred while reading."); }
        }
        internal static string SqlMisc_TruncationMaxDataMessage {
              get { return SR.GetResourceString("SqlMisc_TruncationMaxDataMessage", @"Data returned is larger than 2Gb in size. Use SequentialAccess command behavior in order to get all of the data."); }
        }
        internal static string SqlMisc_NotFilledMessage {
              get { return SR.GetResourceString("SqlMisc_NotFilledMessage", @"SQL Type has not been loaded with data."); }
        }
        internal static string SqlMisc_AlreadyFilledMessage {
              get { return SR.GetResourceString("SqlMisc_AlreadyFilledMessage", @"SQL Type has already been loaded with data."); }
        }
        internal static string SqlMisc_ClosedXmlReaderMessage {
              get { return SR.GetResourceString("SqlMisc_ClosedXmlReaderMessage", @"Invalid attempt to access a closed XmlReader."); }
        }
        internal static string SqlMisc_InvalidOpStreamClosed {
              get { return SR.GetResourceString("SqlMisc_InvalidOpStreamClosed", @"Invalid attempt to call {0} when the stream is closed."); }
        }
        internal static string SqlMisc_InvalidOpStreamNonWritable {
              get { return SR.GetResourceString("SqlMisc_InvalidOpStreamNonWritable", @"Invalid attempt to call {0} when the stream non-writable."); }
        }
        internal static string SqlMisc_InvalidOpStreamNonReadable {
              get { return SR.GetResourceString("SqlMisc_InvalidOpStreamNonReadable", @"Invalid attempt to call {0} when the stream non-readable."); }
        }
        internal static string SqlMisc_InvalidOpStreamNonSeekable {
              get { return SR.GetResourceString("SqlMisc_InvalidOpStreamNonSeekable", @"Invalid attempt to call {0} when the stream is non-seekable."); }
        }
        internal static string SqlMisc_SubclassMustOverride {
              get { return SR.GetResourceString("SqlMisc_SubclassMustOverride", @"Subclass did not override a required method."); }
        }
        internal static string SqlUdtReason_NoUdtAttribute {
              get { return SR.GetResourceString("SqlUdtReason_NoUdtAttribute", @"no UDT attribute"); }
        }
        internal static string SQLUDT_InvalidSqlType {
              get { return SR.GetResourceString("SQLUDT_InvalidSqlType", @"Specified type is not registered on the target server. {0}."); }
        }
        internal static string Sql_InternalError {
              get { return SR.GetResourceString("Sql_InternalError", @"Internal Error"); }
        }
        internal static string ADP_OperationAborted {
              get { return SR.GetResourceString("ADP_OperationAborted", @"Operation aborted."); }
        }
        internal static string ADP_OperationAbortedExceptionMessage {
              get { return SR.GetResourceString("ADP_OperationAbortedExceptionMessage", @"Operation aborted due to an exception (see InnerException for details)."); }
        }
        internal static string ADP_TransactionCompletedButNotDisposed {
              get { return SR.GetResourceString("ADP_TransactionCompletedButNotDisposed", @"The transaction associated with the current connection has completed but has not been disposed.  The transaction must be disposed before the connection can be used to execute SQL statements."); }
        }
        internal static string SqlParameter_UnsupportedTVPOutputParameter {
              get { return SR.GetResourceString("SqlParameter_UnsupportedTVPOutputParameter", @"ParameterDirection '{0}' specified for parameter '{1}' is not supported. Table-valued parameters only support ParameterDirection.Input."); }
        }
        internal static string SqlParameter_DBNullNotSupportedForTVP {
              get { return SR.GetResourceString("SqlParameter_DBNullNotSupportedForTVP", @"DBNull value for parameter '{0}' is not supported. Table-valued parameters cannot be DBNull."); }
        }
        internal static string SqlParameter_UnexpectedTypeNameForNonStruct {
              get { return SR.GetResourceString("SqlParameter_UnexpectedTypeNameForNonStruct", @"TypeName specified for parameter '{0}'.  TypeName must only be set for Structured parameters."); }
        }
        internal static string NullSchemaTableDataTypeNotSupported {
              get { return SR.GetResourceString("NullSchemaTableDataTypeNotSupported", @"DateType column for field '{0}' in schema table is null.  DataType must be non-null."); }
        }
        internal static string InvalidSchemaTableOrdinals {
              get { return SR.GetResourceString("InvalidSchemaTableOrdinals", @"Invalid column ordinals in schema table.  ColumnOrdinals, if present, must not have duplicates or gaps."); }
        }
        internal static string SQL_EnumeratedRecordMetaDataChanged {
              get { return SR.GetResourceString("SQL_EnumeratedRecordMetaDataChanged", @"Metadata for field '{0}' of record '{1}' did not match the original record's metadata."); }
        }
        internal static string SQL_EnumeratedRecordFieldCountChanged {
              get { return SR.GetResourceString("SQL_EnumeratedRecordFieldCountChanged", @"Number of fields in record '{0}' does not match the number in the original record."); }
        }
        internal static string GT_Disabled {
              get { return SR.GetResourceString("GT_Disabled", @"Global Transactions are not enabled for this Azure SQL Database. Please contact Azure SQL Database support for assistance."); }
        }
        internal static string SQL_UnknownSysTxIsolationLevel {
              get { return SR.GetResourceString("SQL_UnknownSysTxIsolationLevel", @"Unrecognized System.Transactions.IsolationLevel enumeration value: {0}."); }
        }
        internal static string SQLNotify_AlreadyHasCommand {
              get { return SR.GetResourceString("SQLNotify_AlreadyHasCommand", @"This SqlCommand object is already associated with another SqlDependency object."); }
        }
        internal static string SqlDependency_DatabaseBrokerDisabled {
              get { return SR.GetResourceString("SqlDependency_DatabaseBrokerDisabled", @"The SQL Server Service Broker for the current database is not enabled, and as a result query notifications are not supported.  Please enable the Service Broker for this database if you wish to use notifications."); }
        }
        internal static string SqlDependency_DefaultOptionsButNoStart {
              get { return SR.GetResourceString("SqlDependency_DefaultOptionsButNoStart", @"When using SqlDependency without providing an options value, SqlDependency.Start() must be called prior to execution of a command added to the SqlDependency instance."); }
        }
        internal static string SqlDependency_NoMatchingServerStart {
              get { return SR.GetResourceString("SqlDependency_NoMatchingServerStart", @"When using SqlDependency without providing an options value, SqlDependency.Start() must be called for each server that is being executed against."); }
        }
        internal static string SqlDependency_NoMatchingServerDatabaseStart {
              get { return SR.GetResourceString("SqlDependency_NoMatchingServerDatabaseStart", @"SqlDependency.Start has been called for the server the command is executing against more than once, but there is no matching server/user/database Start() call for current command."); }
        }
        internal static string SqlDependency_EventNoDuplicate {
              get { return SR.GetResourceString("SqlDependency_EventNoDuplicate", @"SqlDependency.OnChange does not support multiple event registrations for the same delegate."); }
        }
        internal static string SqlDependency_IdMismatch {
              get { return SR.GetResourceString("SqlDependency_IdMismatch", @"No SqlDependency exists for the key."); }
        }
        internal static string SqlDependency_InvalidTimeout {
              get { return SR.GetResourceString("SqlDependency_InvalidTimeout", @"Timeout specified is invalid. Timeout cannot be < 0."); }
        }
        internal static string SqlDependency_DuplicateStart {
              get { return SR.GetResourceString("SqlDependency_DuplicateStart", @"SqlDependency does not support calling Start() with different connection strings having the same server, user, and database in the same app domain."); }
        }
        internal static string SqlMetaData_InvalidSqlDbTypeForConstructorFormat {
              get { return SR.GetResourceString("SqlMetaData_InvalidSqlDbTypeForConstructorFormat", @"The dbType {0} is invalid for this constructor."); }
        }
        internal static string SqlMetaData_NameTooLong {
              get { return SR.GetResourceString("SqlMetaData_NameTooLong", @"The name is too long."); }
        }
        internal static string SqlMetaData_SpecifyBothSortOrderAndOrdinal {
              get { return SR.GetResourceString("SqlMetaData_SpecifyBothSortOrderAndOrdinal", @"The sort order and ordinal must either both be specified, or neither should be specified (SortOrder.Unspecified and -1).  The values given were: order = {0}, ordinal = {1}."); }
        }
        internal static string SqlProvider_InvalidDataColumnType {
              get { return SR.GetResourceString("SqlProvider_InvalidDataColumnType", @"The type of column '{0}' is not supported.  The type is '{1}'"); }
        }
        internal static string SqlProvider_NotEnoughColumnsInStructuredType {
              get { return SR.GetResourceString("SqlProvider_NotEnoughColumnsInStructuredType", @"There are not enough fields in the Structured type.  Structured types must have at least one field."); }
        }
        internal static string SqlProvider_DuplicateSortOrdinal {
              get { return SR.GetResourceString("SqlProvider_DuplicateSortOrdinal", @"The sort ordinal {0} was specified twice."); }
        }
        internal static string SqlProvider_MissingSortOrdinal {
              get { return SR.GetResourceString("SqlProvider_MissingSortOrdinal", @"The sort ordinal {0} was not specified."); }
        }
        internal static string SqlProvider_SortOrdinalGreaterThanFieldCount {
              get { return SR.GetResourceString("SqlProvider_SortOrdinalGreaterThanFieldCount", @"The sort ordinal {0} on field {1} exceeds the total number of fields."); }
        }
        internal static string SQLUDT_MaxByteSizeValue {
              get { return SR.GetResourceString("SQLUDT_MaxByteSizeValue", @"range: 0-8000"); }
        }
        internal static string SQLUDT_Unexpected {
              get { return SR.GetResourceString("SQLUDT_Unexpected", @"unexpected error encountered in SqlClient data provider. {0}"); }
        }
        internal static string SQLUDT_UnexpectedUdtTypeName {
              get { return SR.GetResourceString("SQLUDT_UnexpectedUdtTypeName", @"UdtTypeName property must be set only for UDT parameters."); }
        }
        internal static string SQLUDT_InvalidUdtTypeName {
              get { return SR.GetResourceString("SQLUDT_InvalidUdtTypeName", @"UdtTypeName property must be set for UDT parameters."); }
        }
        internal static string SqlUdt_InvalidUdtMessage {
              get { return SR.GetResourceString("SqlUdt_InvalidUdtMessage", @"'{0}' is an invalid user defined type, reason: {1}."); }
        }
        internal static string SQL_UDTTypeName {
              get { return SR.GetResourceString("SQL_UDTTypeName", @"SqlParameter.UdtTypeName is an invalid multipart name"); }
        }
        internal static string SQL_InvalidUdt3PartNameFormat {
              get { return SR.GetResourceString("SQL_InvalidUdt3PartNameFormat", @"Invalid 3 part name format for UdtTypeName."); }
        }
        internal static string IEnumerableOfSqlDataRecordHasNoRows {
              get { return SR.GetResourceString("IEnumerableOfSqlDataRecordHasNoRows", @"There are no records in the SqlDataRecord enumeration. To send a table-valued parameter with no rows, use a null reference for the value instead."); }
        }
        internal static string SNI_ERROR_1 {
              get { return SR.GetResourceString("SNI_ERROR_1", @"I/O Error detected in read/write operation"); }
        }
        internal static string SNI_ERROR_2 {
              get { return SR.GetResourceString("SNI_ERROR_2", @"Connection was terminated"); }
        }
        internal static string SNI_ERROR_3 {
              get { return SR.GetResourceString("SNI_ERROR_3", @"Asynchronous operations not supported"); }
        }
        internal static string SNI_ERROR_5 {
              get { return SR.GetResourceString("SNI_ERROR_5", @"Invalid parameter(s) found"); }
        }
        internal static string SNI_ERROR_6 {
              get { return SR.GetResourceString("SNI_ERROR_6", @"Unsupported protocol specified"); }
        }
        internal static string SNI_ERROR_7 {
              get { return SR.GetResourceString("SNI_ERROR_7", @"Invalid connection found when setting up new session protocol"); }
        }
        internal static string SNI_ERROR_8 {
              get { return SR.GetResourceString("SNI_ERROR_8", @"Protocol not supported"); }
        }
        internal static string SNI_ERROR_9 {
              get { return SR.GetResourceString("SNI_ERROR_9", @"Associating port with I/O completion mechanism failed"); }
        }
        internal static string SNI_ERROR_11 {
              get { return SR.GetResourceString("SNI_ERROR_11", @"Timeout error"); }
        }
        internal static string SNI_ERROR_12 {
              get { return SR.GetResourceString("SNI_ERROR_12", @"No server name supplied"); }
        }
        internal static string SNI_ERROR_13 {
              get { return SR.GetResourceString("SNI_ERROR_13", @"TerminateListener() has been called"); }
        }
        internal static string SNI_ERROR_14 {
              get { return SR.GetResourceString("SNI_ERROR_14", @"Win9x not supported"); }
        }
        internal static string SNI_ERROR_15 {
              get { return SR.GetResourceString("SNI_ERROR_15", @"Function not supported"); }
        }
        internal static string SNI_ERROR_16 {
              get { return SR.GetResourceString("SNI_ERROR_16", @"Shared-Memory heap error"); }
        }
        internal static string SNI_ERROR_17 {
              get { return SR.GetResourceString("SNI_ERROR_17", @"Cannot find an ip/ipv6 type address to connect"); }
        }
        internal static string SNI_ERROR_18 {
              get { return SR.GetResourceString("SNI_ERROR_18", @"Connection has been closed by peer"); }
        }
        internal static string SNI_ERROR_19 {
              get { return SR.GetResourceString("SNI_ERROR_19", @"Physical connection is not usable"); }
        }
        internal static string SNI_ERROR_20 {
              get { return SR.GetResourceString("SNI_ERROR_20", @"Connection has been closed"); }
        }
        internal static string SNI_ERROR_21 {
              get { return SR.GetResourceString("SNI_ERROR_21", @"Encryption is enforced but there is no valid certificate"); }
        }
        internal static string SNI_ERROR_22 {
              get { return SR.GetResourceString("SNI_ERROR_22", @"Couldn't load library"); }
        }
        internal static string SNI_ERROR_23 {
              get { return SR.GetResourceString("SNI_ERROR_23", @"Cannot open a new thread in server process"); }
        }
        internal static string SNI_ERROR_24 {
              get { return SR.GetResourceString("SNI_ERROR_24", @"Cannot post event to completion port"); }
        }
        internal static string SNI_ERROR_25 {
              get { return SR.GetResourceString("SNI_ERROR_25", @"Connection string is not valid"); }
        }
        internal static string SNI_ERROR_26 {
              get { return SR.GetResourceString("SNI_ERROR_26", @"Error Locating Server/Instance Specified"); }
        }
        internal static string SNI_ERROR_27 {
              get { return SR.GetResourceString("SNI_ERROR_27", @"Error getting enabled protocols list from registry"); }
        }
        internal static string SNI_ERROR_28 {
              get { return SR.GetResourceString("SNI_ERROR_28", @"Server doesn't support requested protocol"); }
        }
        internal static string SNI_ERROR_29 {
              get { return SR.GetResourceString("SNI_ERROR_29", @"Shared Memory is not supported for clustered server connectivity"); }
        }
        internal static string SNI_ERROR_30 {
              get { return SR.GetResourceString("SNI_ERROR_30", @"Invalid attempt bind to shared memory segment"); }
        }
        internal static string SNI_ERROR_31 {
              get { return SR.GetResourceString("SNI_ERROR_31", @"Encryption(ssl/tls) handshake failed"); }
        }
        internal static string SNI_ERROR_32 {
              get { return SR.GetResourceString("SNI_ERROR_32", @"Packet size too large for SSL Encrypt/Decrypt operations"); }
        }
        internal static string SNI_ERROR_33 {
              get { return SR.GetResourceString("SNI_ERROR_33", @"SSRP error"); }
        }
        internal static string SNI_ERROR_34 {
              get { return SR.GetResourceString("SNI_ERROR_34", @"Could not connect to the Shared Memory pipe"); }
        }
        internal static string SNI_ERROR_35 {
              get { return SR.GetResourceString("SNI_ERROR_35", @"An internal exception was caught"); }
        }
        internal static string SNI_ERROR_36 {
              get { return SR.GetResourceString("SNI_ERROR_36", @"The Shared Memory dll used to connect to SQL Server 2000 was not found"); }
        }
        internal static string SNI_ERROR_37 {
              get { return SR.GetResourceString("SNI_ERROR_37", @"The SQL Server 2000 Shared Memory client dll appears to be invalid/corrupted"); }
        }
        internal static string SNI_ERROR_38 {
              get { return SR.GetResourceString("SNI_ERROR_38", @"Cannot open a Shared Memory connection to SQL Server 2000"); }
        }
        internal static string SNI_ERROR_39 {
              get { return SR.GetResourceString("SNI_ERROR_39", @"Shared memory connectivity to SQL Server 2000 is either disabled or not available on this machine"); }
        }
        internal static string SNI_ERROR_40 {
              get { return SR.GetResourceString("SNI_ERROR_40", @"Could not open a connection to SQL Server"); }
        }
        internal static string SNI_ERROR_41 {
              get { return SR.GetResourceString("SNI_ERROR_41", @"Cannot open a Shared Memory connection to a remote SQL server"); }
        }
        internal static string SNI_ERROR_42 {
              get { return SR.GetResourceString("SNI_ERROR_42", @"Could not establish dedicated administrator connection (DAC) on default port. Make sure that DAC is enabled"); }
        }
        internal static string SNI_ERROR_43 {
              get { return SR.GetResourceString("SNI_ERROR_43", @"An error occurred while obtaining the dedicated administrator connection (DAC) port. Make sure that SQL Browser is running, or check the error log for the port number"); }
        }
        internal static string SNI_ERROR_44 {
              get { return SR.GetResourceString("SNI_ERROR_44", @"Could not compose Service Principal Name (SPN) for Windows Integrated Authentication. Possible causes are server(s) incorrectly specified to connection API calls, Domain Name System (DNS) lookup failure or memory shortage"); }
        }
        internal static string SNI_ERROR_47 {
              get { return SR.GetResourceString("SNI_ERROR_47", @"Connecting with the MultiSubnetFailover connection option to a SQL Server instance configured with more than 64 IP addresses is not supported."); }
        }
        internal static string SNI_ERROR_48 {
              get { return SR.GetResourceString("SNI_ERROR_48", @"Connecting to a named SQL Server instance using the MultiSubnetFailover connection option is not supported."); }
        }
        internal static string SNI_ERROR_49 {
              get { return SR.GetResourceString("SNI_ERROR_49", @"Connecting to a SQL Server instance using the MultiSubnetFailover connection option is only supported when using the TCP protocol."); }
        }
        internal static string SNI_ERROR_50 {
              get { return SR.GetResourceString("SNI_ERROR_50", @"Local Database Runtime error occurred. "); }
        }
        internal static string SNI_ERROR_51 {
              get { return SR.GetResourceString("SNI_ERROR_51", @"An instance name was not specified while connecting to a Local Database Runtime. Specify an instance name in the format (localdb)\instance_name."); }
        }
        internal static string SNI_ERROR_52 {
              get { return SR.GetResourceString("SNI_ERROR_52", @"Unable to locate a Local Database Runtime installation. Verify that SQL Server Express is properly installed and that the Local Database Runtime feature is enabled."); }
        }
        internal static string SNI_ERROR_53 {
              get { return SR.GetResourceString("SNI_ERROR_53", @"Invalid Local Database Runtime registry configuration found. Verify that SQL Server Express is properly installed."); }
        }
        internal static string SNI_ERROR_54 {
              get { return SR.GetResourceString("SNI_ERROR_54", @"Unable to locate the registry entry for SQLUserInstance.dll file path. Verify that the Local Database Runtime feature of SQL Server Express is properly installed."); }
        }
        internal static string SNI_ERROR_55 {
              get { return SR.GetResourceString("SNI_ERROR_55", @"Registry value contains an invalid SQLUserInstance.dll file path. Verify that the Local Database Runtime feature of SQL Server Express is properly installed."); }
        }
        internal static string SNI_ERROR_56 {
              get { return SR.GetResourceString("SNI_ERROR_56", @"Unable to load the SQLUserInstance.dll from the location specified in the registry. Verify that the Local Database Runtime feature of SQL Server Express is properly installed."); }
        }
        internal static string SNI_ERROR_57 {
              get { return SR.GetResourceString("SNI_ERROR_57", @"Invalid SQLUserInstance.dll found at the location specified in the registry. Verify that the Local Database Runtime feature of SQL Server Express is properly installed."); }
        }
        internal static string Snix_Connect {
              get { return SR.GetResourceString("Snix_Connect", @"A network-related or instance-specific error occurred while establishing a connection to SQL Server. The server was not found or was not accessible. Verify that the instance name is correct and that SQL Server is configured to allow remote connections."); }
        }
        internal static string Snix_PreLoginBeforeSuccessfulWrite {
              get { return SR.GetResourceString("Snix_PreLoginBeforeSuccessfulWrite", @"The client was unable to establish a connection because of an error during connection initialization process before login. Possible causes include the following:  the client tried to connect to an unsupported version of SQL Server; the server was too busy to accept new connections; or there was a resource limitation (insufficient memory or maximum allowed connections) on the server."); }
        }
        internal static string Snix_PreLogin {
              get { return SR.GetResourceString("Snix_PreLogin", @"A connection was successfully established with the server, but then an error occurred during the pre-login handshake."); }
        }
        internal static string Snix_LoginSspi {
              get { return SR.GetResourceString("Snix_LoginSspi", @"A connection was successfully established with the server, but then an error occurred when obtaining the security/SSPI context information for integrated security login."); }
        }
        internal static string Snix_Login {
              get { return SR.GetResourceString("Snix_Login", @"A connection was successfully established with the server, but then an error occurred during the login process."); }
        }
        internal static string Snix_EnableMars {
              get { return SR.GetResourceString("Snix_EnableMars", @"Connection open and login was successful, but then an error occurred while enabling MARS for this connection."); }
        }
        internal static string Snix_AutoEnlist {
              get { return SR.GetResourceString("Snix_AutoEnlist", @"Connection open and login was successful, but then an error occurred while enlisting the connection into the current distributed transaction."); }
        }
        internal static string Snix_GetMarsSession {
              get { return SR.GetResourceString("Snix_GetMarsSession", @"Failed to establish a MARS session in preparation to send the request to the server."); }
        }
        internal static string Snix_Execute {
              get { return SR.GetResourceString("Snix_Execute", @"A transport-level error has occurred when sending the request to the server."); }
        }
        internal static string Snix_Read {
              get { return SR.GetResourceString("Snix_Read", @"A transport-level error has occurred when receiving results from the server."); }
        }
        internal static string Snix_Close {
              get { return SR.GetResourceString("Snix_Close", @"A transport-level error has occurred during connection clean-up."); }
        }
        internal static string Snix_SendRows {
              get { return SR.GetResourceString("Snix_SendRows", @"A transport-level error has occurred while sending information to the server."); }
        }
        internal static string Snix_ProcessSspi {
              get { return SR.GetResourceString("Snix_ProcessSspi", @"A transport-level error has occurred during SSPI handshake."); }
        }
        internal static string LocalDB_FailedGetDLLHandle {
              get { return SR.GetResourceString("LocalDB_FailedGetDLLHandle", @"Local Database Runtime: Cannot load SQLUserInstance.dll."); }
        }
        internal static string LocalDB_MethodNotFound {
              get { return SR.GetResourceString("LocalDB_MethodNotFound", @"Invalid SQLUserInstance.dll found at the location specified in the registry. Verify that the Local Database Runtime feature of SQL Server Express is properly installed."); }
        }
        internal static string LocalDB_UnobtainableMessage {
              get { return SR.GetResourceString("LocalDB_UnobtainableMessage", @"Cannot obtain Local Database Runtime error message"); }
        }
        internal static string SQLROR_RecursiveRoutingNotSupported {
              get { return SR.GetResourceString("SQLROR_RecursiveRoutingNotSupported", @"Two or more redirections have occurred. Only one redirection per login is allowed."); }
        }
        internal static string SQLROR_FailoverNotSupported {
              get { return SR.GetResourceString("SQLROR_FailoverNotSupported", @"Connecting to a mirrored SQL Server instance using the ApplicationIntent ReadOnly connection option is not supported."); }
        }
        internal static string SQLROR_UnexpectedRoutingInfo {
              get { return SR.GetResourceString("SQLROR_UnexpectedRoutingInfo", @"Unexpected routing information received."); }
        }
        internal static string SQLROR_InvalidRoutingInfo {
              get { return SR.GetResourceString("SQLROR_InvalidRoutingInfo", @"Invalid routing information received."); }
        }
        internal static string SQLROR_TimeoutAfterRoutingInfo {
              get { return SR.GetResourceString("SQLROR_TimeoutAfterRoutingInfo", @"Server provided routing information, but timeout already expired."); }
        }
        internal static string SQLCR_InvalidConnectRetryCountValue {
              get { return SR.GetResourceString("SQLCR_InvalidConnectRetryCountValue", @"Invalid ConnectRetryCount value (should be 0-255)."); }
        }
        internal static string SQLCR_InvalidConnectRetryIntervalValue {
              get { return SR.GetResourceString("SQLCR_InvalidConnectRetryIntervalValue", @"Invalid ConnectRetryInterval value (should be 1-60)."); }
        }
        internal static string SQLCR_NextAttemptWillExceedQueryTimeout {
              get { return SR.GetResourceString("SQLCR_NextAttemptWillExceedQueryTimeout", @"Next reconnection attempt will exceed query timeout. Reconnection was terminated."); }
        }
        internal static string SQLCR_EncryptionChanged {
              get { return SR.GetResourceString("SQLCR_EncryptionChanged", @"The server did not preserve SSL encryption during a recovery attempt, connection recovery is not possible."); }
        }
        internal static string SQLCR_TDSVestionNotPreserved {
              get { return SR.GetResourceString("SQLCR_TDSVestionNotPreserved", @"The server did not preserve the exact client TDS version requested during a recovery attempt, connection recovery is not possible."); }
        }
        internal static string SQLCR_AllAttemptsFailed {
              get { return SR.GetResourceString("SQLCR_AllAttemptsFailed", @"The connection is broken and recovery is not possible.  The client driver attempted to recover the connection one or more times and all attempts failed.  Increase the value of ConnectRetryCount to increase the number of recovery attempts."); }
        }
        internal static string SQLCR_UnrecoverableServer {
              get { return SR.GetResourceString("SQLCR_UnrecoverableServer", @"The connection is broken and recovery is not possible.  The connection is marked by the server as unrecoverable.  No attempt was made to restore the connection."); }
        }
        internal static string SQLCR_UnrecoverableClient {
              get { return SR.GetResourceString("SQLCR_UnrecoverableClient", @"The connection is broken and recovery is not possible.  The connection is marked by the client driver as unrecoverable.  No attempt was made to restore the connection."); }
        }
        internal static string SQLCR_NoCRAckAtReconnection {
              get { return SR.GetResourceString("SQLCR_NoCRAckAtReconnection", @"The server did not acknowledge a recovery attempt, connection recovery is not possible."); }
        }
        internal static string SQL_UnsupportedKeyword {
              get { return SR.GetResourceString("SQL_UnsupportedKeyword", @"The keyword '{0}' is not supported on this platform."); }
        }
        internal static string SQL_UnsupportedFeature {
              get { return SR.GetResourceString("SQL_UnsupportedFeature", @"The server is attempting to use a feature that is not supported on this platform."); }
        }
        internal static string SQL_UnsupportedToken {
              get { return SR.GetResourceString("SQL_UnsupportedToken", @"Received an unsupported token '{0}' while reading data from the server."); }
        }
        internal static string SQL_DbTypeNotSupportedOnThisPlatform {
              get { return SR.GetResourceString("SQL_DbTypeNotSupportedOnThisPlatform", @"Type {0} is not supported on this platform."); }
        }
        internal static string SQL_NetworkLibraryNotSupported {
              get { return SR.GetResourceString("SQL_NetworkLibraryNotSupported", @"The keyword 'Network Library' is not supported on this platform, prefix the 'Data Source' with the protocol desired instead ('tcp:' for a TCP connection, or 'np:' for a Named Pipe connection)."); }
        }
        internal static string SNI_PN0 {
              get { return SR.GetResourceString("SNI_PN0", @"HTTP Provider"); }
        }
        internal static string SNI_PN1 {
              get { return SR.GetResourceString("SNI_PN1", @"Named Pipes Provider"); }
        }
        internal static string SNI_PN2 {
              get { return SR.GetResourceString("SNI_PN2", @"Session Provider"); }
        }
        internal static string SNI_PN3 {
              get { return SR.GetResourceString("SNI_PN3", @"Sign Provider"); }
        }
        internal static string SNI_PN4 {
              get { return SR.GetResourceString("SNI_PN4", @"Shared Memory Provider"); }
        }
        internal static string SNI_PN5 {
              get { return SR.GetResourceString("SNI_PN5", @"SMux Provider"); }
        }
        internal static string SNI_PN6 {
              get { return SR.GetResourceString("SNI_PN6", @"SSL Provider"); }
        }
        internal static string SNI_PN7 {
              get { return SR.GetResourceString("SNI_PN7", @"TCP Provider"); }
        }
        internal static string SNI_PN8 {
              get { return SR.GetResourceString("SNI_PN8", @""); }
        }
        internal static string SNI_PN9 {
              get { return SR.GetResourceString("SNI_PN9", @"SQL Network Interfaces"); }
        }
        internal static string AZURESQL_GenericEndpoint {
              get { return SR.GetResourceString("AZURESQL_GenericEndpoint", @".database.windows.net"); }
        }
        internal static string AZURESQL_GermanEndpoint {
              get { return SR.GetResourceString("AZURESQL_GermanEndpoint", @".database.cloudapi.de"); }
        }
        internal static string AZURESQL_UsGovEndpoint {
              get { return SR.GetResourceString("AZURESQL_UsGovEndpoint", @".database.usgovcloudapi.net"); }
        }
        internal static string AZURESQL_ChinaEndpoint {
              get { return SR.GetResourceString("AZURESQL_ChinaEndpoint", @".database.chinacloudapi.cn"); }
        }
        internal static string net_nego_channel_binding_not_supported {
              get { return SR.GetResourceString("net_nego_channel_binding_not_supported", @"No support for channel binding on operating systems other than Windows."); }
        }
        internal static string net_gssapi_operation_failed_detailed {
              get { return SR.GetResourceString("net_gssapi_operation_failed_detailed", @"GSSAPI operation failed with error - {0} ({1})."); }
        }
        internal static string net_gssapi_operation_failed {
              get { return SR.GetResourceString("net_gssapi_operation_failed", @"GSSAPI operation failed with status: {0} (Minor status: {1})."); }
        }
        internal static string net_ntlm_not_possible_default_cred {
              get { return SR.GetResourceString("net_ntlm_not_possible_default_cred", @"NTLM authentication is not possible with default credentials on this platform."); }
        }
        internal static string net_nego_not_supported_empty_target_with_defaultcreds {
              get { return SR.GetResourceString("net_nego_not_supported_empty_target_with_defaultcreds", @"Target name should be non-empty if default credentials are passed."); }
        }
        internal static string net_nego_server_not_supported {
              get { return SR.GetResourceString("net_nego_server_not_supported", @"Server implementation is not supported."); }
        }
        internal static string net_nego_protection_level_not_supported {
              get { return SR.GetResourceString("net_nego_protection_level_not_supported", @"Requested protection level is not supported with the GSSAPI implementation currently installed."); }
        }
        internal static string net_context_buffer_too_small {
              get { return SR.GetResourceString("net_context_buffer_too_small", @"Insufficient buffer space. Required: {0} Actual: {1}."); }
        }
        internal static string net_auth_message_not_encrypted {
              get { return SR.GetResourceString("net_auth_message_not_encrypted", @"Protocol error: A received message contains a valid signature but it was not encrypted as required by the effective Protection Level."); }
        }
        internal static string net_securitypackagesupport {
              get { return SR.GetResourceString("net_securitypackagesupport", @"The requested security package is not supported."); }
        }
        internal static string net_log_operation_failed_with_error {
              get { return SR.GetResourceString("net_log_operation_failed_with_error", @"{0} failed with error {1}."); }
        }
        internal static string net_MethodNotImplementedException {
              get { return SR.GetResourceString("net_MethodNotImplementedException", @"This method is not implemented by this class."); }
        }
        internal static string event_OperationReturnedSomething {
              get { return SR.GetResourceString("event_OperationReturnedSomething", @"{0} returned {1}."); }
        }
        internal static string net_invalid_enum {
              get { return SR.GetResourceString("net_invalid_enum", @"The specified value is not valid in the '{0}' enumeration."); }
        }
        internal static string SSPIInvalidHandleType {
              get { return SR.GetResourceString("SSPIInvalidHandleType", @"'{0}' is not a supported handle type."); }
        }
        internal static string LocalDBNotSupported {
              get { return SR.GetResourceString("LocalDBNotSupported", @"LocalDB is not supported on this platform."); }
        }
        internal static string PlatformNotSupported_DataSqlClient {
              get { return SR.GetResourceString("PlatformNotSupported_DataSqlClient", @"System.Data.SqlClient is not supported on this platform."); }
        }
        internal static string SqlParameter_InvalidTableDerivedPrecisionForTvp {
              get { return SR.GetResourceString("SqlParameter_InvalidTableDerivedPrecisionForTvp", @"Precision '{0}' required to send all values in column '{1}' exceeds the maximum supported precision '{2}'. The values must all fit in a single precision."); }
        }
        internal static string SqlProvider_InvalidDataColumnMaxLength {
              get { return SR.GetResourceString("SqlProvider_InvalidDataColumnMaxLength", @"The size of column '{0}' is not supported. The size is {1}."); }
        }
        internal static string MDF_InvalidXmlInvalidValue {
              get { return SR.GetResourceString("MDF_InvalidXmlInvalidValue", @"The metadata XML is invalid. The {1} column of the {0} collection must contain a non-empty string."); }
        }
        internal static string MDF_CollectionNameISNotUnique {
              get { return SR.GetResourceString("MDF_CollectionNameISNotUnique", @"There are multiple collections named '{0}'."); }
        }
        internal static string MDF_InvalidXmlMissingColumn {
              get { return SR.GetResourceString("MDF_InvalidXmlMissingColumn", @"The metadata XML is invalid. The {0} collection must contain a {1} column and it must be a string column."); }
        }
        internal static string MDF_InvalidXml {
              get { return SR.GetResourceString("MDF_InvalidXml", @"The metadata XML is invalid."); }
        }
        internal static string MDF_NoColumns {
              get { return SR.GetResourceString("MDF_NoColumns", @"The schema table contains no columns."); }
        }
        internal static string MDF_QueryFailed {
              get { return SR.GetResourceString("MDF_QueryFailed", @"Unable to build the '{0}' collection because execution of the SQL query failed. See the inner exception for details."); }
        }
        internal static string MDF_TooManyRestrictions {
              get { return SR.GetResourceString("MDF_TooManyRestrictions", @"More restrictions were provided than the requested schema ('{0}') supports."); }
        }
        internal static string MDF_DataTableDoesNotExist {
              get { return SR.GetResourceString("MDF_DataTableDoesNotExist", @"The collection '{0}' is missing from the metadata XML."); }
        }
        internal static string MDF_UndefinedCollection {
              get { return SR.GetResourceString("MDF_UndefinedCollection", @"The requested collection ({0}) is not defined."); }
        }
        internal static string MDF_UnsupportedVersion {
              get { return SR.GetResourceString("MDF_UnsupportedVersion", @" requested collection ({0}) is not supported by this version of the provider."); }
        }
        internal static string MDF_MissingRestrictionColumn {
              get { return SR.GetResourceString("MDF_MissingRestrictionColumn", @"One or more of the required columns of the restrictions collection is missing."); }
        }
        internal static string MDF_MissingRestrictionRow {
              get { return SR.GetResourceString("MDF_MissingRestrictionRow", @"A restriction exists for which there is no matching row in the restrictions collection."); }
        }
        internal static string MDF_IncorrectNumberOfDataSourceInformationRows {
              get { return SR.GetResourceString("MDF_IncorrectNumberOfDataSourceInformationRows", @"The DataSourceInformation table must contain exactly one row."); }
        }
        internal static string MDF_MissingDataSourceInformationColumn {
              get { return SR.GetResourceString("MDF_MissingDataSourceInformationColumn", @"One of the required DataSourceInformation tables columns is missing."); }
        }
        internal static string MDF_AmbigousCollectionName {
              get { return SR.GetResourceString("MDF_AmbigousCollectionName", @"The collection name '{0}' matches at least two collections with the same name but with different case, but does not match any of them exactly."); }
        }
        internal static string MDF_UnableToBuildCollection {
              get { return SR.GetResourceString("MDF_UnableToBuildCollection", @"Unable to build schema collection '{0}';"); }
        }

#endif
        internal static Type ResourceType => typeof(SR);
    }
}