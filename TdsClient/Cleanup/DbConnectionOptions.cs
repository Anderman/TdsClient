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
        private readonly NameValuePair _keyChain;
        private readonly Dictionary<string, string> _parsetable;

        private readonly string _usersConnectionString;
        // instances of this class are intended to be immutable, i.e readonly
        // used by pooling classes so it is much easier to verify correctness
        // when not worried about the class being modified during execution

        public DbConnectionOptions(string connectionString, Dictionary<string, string> synonyms)
        {
            _parsetable = new Dictionary<string, string>();
            _usersConnectionString = connectionString ?? "";

            // first pass on parsing, initial syntax check
            if (0 >= _usersConnectionString.Length) return;
            _keyChain = ParseInternal(_parsetable, _usersConnectionString, true, synonyms, false);
            _hasPasswordKeyword = _parsetable.ContainsKey(Key.Password) || _parsetable.ContainsKey(Synonym.Pwd);
        }

        protected DbConnectionOptions(DbConnectionOptions connectionOptions)
        {
            // Clone used by SqlConnectionString
            _usersConnectionString = connectionOptions._usersConnectionString;
            _hasPasswordKeyword = connectionOptions._hasPasswordKeyword;
            _parsetable = connectionOptions._parsetable;
            _keyChain = connectionOptions._keyChain;
        }

        internal bool HasPersistablePassword => !_hasPasswordKeyword || ConvertValueToBoolean(Key.PersistSecurityInfo, false); // no password means persistable password so we don't have to munge

        public bool IsEmpty => _keyChain == null;

        public string UsersConnectionString(bool hidePassword)
        {
            return UsersConnectionString(hidePassword, false);
        }

        private string UsersConnectionString(bool hidePassword, bool forceHidePassword)
        {
            var connectionString = _usersConnectionString;
            if (_hasPasswordKeyword && (forceHidePassword || hidePassword && !HasPersistablePassword)) connectionString = ReplacePasswordPwd();
            return connectionString ?? string.Empty;
        }

        public bool ConvertValueToBoolean(string keyName, bool defaultValue)
        {
            return _parsetable.TryGetValue(keyName, out var value) ? ConvertValueToBooleanInternal(keyName, value) : defaultValue;
        }

        internal static bool ConvertValueToBooleanInternal(string keyName, string stringValue)
        {
            if (CompareInsensitiveInvariant(stringValue, "true") || CompareInsensitiveInvariant(stringValue, "yes")) return true;
            if (CompareInsensitiveInvariant(stringValue, "false") || CompareInsensitiveInvariant(stringValue, "no")) return false;

            var tmp = stringValue.Trim(); // Remove leading & trailing whitespace.
            if (CompareInsensitiveInvariant(tmp, "true") || CompareInsensitiveInvariant(tmp, "yes")) return true;
            if (CompareInsensitiveInvariant(tmp, "false") || CompareInsensitiveInvariant(tmp, "no")) return false;
            throw ADP.InvalidConnectionOptionValue(keyName);
        }

        private static bool CompareInsensitiveInvariant(string strvalue, string strconst)
        {
            return 0 == StringComparer.OrdinalIgnoreCase.Compare(strvalue, strconst);
        }

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
                return buffer.ToString(index, count - index);
            while (index < count && char.IsWhiteSpace(buffer[index])) index++; // leading whitespace
            while (0 < count && char.IsWhiteSpace(buffer[count - 1])) count--; // trailing whitespace

            return buffer.ToString(index, count - index);
        }

        internal static int GetKeyValuePair(string connectionString, int currentPosition, StringBuilder buffer, bool useOdbcRules, out string keyname, out string keyvalue)
        {
            var startposition = currentPosition;

            buffer.Length = 0;
            keyname = null;
            keyvalue = null;

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

                        if (char.IsControl(currentChar)) throw ADP.ConnectionStringSyntax(startposition);
                        startposition = currentPosition;
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
                        if (char.IsControl(currentChar)) throw ADP.ConnectionStringSyntax(startposition);
                        break;

                    case ParserState.KeyEqual: // \\s*=(?!=)\\s*
                        if (!useOdbcRules && '=' == currentChar)
                        {
                            parserState = ParserState.Key;
                            break;
                        }

                        keyname = GetKeyName(buffer);
                        if (string.IsNullOrEmpty(keyname)) throw ADP.ConnectionStringSyntax(startposition);
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
                        if (char.IsControl(currentChar)) throw ADP.ConnectionStringSyntax(startposition);
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

                        if ('\0' == currentChar) throw ADP.ConnectionStringSyntax(startposition);
                        break;

                    case ParserState.DoubleQuoteValueQuote:
                        if ('"' == currentChar)
                        {
                            parserState = ParserState.DoubleQuoteValue;
                            break;
                        }

                        keyvalue = GetKeyValue(buffer, false);
                        parserState = ParserState.QuotedValueEnd;
                        goto case ParserState.QuotedValueEnd;

                    case ParserState.SingleQuoteValue: // "('([^'\u0000]|'')*')"
                        if ('\'' == currentChar)
                        {
                            parserState = ParserState.SingleQuoteValueQuote;
                            continue;
                        }

                        if ('\0' == currentChar) throw ADP.ConnectionStringSyntax(startposition);
                        break;

                    case ParserState.SingleQuoteValueQuote:
                        if ('\'' == currentChar)
                        {
                            parserState = ParserState.SingleQuoteValue;
                            break;
                        }

                        keyvalue = GetKeyValue(buffer, false);
                        parserState = ParserState.QuotedValueEnd;
                        goto case ParserState.QuotedValueEnd;

                    case ParserState.BraceQuoteValue: // "(\\{([^\\}\u0000]|\\}\\})*\\})"
                        if ('}' == currentChar)
                        {
                            parserState = ParserState.BraceQuoteValueQuote;
                            break;
                        }

                        if ('\0' == currentChar) throw ADP.ConnectionStringSyntax(startposition);
                        break;

                    case ParserState.BraceQuoteValueQuote:
                        if ('}' == currentChar)
                        {
                            parserState = ParserState.BraceQuoteValue;
                            break;
                        }

                        keyvalue = GetKeyValue(buffer, false);
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

                        throw ADP.ConnectionStringSyntax(startposition); // unbalanced single quote

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
                    throw ADP.ConnectionStringSyntax(startposition);

                case ParserState.KeyEqual:
                    // equal sign at end of line
                    keyname = GetKeyName(buffer);
                    if (string.IsNullOrEmpty(keyname)) throw ADP.ConnectionStringSyntax(startposition);
                    break;

                case ParserState.UnquotedValue:
                    // unquoted value at end of line
                    keyvalue = GetKeyValue(buffer, true);

                    var tmpChar = keyvalue[keyvalue.Length - 1];
                    if (!useOdbcRules && ('\'' == tmpChar || '"' == tmpChar)) throw ADP.ConnectionStringSyntax(startposition); // unquoted value must not end in quote, except for odbc
                    break;

                case ParserState.DoubleQuoteValueQuote:
                case ParserState.SingleQuoteValueQuote:
                case ParserState.BraceQuoteValueQuote:
                case ParserState.QuotedValueEnd:
                    // quoted value at end of line
                    keyvalue = GetKeyValue(buffer, false);
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

        private static bool IsKeyNameValid(string keyname)
        {
            return !string.IsNullOrEmpty(keyname) && ';' != keyname[0] && !char.IsWhiteSpace(keyname[0]) && -1 == keyname.IndexOf('\u0000');
        }

        private static NameValuePair ParseInternal(Dictionary<string, string> parsetable, string connectionString, bool buildChain, Dictionary<string, string> synonyms, bool firstKey)
        {
            Debug.Assert(null != connectionString, "null connectionstring");
            var buffer = new StringBuilder();
            NameValuePair localKeychain = null, keychain = null;
            var nextStartPosition = 0;
            var endPosition = connectionString.Length;
            while (nextStartPosition < endPosition)
            {
                var startPosition = nextStartPosition;

                nextStartPosition = GetKeyValuePair(connectionString, startPosition, buffer, firstKey, out var keyname, out var keyvalue);
                if (string.IsNullOrEmpty(keyname)) break;
                var realkeyname = null != synonyms ? (synonyms.TryGetValue(keyname, out var synonym) ? synonym : null) : keyname;
                if (!IsKeyNameValid(realkeyname)) throw ADP.KeywordNotSupported(keyname);
                if (!firstKey || !parsetable.ContainsKey(realkeyname)) parsetable[realkeyname] = keyvalue; // last key-value pair wins (or first)

                if (null != localKeychain)
                    localKeychain = localKeychain.Next = new NameValuePair(realkeyname, keyvalue, nextStartPosition - startPosition);
                else if (buildChain) keychain = localKeychain = new NameValuePair(realkeyname, keyvalue, nextStartPosition - startPosition);
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

            var constr = builder.ToString();
            return constr;
        }

        internal bool TryGetParsetableValue(string key, out string value)
        {
            return _parsetable.TryGetValue(key, out value);
        }

        // same as Boolean, but with SSPI thrown in as valid yes
        public bool ConvertValueToIntegratedSecurity()
        {
            return _parsetable.TryGetValue(Key.IntegratedSecurity, out var value) && value != null && ConvertValueToIntegratedSecurityInternal(value);
        }

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

        public int ConvertValueToInt32(string keyName, int defaultValue)
        {
            return _parsetable.TryGetValue(keyName, out var value) && value != null ? ConvertToInt32Internal(keyName, value) : defaultValue;
        }

        private static int ConvertToInt32Internal(string keyname, string stringValue)
        {
            try
            {
                return int.Parse(stringValue, NumberStyles.Integer, CultureInfo.InvariantCulture);
            }
            catch (FormatException e)
            {
                throw ADP.InvalidConnectionOptionValue(keyname, e);
            }
            catch (OverflowException e)
            {
                throw ADP.InvalidConnectionOptionValue(keyname, e);
            }
        }

        public string ConvertValueToString(string keyName, string defaultValue)
        {
            return _parsetable.TryGetValue(keyName, out var value) && value != null ? value : defaultValue;
        }

        public bool ContainsKey(string keyword)
        {
            return _parsetable.ContainsKey(keyword);
        }

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