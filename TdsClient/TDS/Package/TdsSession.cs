using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Medella.TdsClient.Exceptions;
using Medella.TdsClient.TDS.Messages.Client;
using Medella.TdsClient.TDS.Row.Reader.StringHelpers;

namespace Medella.TdsClient.TDS.Package
{
    public class TdsSession
    {
        private static readonly ConcurrentDictionary<int, Encoding> EncodingCache = new ConcurrentDictionary<int, Encoding>();
        public int DefaultCodePage { get; set; }
        public Encoding DefaultEncoding { get; set; }
        public SqlCollations DefaultCollation { get; set; }
        public bool DoNotPoolThisConnection { get; set; }
        public SessionData CurrentSessionData { get; set; } = new SessionData();
        public List<SqlInfoAndError> Errors { get; set; }= new List<SqlInfoAndError>();

        private int _cacheCodePage;
        private Encoding _cacheEncoding;
        public Encoding GetEncodingFromCache(int codePage)
        {
            if (_cacheCodePage == codePage)
                return _cacheEncoding;
            _cacheCodePage = codePage;
            _cacheEncoding = EncodingCache.GetOrAdd(codePage, x => Encoding.GetEncoding(codePage));
            return _cacheEncoding;
        }
    }
}