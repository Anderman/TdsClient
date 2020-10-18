using System;
using System.Collections;
using System.Data.Common;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;

namespace Medella.TdsClient.Exceptions
{
    public sealed class SqlException : DbException
    {
        private const int SqlExceptionHResult = unchecked((int)0x80131904);

        // Do not serialize this field! It is used to indicate that no reconnection attempts are required
        internal bool _doNotReconnect;

        private SqlErrorCollection _errors;

        private SqlException(string message, SqlErrorCollection errorCollection, Exception? innerException, Guid conId) : base(message, innerException)
        {
            HResult = SqlExceptionHResult;
            _errors = errorCollection;
            ClientConnectionId = conId;
        }

        private SqlException(SerializationInfo si, StreamingContext sc) : base(si, sc)
        {
            HResult = SqlExceptionHResult;
            foreach (var siEntry in si)
                if ("ClientConnectionId" == siEntry.Name)
                {
                    ClientConnectionId = (Guid)siEntry.Value;
                    break;
                }
        }

        // runtime will call even if private...
        public SqlErrorCollection Errors
        {
            get
            {
                if (_errors == null) _errors = new SqlErrorCollection();
                return _errors;
            }
        }

        public Guid ClientConnectionId { get; } = Guid.Empty;


        internal static SqlException CreateException(SqlErrorCollection errorCollection, string serverVersion) => CreateException(errorCollection, serverVersion, Guid.Empty);


        internal static SqlException CreateException(SqlErrorCollection errorCollection, string serverVersion, Guid conId, Exception innerException = null)
        {
            Debug.Assert(null != errorCollection && errorCollection.Count > 0, "no errorCollection?");

            var message = new StringBuilder();
            for (var i = 0; i < errorCollection.Count; i++)
            {
                if (i > 0) message.Append(Environment.NewLine);
                message.Append(errorCollection[i].Message);
            }

            var exception = new SqlException(message.ToString(), errorCollection, innerException, conId);

            return exception;
        }

        internal SqlException InternalClone()
        {
            var exception = new SqlException(Message, _errors, InnerException, ClientConnectionId);
            if (Data != null)
                foreach (DictionaryEntry entry in Data)
                    exception.Data.Add(entry.Key, entry.Value);
            exception._doNotReconnect = _doNotReconnect;
            return exception;
        }
    }
}