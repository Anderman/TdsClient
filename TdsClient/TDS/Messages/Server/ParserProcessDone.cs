using Medella.TdsClient.Contants;
using Medella.TdsClient.TDS.Package;
using Medella.TdsClient.TDS.Package.Reader;

namespace Medella.TdsClient.TDS.Messages.Server
{
    public static class ParserProcessDone
    {
        public static bool SqlDone(this TdsPackageReader reader)
        {
            // status
            // command
            // rowcount (valid only if DONE_COUNT bit is set)
            bool attentionReceived;
            var receivedColMetaData = false;
            var status = reader.ReadUInt16();
            var curCmd = reader.ReadUInt16();
            var count = (int) reader.ReadInt64();


            // We get a done token with the attention bit set
            if (TdsEnums.DONE_ATTN == (status & TdsEnums.DONE_ATTN)) attentionReceived = true;

            //if (cmd != null && TdsEnums.DONE_COUNT == (status & TdsEnums.DONE_COUNT))
            //{
            //    if (curCmd != TdsEnums.SELECT) cmd.InternalRecordsAffected = count;
            //    // Skip the bogus DONE counts sent by the server
            //    if (curCmd != TdsEnums.SELECT) cmd.OnStatementCompleted(count);
            //}


            // Surface exception for DONE_ERROR in the case we did not receive an error token
            // in the stream, but an error occurred.  In these cases, we throw a general server error.  The
            // situations where this can occur are: an invalid buffer received from client, login error
            // and the server refused our connection, and the case where we are trying to log in but
            // the server has reached its max connection limit.  Bottom line, we need to throw general
            // error in the cases where we did not receive an error token along with the DONE_ERROR.
            if (TdsEnums.DONE_ERROR == (TdsEnums.DONE_ERROR & status))
            {
            }

            // Similar to above, only with a more severe error.  In this case, if we received
            // the done_srverror, this exception will be added to the collection regardless.
            // The server will always break the connection in this case.
            if (TdsEnums.DONE_SRVERROR == (TdsEnums.DONE_SRVERROR & status))
            {
            }


            // stop if the DONE_MORE bit isn't set (see above for attention handling)
            var _pendingData = false;
            var _hasOpenResult = false;
            if (TdsEnums.DONE_MORE != (status & TdsEnums.DONE_MORE)) return true;

            // _pendingData set by e.g. 'TdsExecuteSQLBatch'
            // _hasOpenResult always set to true by 'WriteMarsHeader'
            //
            if (!_pendingData && _hasOpenResult) ;
            return false;
        }
    }
}