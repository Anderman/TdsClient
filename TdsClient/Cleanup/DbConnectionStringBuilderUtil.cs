using System;
using System.Diagnostics;
using System.Reflection;
using Medella.TdsClient.Constants;
using Medella.TdsClient.Exceptions;

namespace Medella.TdsClient.Cleanup
{
    internal static class DbConnectionStringBuilderUtil
    {
        private const string ApplicationIntentReadWriteString = "ReadWrite";
        private const string ApplicationIntentReadOnlyString = "ReadOnly";

        internal static bool ConvertToBoolean(object value)
        {
            Debug.Assert(null != value, "ConvertToBoolean(null)");
            if (value is string sValue)
            {
                if (StringComparer.OrdinalIgnoreCase.Equals(sValue, "true") || StringComparer.OrdinalIgnoreCase.Equals(sValue, "yes")) return true;

                if (StringComparer.OrdinalIgnoreCase.Equals(sValue, "false") || StringComparer.OrdinalIgnoreCase.Equals(sValue, "no")) return false;

                var tmp = sValue.Trim(); // Remove leading & trailing whitespace.
                if (StringComparer.OrdinalIgnoreCase.Equals(tmp, "true") || StringComparer.OrdinalIgnoreCase.Equals(tmp, "yes"))
                    return true;
                if (StringComparer.OrdinalIgnoreCase.Equals(tmp, "false") || StringComparer.OrdinalIgnoreCase.Equals(tmp, "no"))
                    return false;
                return bool.Parse(sValue);
            }

            try
            {
                return Convert.ToBoolean(value);
            }
            catch (InvalidCastException e)
            {
                throw ADP.ConvertFailed(value.GetType(), typeof(bool), e);
            }
        }

        internal static bool ConvertToIntegratedSecurity(object value)
        {
            Debug.Assert(null != value, "ConvertToIntegratedSecurity(null)");
            if (value is string sValue)
            {
                if (StringComparer.OrdinalIgnoreCase.Equals(sValue, "sspi") || StringComparer.OrdinalIgnoreCase.Equals(sValue, "true") || StringComparer.OrdinalIgnoreCase.Equals(sValue, "yes")) return true;

                if (StringComparer.OrdinalIgnoreCase.Equals(sValue, "false") || StringComparer.OrdinalIgnoreCase.Equals(sValue, "no")) return false;

                var tmp = sValue.Trim(); // Remove leading & trailing whitespace.
                if (StringComparer.OrdinalIgnoreCase.Equals(tmp, "sspi") || StringComparer.OrdinalIgnoreCase.Equals(tmp, "true") || StringComparer.OrdinalIgnoreCase.Equals(tmp, "yes"))
                    return true;
                if (StringComparer.OrdinalIgnoreCase.Equals(tmp, "false") || StringComparer.OrdinalIgnoreCase.Equals(tmp, "no"))
                    return false;
                return bool.Parse(sValue);
            }

            try
            {
                return Convert.ToBoolean(value);
            }
            catch (InvalidCastException e)
            {
                throw ADP.ConvertFailed(value.GetType(), typeof(bool), e);
            }
        }

        internal static int ConvertToInt32(object value)
        {
            try
            {
                return Convert.ToInt32(value);
            }
            catch (InvalidCastException e)
            {
                throw ADP.ConvertFailed(value.GetType(), typeof(int), e);
            }
        }

        internal static string? ConvertToString(object value)
        {
            try
            {
                return Convert.ToString(value);
            }
            catch (InvalidCastException e)
            {
                throw ADP.ConvertFailed(value.GetType(), typeof(string), e);
            }
        }

        internal static bool TryConvertToApplicationIntent(string value, out ApplicationIntent result)
        {
            Debug.Assert(Enum.GetNames(typeof(ApplicationIntent)).Length == 2, "ApplicationIntent enum has changed, update needed");
            Debug.Assert(null != value, "TryConvertToApplicationIntent(null,...)");

            if (StringComparer.OrdinalIgnoreCase.Equals(value, ApplicationIntentReadOnlyString))
            {
                result = ApplicationIntent.ReadOnly;
                return true;
            }

            if (StringComparer.OrdinalIgnoreCase.Equals(value, ApplicationIntentReadWriteString))
            {
                result = ApplicationIntent.ReadWrite;
                return true;
            }

            result = DbConnectionStringDefaults.ApplicationIntent;
            return false;
        }

        internal static bool IsValidApplicationIntentValue(ApplicationIntent value)
        {
            Debug.Assert(Enum.GetNames(typeof(ApplicationIntent)).Length == 2, "ApplicationIntent enum has changed, update needed");
            return value == ApplicationIntent.ReadOnly || value == ApplicationIntent.ReadWrite;
        }

        internal static string ApplicationIntentToString(ApplicationIntent value)
        {
            Debug.Assert(IsValidApplicationIntentValue(value));
            if (value == ApplicationIntent.ReadOnly)
                return ApplicationIntentReadOnlyString;
            return ApplicationIntentReadWriteString;
        }

        /// <summary>
        ///     This method attempts to convert the given value tp ApplicationIntent enum. The algorithm is:
        ///     * if the value is from type string, it will be matched against ApplicationIntent enum names only, using ordinal,
        ///     case-insensitive comparer
        ///     * if the value is from type ApplicationIntent, it will be used as is
        ///     * if the value is from integral type (SByte, Int16, Int32, Int64, Byte, UInt16, UInt32, or UInt64), it will be
        ///     converted to enum
        ///     * if the value is another enum or any other type, it will be blocked with an appropriate ArgumentException
        ///     in any case above, if the converted value is out of valid range, the method raises ArgumentOutOfRangeException.
        /// </summary>
        /// <returns>application intent value in the valid range</returns>
        internal static ApplicationIntent ConvertToApplicationIntent(string keyword, object value)
        {
            Debug.Assert(null != value, "ConvertToApplicationIntent(null)");
            if (value is string sValue)
            {
                // We could use Enum.TryParse<ApplicationIntent> here, but it accepts value combinations like
                // "ReadOnly, ReadWrite" which are unwelcome here
                // Also, Enum.TryParse is 100x slower than plain StringComparer.OrdinalIgnoreCase.Equals method.

                if (TryConvertToApplicationIntent(sValue, out var result)) return result;

                // try again after remove leading & trailing whitespace.
                sValue = sValue.Trim();
                if (TryConvertToApplicationIntent(sValue, out result)) return result;

                // string values must be valid
                throw ADP.InvalidConnectionOptionValue(keyword);
            }

            // the value is not string, try other options
            ApplicationIntent eValue;

            if (value is ApplicationIntent intent)
                eValue = intent;
            else if (value.GetType().GetTypeInfo().IsEnum)
                throw ADP.ConvertFailed(value.GetType(), typeof(ApplicationIntent), null);
            else
                try
                {
                    // Enum.ToObject allows only integral and enum values (enums are blocked above), raising ArgumentException for the rest
                    eValue = (ApplicationIntent)Enum.ToObject(typeof(ApplicationIntent), value);
                }
                catch (ArgumentException e)
                {
                    // to be consistent with the messages we send in case of wrong type usage, replace 
                    // the error with our exception, and keep the original one as inner one for troubleshooting
                    throw ADP.ConvertFailed(value.GetType(), typeof(ApplicationIntent), e);
                }

            // ensure value is in valid range
            if (IsValidApplicationIntentValue(eValue))
                return eValue;
            throw ADP.InvalidEnumerationValue(typeof(ApplicationIntent), (int)eValue);
        }
    }
}