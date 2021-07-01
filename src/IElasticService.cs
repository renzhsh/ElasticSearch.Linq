using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Nest;

namespace ElasticSearch.Linq
{
    public interface IElasticService
    {
        /// <summary>
        /// 覆盖索引
        /// </summary>
        /// <param name="indexName"></param>
        void OverlapIndex(string indexName);

        bool DeleteIndex(string indexName);

        Task<bool> DeleteIndexAsync(string indexName);

        bool DeleteIndex<TDocument>() where TDocument : class;

        Task<bool> DeleteIndexAsync<TDocument>() where TDocument : class;

        IndexResponse IndexDoc<TDocument>(TDocument document) where TDocument : class;

        Task<IndexResponse> IndexDocAsync<TDocument>(TDocument document) where TDocument : class;

        BulkResponse IndexMany<TKey, TDocument>(IEnumerable<TDocument> docs) where TKey : IEquatable<TKey> where TDocument : class, IElasticEntity<TKey>;

        Task<BulkResponse> IndexManyAsync<TKey, TDocument>(IEnumerable<TDocument> docs) where TKey : IEquatable<TKey> where TDocument : class, IElasticEntity<TKey>;

        ElasticSearchable<TDocument> Search<TDocument>() where TDocument : class;
    }
}
