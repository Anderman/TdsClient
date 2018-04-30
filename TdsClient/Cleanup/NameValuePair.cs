using System.Diagnostics;
using Medella.TdsClient.Exceptions;

namespace Medella.TdsClient.Cleanup
{
    internal sealed class NameValuePair
    {
        private readonly int _length;
        private NameValuePair _next;

        internal NameValuePair(string name, string value, int length)
        {
            Debug.Assert(!string.IsNullOrEmpty(name), "empty keyname");
            Name = name;
            Value = value;
            _length = length;
        }

        internal int Length
        {
            get
            {
                Debug.Assert(0 < _length, "NameValuePair zero Length usage");
                return _length;
            }
        }

        internal string Name { get; }

        internal string Value { get; }

        internal NameValuePair Next
        {
            get => _next;
            set
            {
                if (null != _next || null == value) throw ADP.InternalError(ADP.InternalErrorCode.NameValuePairNext);

                _next = value;
            }
        }
    }
}