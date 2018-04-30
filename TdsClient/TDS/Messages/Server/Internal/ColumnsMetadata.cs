using System.Diagnostics;

namespace Medella.TdsClient.TDS.Messages.Server.Internal
{
    public class ColumnsMetadata
    {
        private readonly ColumnMetadata[] _metaDataArray;

        internal ColumnsMetadata(int count)
        {
            _metaDataArray = new ColumnMetadata[count];
            for (var i = 0; i < _metaDataArray.Length; ++i) _metaDataArray[i] = new ColumnMetadata();
        }

        internal int Length => _metaDataArray.Length;

        internal ColumnMetadata this[int index]
        {
            get => _metaDataArray[index];
            set
            {
                Debug.Assert(null == value, "used only by SqlBulkCopy");
                _metaDataArray[index] = value;
            }
        }
    }
}