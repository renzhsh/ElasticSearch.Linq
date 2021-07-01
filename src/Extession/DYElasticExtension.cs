using Nest;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearch.Linq
{
    public static class DYElasticExtension
    {
        public static BulkResponse IndexMany<TDocument>(this IElasticService service, IEnumerable<TDocument> docs) where TDocument : class, IElasticEntity<long>
        {
            return service.IndexMany<long, TDocument>(docs);
        }

        public static Task<BulkResponse> IndexManyAsync<TDocument>(this IElasticService service, IEnumerable<TDocument> docs) where TDocument : class, IElasticEntity<long>
        {
            return service.IndexManyAsync<long, TDocument>(docs);
        }
    }
}
