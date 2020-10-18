using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using Medella.TdsClient.Exceptions;

namespace Medella.TdsClient.Cleanup
{
    public class DbConnectionOptions
    {
        private readonly bool _hasPasswordKeyword;
        private readonly NameValuePair? _keyChain;
        private readonly Dictionary<string, string> _parseTable;

        private readonly string _usersConnectionString;
        // instances of this class are intended to be immutable, i.e readonly
        // used by pooling classes so it is much easier to verify correctness
        // when not worried about the class being modified during execution

        public DbConnectionOptions(string connectionString, Dictionary<string, string> synonyms)
        {
            _parseTable = new Dictionary<string, string>();
            _usersConnectionString = connectionString ?? "";

            // first pass on parsing, initial syntax check
            if (0 >= _usersConnectionString.Length) return;
            _keyChain = ParseInternal(_parseTable, _usersConnectionString, true, synonyms, false);
            _hasPasswordKeyword = _parseTable.ContainsKey(Key.Password) || _parseTable.ContainsKey(Synonym.Pwd);
        }

        protected DbConnectionOptions(DbConnectionOptions connectionOptions)
        {
            // Clone used by SqlConnectionString
            _usersConnectionString = connectionOptions._usersConnectionString;
            _hasPasswordKeyword = connectionOptions._hasPasswordKeyword;
            _parseTable = connectionOptions._parseTable;
            _keyChain = connectionOptions._keyChain;
        }

        internal bool HasPersisTablePassword => !_hasPasswordKeyword || ConvertValueToBoolean(Key.PersistSecurityInfo, false); // no password means persistable password so we don't have to munge

        public bool IsEmpty => _keyChain == null;

        public string UsersConnectionString(bool hidePassword) => UsersConnectionString(hidePassword, false);

        private string UsersConnectionString(bool hidePassword, bool forceHidePassword)
        {
            var connectionString = _usersConnectionString;
            if (_hasPasswordKeyword && (forceHidePassword || hidePassword && !HasPersisTablePassword)) connectionString = ReplacePasswordPwd();
            return connectionString ?? string.Empty;
        }

        public bool ConvertValueToBoolean(string keyName, bool defaultValue) => _parseTable.TryGetValue(keyName, out var value) ? ConvertValueToBooleanInternal(keyName, value) : defaultValue;

        internal static bool ConvertValueToBooleanInternal(string keyName, string stringValue)
        {
            if (CompareInsensitiveInvariant(stringValue, "true") || CompareInsensitiveInvariant(stringValue, "yes")) return true;
            if (CompareInsensitiveInvariant(stringValue, "false") || CompareInsensitiveInvariant(stringValue, "no")) return false;

            var tmp = stringValue.Trim(); // Remove leading & trailing whitespace.
            if (CompareInsensitiveInvariant(tmp, "true") || CompareInsensitiveInvariant(tmp, "yes")) return true;
            if (CompareInsensitiveInvariant(tmp, "false") || CompareInsensitiveInvariant(tmp, "no")) return false;
            throw ADP.InvalidConnectionOptionValue(keyName);
        }

        private static bool CompareInsensitiveInvariant(string strValue, string strConst) => 0 == StringComparer.OrdinalIgnoreCase.Compare(strValue, strConst);

        private static string GetKeyName(StringBuilder buffer)
        {
            var count = buffer.Length;
            while (0 < count && char.IsWhiteSpace(buffer[count - 1])) count--; // trailing whitespace
            return buffer.ToString(0, count).ToLower(CultureInfo.InvariantCulture);
        }

        private static string GetKeyValue(StringBuilder buffer, bool trimWhitespace)
        {
            var count = buffer.Length;
            var index = 0;
            if (!trimWhitespace)
                return buffer.ToString(index, count);
            while (index < count && char.IsWhiteSpace(buffer[index])) index++; // leading whitespace
            while (0 < count && char.IsWhiteSpace(buffer[count - 1])) count--; // trailing whitespace

            return buffer.ToString(index, count - index);
        }

