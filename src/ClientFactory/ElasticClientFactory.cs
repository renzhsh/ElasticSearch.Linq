using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Linq;
using Elasticsearch.Net;
using Nest;
using Microsoft.Extensions.Options;

namespace ElasticSearch.Linq
{
    public class ElasticClientFactory : IElasticClientFactory
    {
        private readonly ESConfig _config;

        private readonly IConnectionPool _connectionPool;

        private readonly ConcurrentDictionary<string, IElasticClient> dict = new ConcurrentDictionary<string, IElasticClient>();

        public ElasticClientFactory(IOptions<ESConfig> options)
        {
            DCheck.NotNull(options, nameof(options));
            DCheck.NotNull(options.Value, nameof(options));

            ValidateConfig(options.Value);

            _config = options.Value;

            var urls = _config.Urls.Select(e => new Uri(e));
            //只请求正常节点，异常节点恢复后可以请求,默认随机请求服务  静态链接池，支持ping,适合小型集群
            _connectionPool = new StaticConnectionPool(urls)
            {
                SniffedOnStartup = true //开启链接池
            };

            var _connectionSettings = new ConnectionSettings(_connectionPool).DefaultIndex(_config.DefaultIndex);

            var _esClient = new ElasticClient(_connectionSettings);

            dict.TryAdd(_config.DefaultIndex, _esClient);

        }

        /// <summary>
        /// 覆盖索引
        /// </summary>
        /// <param name="indexName"></param>
        public void OverlapIndex(string indexName)
        {
            DCheck.NotNullOrEmpty(indexName, nameof(indexName));
            _config.DefaultIndex = indexName;
        }

        public string GetIndexName<TDocument>() where TDocument : class
        {
            if (IsCompatibleVersion())
            {
                return $"{_config.DefaultIndex}.{typeof(TDocument).FullName}".Replace(".", "_").ToLower();
            }
            else
            {
                return _config.DefaultIndex;
            }
        }

        public IElasticClient GetClient(string indexName)
        {
            DCheck.NotNullOrEmpty(indexName, nameof(indexName));
            return dict.GetOrAdd(indexName, (key) =>
            {
                var connectionSettings = new ConnectionSettings(_connectionPool).DefaultIndex(key);

                connectionSettings.DisableDirectStreaming(false);

                return new ElasticClient(connectionSettings);
            });
        }

        public IElasticClient GetClient<TDocument>() where TDocument : class
        {
            var indexName = GetIndexName<TDocument>();

            return GetClient(indexName);
        }

        public IElasticLowLevelClient GetLowLevelClient(string indexName)
        {
            DCheck.NotNullOrEmpty(indexName, nameof(indexName));
            return GetClient(indexName).LowLevel;
        }

        public IElasticLowLevelClient LowLevelClient { get => GetLowLevelClient(_config.DefaultIndex); }


        private void ValidateConfig(ESConfig config)
        {
            if (config.Urls == null || !config.Urls.Any())
            {
                throw new Exception("未指定ElasticSearch的Urls");
            }

            if (string.IsNullOrEmpty(config.DefaultIndex))
            {
                throw new Exception("未指定ElasticSearch的DefaultIndex");
            }
        }

        public bool IsCompatibleVersion()
        {
            if (string.IsNullOrEmpty(_config.EsVersion)) return true;

            try
            {
                var major = _config.EsVersion.Split(".").FirstOrDefault();
                return int.Parse(major) >= 7;
            }
            catch
            {
                return false;
            }
        }

    }
}
