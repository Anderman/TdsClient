using System;
using Medella.TdsClient.Contants;

namespace Medella.TdsClient.TDS
{
    public class SqlInfoAndError
    {
        public string Source { get; set; } = TdsEnums.SQL_PROVIDER_NAME;

        public int Number { get; set; }

        public byte State { get; set; }

        public byte Class { get; set; }

        public string Server { get; set; }

        public string Message { get; set; }

        public string Procedure { get; set; }

        public int LineNumber { get; set; }

        public int Win32ErrorCode { get; set; }

        public Exception Exception { get; set; }

        // There is no exception stack included because the correct exception stack is only available 
        // on SqlException, and to obtain that the SqlError would have to have backpointers all the
        // way back to SqlException.  If the user needs a call stack, they can obtain it on SqlException.
        public override string ToString()
        {
            return $"number:{Number}, state:{State}, errorClass:{Class}, server:{Server}, message:{Message}, procedure:{Procedure}, line:{LineNumber}";
        }
    }
}