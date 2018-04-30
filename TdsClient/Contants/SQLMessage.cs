using System;

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

        internal static string CultureIdError()
        {
            return SR.GetString(SR.SQL_CultureIdError);
        }

        internal static string EncryptionNotSupportedByClient()
        {
            return SR.GetString(SR.SQL_EncryptionNotSupportedByClient);
        }

        internal static string EncryptionNotSupportedByServer()
        {
            return SR.GetString(SR.SQL_EncryptionNotSupportedByServer);
        }

        internal static string OperationCancelled()
        {
            return SR.GetString(SR.SQL_OperationCancelled);
        }

        internal static string SevereError()
        {
            return SR.GetString(SR.SQL_SevereError);
        }

        internal static string SSPIInitializeError()
        {
            return SR.GetString(SR.SQL_SSPIInitializeError);
        }

        public static string SSPIGenerateError()
        {
            return SR.GetString(SR.SQL_SSPIGenerateError);
        }

        internal static string SqlServerBrowserNotAccessible()
        {
            return SR.GetString(SR.SQL_SqlServerBrowserNotAccessible);
        }

        public static string KerberosTicketMissingError()
        {
            return SR.GetString(SR.SQL_KerberosTicketMissingError);
        }

        internal static string Timeout()
        {
            return SR.GetString(SR.SQL_Timeout);
        }

        internal static string Timeout_PreLogin_Begin()
        {
            return SR.GetString(SR.SQL_Timeout_PreLogin_Begin);
        }

        internal static string Timeout_PreLogin_InitializeConnection()
        {
            return SR.GetString(SR.SQL_Timeout_PreLogin_InitializeConnection);
        }

        internal static string Timeout_PreLogin_SendHandshake()
        {
            return SR.GetString(SR.SQL_Timeout_PreLogin_SendHandshake);
        }

        internal static string Timeout_PreLogin_ConsumeHandshake()
        {
            return SR.GetString(SR.SQL_Timeout_PreLogin_ConsumeHandshake);
        }

        internal static string Timeout_Login_Begin()
        {
            return SR.GetString(SR.SQL_Timeout_Login_Begin);
        }

        internal static string Timeout_Login_ProcessConnectionAuth()
        {
            return SR.GetString(SR.SQL_Timeout_Login_ProcessConnectionAuth);
        }

        internal static string Timeout_PostLogin()
        {
            return SR.GetString(SR.SQL_Timeout_PostLogin);
        }

        internal static string Timeout_FailoverInfo()
        {
            return SR.GetString(SR.SQL_Timeout_FailoverInfo);
        }

        internal static string Timeout_RoutingDestination()
        {
            return SR.GetString(SR.SQL_Timeout_RoutingDestinationInfo);
        }

        internal static string Duration_PreLogin_Begin(long PreLoginBeginDuration)
        {
            return SR.GetString(SR.SQL_Duration_PreLogin_Begin, PreLoginBeginDuration);
        }

        internal static string Duration_PreLoginHandshake(long PreLoginBeginDuration, long PreLoginHandshakeDuration)
        {
            return SR.GetString(SR.SQL_Duration_PreLoginHandshake, PreLoginBeginDuration, PreLoginHandshakeDuration);
        }

        internal static string Duration_Login_Begin(long PreLoginBeginDuration, long PreLoginHandshakeDuration, long LoginBeginDuration)
        {
            return SR.GetString(SR.SQL_Duration_Login_Begin, PreLoginBeginDuration, PreLoginHandshakeDuration, LoginBeginDuration);
        }

        internal static string Duration_Login_ProcessConnectionAuth(long PreLoginBeginDuration, long PreLoginHandshakeDuration, long LoginBeginDuration, long LoginAuthDuration)
        {
            return SR.GetString(SR.SQL_Duration_Login_ProcessConnectionAuth, PreLoginBeginDuration, PreLoginHandshakeDuration, LoginBeginDuration, LoginAuthDuration);
        }

        internal static string Duration_PostLogin(long PreLoginBeginDuration, long PreLoginHandshakeDuration, long LoginBeginDuration, long LoginAuthDuration, long PostLoginDuration)
        {
            return SR.GetString(SR.SQL_Duration_PostLogin, PreLoginBeginDuration, PreLoginHandshakeDuration, LoginBeginDuration, LoginAuthDuration, PostLoginDuration);
        }

        internal static string UserInstanceFailure()
        {
            return SR.GetString(SR.SQL_UserInstanceFailure);
        }

        internal static string PreloginError()
        {
            return SR.GetString(SR.Snix_PreLogin);
        }

        internal static string ExClientConnectionId()
        {
            return SR.GetString(SR.SQL_ExClientConnectionId);
        }

        internal static string ExErrorNumberStateClass()
        {
            return SR.GetString(SR.SQL_ExErrorNumberStateClass);
        }

        internal static string ExOriginalClientConnectionId()
        {
            return SR.GetString(SR.SQL_ExOriginalClientConnectionId);
        }

        internal static string ExRoutingDestination()
        {
            return SR.GetString(SR.SQL_ExRoutingDestination);
        }
    }
}