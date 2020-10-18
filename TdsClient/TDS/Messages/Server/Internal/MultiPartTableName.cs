namespace Medella.TdsClient.TDS.Messages.Server.Internal
{
    public class MultiPartTableName
    {
        internal string? ServerName { get; set; }

        internal string? CatalogName { get; set; }

        internal string? SchemaName { get; set; }

        internal string? TableName { get; set; }
    }
}