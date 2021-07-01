using Elasticsearch.Net;
using Nest;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElasticSearch.Linq.Extession;

namespace ElasticSearch.Linq
{
    public class LowElasticService : IElasticService
    {
        private readonly IElasticClientFactory _clientFactory;

        public LowElasticService(IElasticClientFactory clientFactory)
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
            var client = _clientFactory.LowLevelClient;

            return client.Indices.Delete<DeleteIndexResponse>(indexName).Acknowledged;
        }

        public async Task<bool> DeleteIndexAsync(string indexName)
        {
            var client = _clientFactory.LowLevelClient;

            var resp = await client.Indices.DeleteAsync<DeleteIndexResponse>(indexName);

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
            var client = _clientFactory.LowLevelClient;

            var indexName = _clientFactory.GetIndexName<TDocument>();

            return client.IndexUsingType<IndexResponse>(indexName, typeof(TDocument).EsType(), JsonConvert.SerializeObject(document, Formatting.None));
        }

        public async Task<IndexResponse> IndexDocAsync<TDocument>(TDocument document) where TDocument : class
        {
            var client = _clientFactory.LowLevelClient;

            var indexName = _clientFactory.GetIndexName<TDocument>();

            return await client.IndexUsingTypeAsync<IndexResponse>(indexName, typeof(TDocument).EsType(), JsonConvert.SerializeObject(document, Formatting.None));
        }

        public BulkResponse IndexMany<TKey, TDocument>(IEnumerable<TDocument> docs)
            where TKey : IEquatable<TKey>
            where TDocument : class, IElasticEntity<TKey>
        {
            var client = _clientFactory.LowLevelClient;

            var indexName = _clientFactory.GetIndexName<TDocument>();

            return client.BulkUsingType<BulkResponse>(indexName, typeof(TDocument).EsType(), BuildBulkIndex<TKey, TDocument>(indexName, typeof(TDocument).EsType(), docs));
        }

        public async Task<BulkResponse> IndexManyAsync<TKey, TDocument>(IEnumerable<TDocument> docs)
            where TKey : IEquatable<TKey>
            where TDocument : class, IElasticEntity<TKey>
        {
            var client = _clientFactory.LowLevelClient;

            var indexName = _clientFactory.GetIndexName<TDocument>();

            return await client.BulkUsingTypeAsync<BulkResponse>(indexName, typeof(TDocument).EsType(), BuildBulkIndex<TKey, TDocument>(indexName, typeof(TDocument).EsType(), docs));
        }

        private string BuildBulkIndex<TKey, TDocument>(string index, string type, IEnumerable<TDocument> docs)
            where TKey : IEquatable<TKey>
            where TDocument : class, IElasticEntity<TKey>
        {
            StringBuilder sb = new StringBuilder();

            foreach (var doc in docs)
            {
                sb.AppendLine("{\"index\":{\"_index\":\"" + index + "\",\"_type\":\"" + type + "\",\"_id\":\"" + doc.Id + "\"}");
                sb.AppendLine(JsonConvert.SerializeObject(doc));
            }

            return sb.ToString();
        }

        #endregion

        public ElasticSearchable<TDocument> Search<TDocument>() where TDocument : class
        {
            return new ElasticSearchable<TDocument>(_clientFactory.LowLevelClient, _clientFactory.GetIndexName<TDocument>());
        }
    }
}
