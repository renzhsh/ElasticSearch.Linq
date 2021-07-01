using Nest;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Elasticsearch.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ElasticSearch.Linq
{
    public class ElasticQueryResponse<T> where T : class
    {
        internal ElasticQueryResponse(ISearchResponse<T> response, int pageIndex, int pageSize)
        {
            if (response != null)
            {
                Documents = response.Documents;
                Total = response.Total;
            }

            PageIndex = pageIndex;
            PageSize = pageSize;
        }

        internal ElasticQueryResponse(StringResponse response, int pageIndex, int pageSize, string eql)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            Eql = eql;

            if (response.HttpStatusCode == 200)
            {
                var (resp, total) = ParseResponse(response.Body);

                Documents = resp;
                Total = total;
            }
            else
            {
                ErrMessage = ParseError(response.Body);
            }

        }

        public IReadOnlyCollection<T> Documents { get; }

        public long Total { get; }

        public int PageIndex { get; }

        public int PageSize { get; }

        public string Eql { get; }

        public string ErrMessage { get; } = "";


        private (IReadOnlyCollection<T>, long) ParseResponse(string content)
        {
            try
            {
                JObject resp = JObject.Parse(content);

                var hits = resp["hits"];
                long total = hits["total"].Value<long>();

                var docs = ((JArray)hits["hits"]).Select(item => item["_source"].ToObject<T>());

                return (new List<T>(docs), total);

            }
            catch (Exception)
            {
                return (new List<T>(), 0);
            }
        }

        private string ParseError(string content)
        {
            try
            {
                JObject resp = JObject.Parse(content);

                return resp["error"]["reason"].Value<string>();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