        internal static int GetKeyValuePair(string connectionString, int currentPosition, StringBuilder buffer, bool useOdbcRules, out string? keyName, out string? keyValue)
        {
            var startPosition = currentPosition;

            buffer.Length = 0;
            keyName = null;
            keyValue = null;

            var currentChar = '\0';

            var parserState = ParserState.NothingYet;
            var length = connectionString.Length;
            for (; currentPosition < length; ++currentPosition)
            {
                currentChar = connectionString[currentPosition];

                switch (parserState)
                {
                    case ParserState.NothingYet: // [\\s;]*
                        if (';' == currentChar || char.IsWhiteSpace(currentChar)) continue;
                        if ('\0' == currentChar)
                        {
                            parserState = ParserState.NullTermination;
                            continue;
                        }

                        if (char.IsControl(currentChar)) throw ADP.ConnectionStringSyntax(startPosition);
                        startPosition = currentPosition;
                        if ('=' != currentChar)
                        {
                            parserState = ParserState.Key;
                            break;
                        }
                        else
                        {
                            parserState = ParserState.KeyEqual;
                            continue;
                        }

                    case ParserState.Key: // (?<key>([^=\\s\\p{Cc}]|\\s+[^=\\s\\p{Cc}]|\\s+==|==)+)
                        if ('=' == currentChar)
                        {
                            parserState = ParserState.KeyEqual;
                            continue;
                        }

                        if (char.IsWhiteSpace(currentChar)) break;
                        if (char.IsControl(currentChar)) throw ADP.ConnectionStringSyntax(startPosition);
                        break;

                    case ParserState.KeyEqual: // \\s*=(?!=)\\s*
                        if (!useOdbcRules && '=' == currentChar)
                        {
                            parserState = ParserState.Key;
                            break;
                        }

                        keyName = GetKeyName(buffer);
                        if (string.IsNullOrEmpty(keyName)) throw ADP.ConnectionStringSyntax(startPosition);
                        buffer.Length = 0;
                        parserState = ParserState.KeyEnd;
                        goto case ParserState.KeyEnd;

                    case ParserState.KeyEnd:
                        if (char.IsWhiteSpace(currentChar)) continue;
                        if (useOdbcRules)
                        {
                            if ('{' == currentChar)
                            {
                                parserState = ParserState.BraceQuoteValue;
                                break;
                            }
                        }
                        else
                        {
                            if ('\'' == currentChar)
                            {
                                parserState = ParserState.SingleQuoteValue;
                                continue;
                            }

                            if ('"' == currentChar)
                            {
                                parserState = ParserState.DoubleQuoteValue;
                                continue;
                            }
                        }

                        if (';' == currentChar) goto ParserExit;
                        if ('\0' == currentChar) goto ParserExit;
                        if (char.IsControl(currentChar)) throw ADP.ConnectionStringSyntax(startPosition);
                        parserState = ParserState.UnquotedValue;
                        break;

                    case ParserState.UnquotedValue: // "((?![\"'\\s])" + "([^;\\s\\p{Cc}]|\\s+[^;\\s\\p{Cc}])*" + "(?<![\"']))"
                        if (char.IsWhiteSpace(currentChar)) break;
                        if (char.IsControl(currentChar) || ';' == currentChar) goto ParserExit;
                        break;

                    case ParserState.DoubleQuoteValue: // "(\"([^\"\u0000]|\"\")*\")"
                        if ('"' == currentChar)
                        {
                            parserState = ParserState.DoubleQuoteValueQuote;
                            continue;
                        }

                        if ('\0' == currentChar) throw ADP.ConnectionStringSyntax(startPosition);
                        break;

                    case ParserState.DoubleQuoteValueQuote:
                        if ('"' == currentChar)
                        {
                            parserState = ParserState.DoubleQuoteValue;
                            break;
                        }

                        keyValue = GetKeyValue(buffer, false);
                        parserState = ParserState.QuotedValueEnd;
                        goto case ParserState.QuotedValueEnd;

                    case ParserState.SingleQuoteValue: // "('([^'\u0000]|'')*')"
                        if ('\'' == currentChar)
                        {
                            parserState = ParserState.SingleQuoteValueQuote;
                            continue;
                        }

                        if ('\0' == currentChar) throw ADP.ConnectionStringSyntax(startPosition);
                        break;

                    case ParserState.SingleQuoteValueQuote:
                        if ('\'' == currentChar)
                        {
                            parserState = ParserState.SingleQuoteValue;
                            break;
                        }

                        keyValue = GetKeyValue(buffer, false);
                        parserState = ParserState.QuotedValueEnd;
                        goto case ParserState.QuotedValueEnd;

                    case ParserState.BraceQuoteValue: // "(\\{([^\\}\u0000]|\\}\\})*\\})"
                        if ('}' == currentChar)
                        {
                            parserState = ParserState.BraceQuoteValueQuote;
                            break;
                        }

                        if ('\0' == currentChar) throw ADP.ConnectionStringSyntax(startPosition);
                        break;

                    case ParserState.BraceQuoteValueQuote:
                        if ('}' == currentChar)
                        {
                            parserState = ParserState.BraceQuoteValue;
                            break;
                        }

                        keyValue = GetKeyValue(buffer, false);
                        parserState = ParserState.QuotedValueEnd;
                        goto case ParserState.QuotedValueEnd;

                    case ParserState.QuotedValueEnd:
                        if (char.IsWhiteSpace(currentChar)) continue;
                        if (';' == currentChar) goto ParserExit;
                        if ('\0' == currentChar)
                        {
                            parserState = ParserState.NullTermination;
                            continue;
                        }

                        throw ADP.ConnectionStringSyntax(startPosition); // unbalanced single quote

                    case ParserState.NullTermination: // [\\s;\u0000]*
                        if ('\0' == currentChar) continue;
                        if (char.IsWhiteSpace(currentChar)) continue;
                        throw ADP.ConnectionStringSyntax(currentPosition);

                    default:
                        throw ADP.InternalError(ADP.InternalErrorCode.InvalidParserState1);
                }

                buffer.Append(currentChar);
            }

            ParserExit:
            switch (parserState)
            {
                case ParserState.Key:
                case ParserState.DoubleQuoteValue:
                case ParserState.SingleQuoteValue:
                case ParserState.BraceQuoteValue:
                    // keyword not found/unbalanced double/single quote
                    throw ADP.ConnectionStringSyntax(startPosition);

                case ParserState.KeyEqual:
                    // equal sign at end of line
                    keyName = GetKeyName(buffer);
                    if (string.IsNullOrEmpty(keyName)) throw ADP.ConnectionStringSyntax(startPosition);
                    break;

                case ParserState.UnquotedValue:
                    // unquoted value at end of line
                    keyValue = GetKeyValue(buffer, true);

                    var tmpChar = keyValue[^1];
                    if (!useOdbcRules && ('\'' == tmpChar || '"' == tmpChar)) throw ADP.ConnectionStringSyntax(startPosition); // unquoted value must not end in quote, except for odbc
                    break;

                case ParserState.DoubleQuoteValueQuote:
                case ParserState.SingleQuoteValueQuote:
                case ParserState.BraceQuoteValueQuote:
                case ParserState.QuotedValueEnd:
                    // quoted value at end of line
                    keyValue = GetKeyValue(buffer, false);
                    break;

                case ParserState.NothingYet:
                case ParserState.KeyEnd:
                case ParserState.NullTermination:
                    // do nothing
                    break;

                default:
                    throw ADP.InternalError(ADP.InternalErrorCode.InvalidParserState2);
            }

            if (';' == currentChar && currentPosition < connectionString.Length) currentPosition++;
            return currentPosition;
        }

