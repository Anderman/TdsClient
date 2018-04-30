using System;

namespace Medella.TdsClient.TDS.Messages.Server.Internal
{
    public struct MultiPartTableName
    {
        private string _multipartName;
        private string _serverName;
        private string _catalogName;
        private string _schemaName;
        private string _tableName;

        internal MultiPartTableName(string[] parts)
        {
            _multipartName = null;
            _serverName = parts[0];
            _catalogName = parts[1];
            _schemaName = parts[2];
            _tableName = parts[3];
        }

        internal MultiPartTableName(string multipartName)
        {
            _multipartName = multipartName;
            _serverName = null;
            _catalogName = null;
            _schemaName = null;
            _tableName = null;
        }

        internal string ServerName
        {
            get
            {
                ParseMultipartName();
                return _serverName;
            }
            set => _serverName = value;
        }

        internal string CatalogName
        {
            get
            {
                ParseMultipartName();
                return _catalogName;
            }
            set => _catalogName = value;
        }

        internal string SchemaName
        {
            get
            {
                ParseMultipartName();
                return _schemaName;
            }
            set => _schemaName = value;
        }

        internal string TableName
        {
            get
            {
                ParseMultipartName();
                return _tableName;
            }
            set => _tableName = value;
        }

        private void ParseMultipartName()
        {
            if (null != _multipartName)
            {
                var parts = MultipartIdentifier.ParseMultipartIdentifier(_multipartName, "[\"", "]\"", SR.SQL_TDSParserTableName, false);
                _serverName = parts[0];
                _catalogName = parts[1];
                _schemaName = parts[2];
                _tableName = parts[3];
                _multipartName = null;
            }
        }

        internal static readonly MultiPartTableName Null = new MultiPartTableName(new string[] {null, null, null, null});
    }
}