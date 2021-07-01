using System;
using System.Collections.Generic;
using System.Text;
using Elasticsearch.Net;
using Nest;

namespace ElasticSearch.Linq
{
    public interface IElasticClientFactory
    {
        /// <summary>
        /// 覆盖索引
        /// </summary>
        /// <param name="indexName"></param>
        void OverlapIndex(string indexName);

        string GetIndexName<TDocument>() where TDocument : class;

        IElasticClient GetClient(string indexName);

        IElasticClient GetClient<TDocument>() where TDocument : class;

        IElasticLowLevelClient GetLowLevelClient(string indexName);

        IElasticLowLevelClient LowLevelClient { get; }
    }
}