        private static bool IsKeyNameValid(string? keyName) => !string.IsNullOrEmpty(keyName) && ';' != keyName[0] && !char.IsWhiteSpace(keyName[0]) && -1 == keyName.IndexOf('\u0000');

        private static NameValuePair? ParseInternal(Dictionary<string, string> parseTable, string connectionString, bool buildChain, Dictionary<string, string> synonyms, bool firstKey)
        {
            Debug.Assert(null != connectionString, "null connectionString");
            var buffer = new StringBuilder();
            NameValuePair? localKeychain = null;
            NameValuePair? keychain = null;
            var nextStartPosition = 0;
            var endPosition = connectionString.Length;
            while (nextStartPosition < endPosition)
            {
                var startPosition = nextStartPosition;

                nextStartPosition = GetKeyValuePair(connectionString, startPosition, buffer, firstKey, out var keyName, out var keyValue);
                if (string.IsNullOrEmpty(keyName)) break;
                var realKeyName = null != synonyms ? synonyms.TryGetValue(keyName, out var synonym) ? synonym : null : keyName;
                if (realKeyName == null || !IsKeyNameValid(realKeyName)) throw ADP.KeywordNotSupported(keyName);
                if (!firstKey || !parseTable.ContainsKey(realKeyName)) parseTable[realKeyName] = keyValue!; // last key-value pair wins (or first)

                if (null != localKeychain)
                    localKeychain = localKeychain.Next = new NameValuePair(realKeyName, keyValue!, nextStartPosition - startPosition);
                else if (buildChain) keychain = localKeychain = new NameValuePair(realKeyName, keyValue!, nextStartPosition - startPosition);
            }

            return keychain;
        }

