namespace Medella.TdsClient.TDS.Package
{
    public class TdsRow
    {
        public bool IsNbcRow { get; set; }
        public byte[] NbcBitmap { get; set; }

        public bool IsNull(int columnOrdinal)
        {
            var testBit = (byte)(1 << (columnOrdinal & 0x7)); // columnOrdinal & 0x7 == columnOrdinal MOD 0x7
            return (testBit & NbcBitmap[columnOrdinal >> 3]) != 0;
        }
    }
}