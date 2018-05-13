using Medella.TdsClient.TDS.Controller;

namespace Medella.TdsClient.TDS
{
    public class OpenTransaction
    {
        public TdsPhysicalConnection Connection;
        public long SqlTransactionId;

        public OpenTransaction(TdsPhysicalConnection cnn, long sqlTransactionId)
        {
            Connection = cnn;
            SqlTransactionId = sqlTransactionId;
        }
    }
}