        private string ReplacePasswordPwd()
        {
            var copyPosition = 0;
            var builder = new StringBuilder(_usersConnectionString.Length);
            for (var current = _keyChain; null != current; current = current.Next)
            {
                if (Key.Password != current.Name && Synonym.Pwd != current.Name) builder.Append(_usersConnectionString, copyPosition, current.Length);
                copyPosition += current.Length;
            }

            var conStr = builder.ToString();
            return conStr;
        }

        internal bool TryGetParseTableValue(string key, out string? value) => _parseTable.TryGetValue(key, out value);

        // same as Boolean, but with SSPI thrown in as valid yes
        public bool ConvertValueToIntegratedSecurity() => _parseTable.TryGetValue(Key.IntegratedSecurity, out var value) && value != null && ConvertValueToIntegratedSecurityInternal(value);

        private bool ConvertValueToIntegratedSecurityInternal(string stringValue)
        {
            if (CompareInsensitiveInvariant(stringValue, "sspi") || CompareInsensitiveInvariant(stringValue, "true") || CompareInsensitiveInvariant(stringValue, "yes")) return true;

            if (CompareInsensitiveInvariant(stringValue, "false") || CompareInsensitiveInvariant(stringValue, "no")) return false;

            var tmp = stringValue.Trim(); // Remove leading & trailing whitespace.
            if (CompareInsensitiveInvariant(tmp, "sspi") || CompareInsensitiveInvariant(tmp, "true") || CompareInsensitiveInvariant(tmp, "yes"))
                return true;
            if (CompareInsensitiveInvariant(tmp, "false") || CompareInsensitiveInvariant(tmp, "no"))
                return false;
            throw ADP.InvalidConnectionOptionValue(Key.IntegratedSecurity);
        }

        public int ConvertValueToInt32(string keyName, int defaultValue) => _parseTable.TryGetValue(keyName, out var value) && value != null ? ConvertToInt32Internal(keyName, value) : defaultValue;

        private static int ConvertToInt32Internal(string keyName, string stringValue)
        {
            try
            {
                return int.Parse(stringValue, NumberStyles.Integer, CultureInfo.InvariantCulture);
            }
            catch (FormatException e)
            {
                throw ADP.InvalidConnectionOptionValue(keyName, e);
            }
            catch (OverflowException e)
            {
                throw ADP.InvalidConnectionOptionValue(keyName, e);
            }
        }

        public string ConvertValueToString(string keyName, string defaultValue) => _parseTable.TryGetValue(keyName, out var value) && value != null ? value : defaultValue;

        public bool ContainsKey(string keyword) => _parseTable.ContainsKey(keyword);

        // connection string common keywords
        private static class Key
        {
            internal const string IntegratedSecurity = "integrated security";
            internal const string Password = "password";
            internal const string PersistSecurityInfo = "persist security info";
        }

        // known connection string common synonyms
        private static class Synonym
        {
            internal const string Pwd = "pwd";
        }

        // transition states used for parsing
        private enum ParserState
        {
            NothingYet = 1, //start point
            Key,
            KeyEqual,
            KeyEnd,
            UnquotedValue,
            DoubleQuoteValue,
            DoubleQuoteValueQuote,
            SingleQuoteValue,
            SingleQuoteValueQuote,
            BraceQuoteValue,
            BraceQuoteValueQuote,
            QuotedValueEnd,
            NullTermination
        }
    }
}