using System.Diagnostics;

namespace Medella.TdsClient.TDS.Messages.Server.Internal
{
    public class ColumnsMetadata
    {
        private readonly ColumnMetadata[] _metaDataArray;

        public ColumnsMetadata(int count)
        {
            _metaDataArray = new ColumnMetadata[count];
            for (var i = 0; i < _metaDataArray.Length; ++i) _metaDataArray[i] = new ColumnMetadata();
        }


        internal int Length => _metaDataArray.Length;

        public ColumnMetadata this[int index] => _metaDataArray[index];
    }
}