using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Nest;
using System.Linq.Expressions;

namespace ElasticSearch.Linq
{
    public class ElasticService : IElasticService
    {
        private readonly IElasticClientFactory _clientFactory;

        public ElasticService(IElasticClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }
        #region "Index"
        public void OverlapIndex(string indexName)
        {
            DCheck.NotNullOrEmpty(indexName, nameof(indexName));
            _clientFactory.OverlapIndex(indexName);
        }

        public bool DeleteIndex(string indexName)
        {
            DCheck.NotNullOrEmpty(indexName, nameof(indexName));
            var client = _clientFactory.GetClient(indexName);

            return client.Indices.Delete(Indices.Parse(indexName)).Acknowledged;
        }

        public async Task<bool> DeleteIndexAsync(string indexName)
        {
            DCheck.NotNullOrEmpty(indexName, nameof(indexName));
            var client = _clientFactory.GetClient(indexName);
            var resp = await client.Indices.DeleteAsync(Indices.Parse(indexName));

            return resp.Acknowledged;
        }

        public bool DeleteIndex<TDocument>() where TDocument : class
        {
            var indexName = _clientFactory.GetIndexName<TDocument>();
            return DeleteIndex(indexName);
        }

        public Task<bool> DeleteIndexAsync<TDocument>() where TDocument : class
        {
            var indexName = _clientFactory.GetIndexName<TDocument>();
            return DeleteIndexAsync(indexName);
        }

        #endregion

        #region "IndexDoc"
        public IndexResponse IndexDoc<TDocument>(TDocument document) where TDocument : class
        {
            var client = _clientFactory.GetClient<TDocument>();

            return client.IndexDocument(document);
        }

        public async Task<IndexResponse> IndexDocAsync<TDocument>(TDocument document) where TDocument : class
        {
            var client = _clientFactory.GetClient<TDocument>();
            return await client.IndexDocumentAsync(document);
        }

        public BulkResponse IndexMany<TKey, TDocument>(IEnumerable<TDocument> docs)
            where TKey : IEquatable<TKey>
            where TDocument : class, IElasticEntity<TKey>
        {
            var client = _clientFactory.GetClient<TDocument>();
            return client.IndexMany(docs);
        }

        public async Task<BulkResponse> IndexManyAsync<TKey, TDocument>(IEnumerable<TDocument> docs)
            where TKey : IEquatable<TKey>
            where TDocument : class, IElasticEntity<TKey>
        {
            var client = _clientFactory.GetClient<TDocument>();
            return await client.IndexManyAsync(docs);
        }

        #endregion

        public ElasticSearchable<TDocument> Search<TDocument>() where TDocument : class
        {
            return new ElasticSearchable<TDocument>(_clientFactory.GetClient<TDocument>());
        }
    }
}
