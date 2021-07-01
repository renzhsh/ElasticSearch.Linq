using Elasticsearch.Net;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ElasticSearch.Linq.Extession;
using Elasticsearch;
using Nest;

namespace ElasticSearch.Linq
{
    public class LowLevelExpressionBuilder<T> : IExpressionBuilder<T> where T : class
    {
        public LowLevelExpressionBuilder(IElasticLowLevelClient client, ElasticSearchable<T> search, string indexName)
        {
            Client = client;
            Searchable = search;
            IndexName = indexName;
        }
        private ElasticSearchable<T> Searchable { get; }

        private IElasticLowLevelClient Client { get; }

        private string IndexName { get; }

        public ElasticQueryResponse<T> ToList()
        {
            var eql = BuildSearchExpression().ToString(Formatting.None);

            var resp = Client.SearchUsingType<StringResponse>(IndexName, typeof(T).Name.ToLower(), eql);

            return new ElasticQueryResponse<T>(resp, Searchable._pageIndex, Searchable._pageSize, eql);
        }

        public async Task<ElasticQueryResponse<T>> ToListAsync()
        {
            var eql = BuildSearchExpression().ToString();
            var resp = await Client.SearchUsingTypeAsync<StringResponse>(IndexName, typeof(T).Name.ToLower(), eql);

            return new ElasticQueryResponse<T>(resp, Searchable._pageIndex, Searchable._pageSize, eql);
        }

        private JObject BuildSearchExpression()
        {
            JObject result = new JObject
            {
                { "from", (Searchable._pageIndex - 1) * Searchable._pageSize },
                { "size", Searchable._pageSize }
            };

            JArray sort = new JArray();
            if (Searchable._sortExpresion != null)
            {
                sort.Add(new JObject
                {
                    {Searchable._sortExpresion.FieldName(),"asc" }
                });
            }
            if (Searchable._sortDescExpresion != null)
            {
                sort.Add(new JObject
                {
                    {Searchable._sortDescExpresion.FieldName(),"desc" }
                });
            }

            if (sort.Count > 0)
            {
                result.Add("sort", sort);
            }

            JObject _source = new JObject();
            if (Searchable._includeExpression != null && Searchable._includeExpression.Any())
            {
                var fields = Searchable._includeExpression.Select(item => item.FieldName()).ToArray();

                _source.Add("includes", new JArray(fields));
            }
            if (Searchable._excludeExpression != null && Searchable._excludeExpression.Any())
            {
                var fields = Searchable._excludeExpression.Select(item => item.FieldName()).ToArray();
                _source.Add("excludes", new JArray(fields));
            }

            if (_source.Count > 0)
            {
                result.Add("_source", _source);
            }


            JObject _bool = new JObject();
            if (Searchable._mustQueryable != null)
            {
                _bool.Add("must", BuildBoolExpression(Searchable._mustQueryable));
            }

            if (Searchable._shouldQueryable != null)
            {
                _bool.Add("should", BuildBoolExpression(Searchable._shouldQueryable));
            }

            if (Searchable._mustNotQueryable != null)
            {
                _bool.Add("must_not", BuildBoolExpression(Searchable._mustNotQueryable));
            }

            if (_bool.Count > 0)
            {
                JObject _query = new JObject
                {
                    { "bool", _bool }
                };

                result.Add("query", _query);
            }

            return result;
        }

        private JArray BuildBoolExpression(ElasticQueryable<T> queryable)
        {
            JArray queries = new JArray();
            if (!queryable._match.IsNullOrEmpty())
            {
                foreach (var match in queryable._match)
                {
                    JObject _match = new JObject
                    {
                        { match.Item1.FieldName(), match.Item2 }
                    };

                    queries.Add(new JObject
                    {
                        { "match", _match }
                    });
                }
            }

            if (!queryable._matchPhrase.IsNullOrEmpty())
            {
                foreach (var phrase in queryable._matchPhrase)
                {
                    JObject _phrase = new JObject
                    {
                        { phrase.Item1.FieldName(), phrase.Item2 }
                    };

                    queries.Add(new JObject
                    {
                        {"match_phrase", _phrase }
                    });
                }
            }

            if (!queryable._term.IsNullOrEmpty())
            {
                foreach (var term in queryable._term)
                {
                    JObject _term = new JObject
                    {
                        {
                            term.Item1.ToField().IfKeyword(),
                            term.Item2.GetType().Name switch
                            {
                                "Boolean" => Convert.ToBoolean(term.Item2),
                                _ => term.Item2.ToString()
                            }
                        }
                    };

                    queries.Add(new JObject
                    {
                        {"term", _term }
                    });
                }
            }

            if (!queryable._terms.IsNullOrEmpty())
            {
                foreach (var terms in queryable._terms)
                {
                    JObject _terms = new JObject
                    {
                        { terms.Item1.ToField().IfKeyword(), new JArray(terms.Item2) }
                    };

                    queries.Add(new JObject
                    {
                        {"terms", _terms}
                    });
                }
            }


            JObject _range = new JObject();
            const string dateFormat = "yyyy/MM/dd HH:mm:ss";

            if (!queryable._greaterThan.IsNullOrEmpty())
            {
                foreach (var greater in queryable._greaterThan)
                {
                    JObject _field;
                    if (_range.ContainsKey(greater.Item1.FieldName()))
                    {
                        _field = (JObject)_range[greater.Item1.FieldName()];
                    }
                    else
                    {
                        _field = new JObject();
                        _range.Add(greater.Item1.FieldName(), _field);
                    }

                    if (greater.Item2 is DateTime time)
                    {
                        if (!_field.ContainsKey(dateFormat))
                        {
                            _field.Add("format", dateFormat);
                        }
                        _field.Add("gt", time.ToString(dateFormat));
                    }
                    else
                    {
                        _field.Add("gt", (double)greater.Item2);
                    }
                }
            }

            if (!queryable._greaterThanEqual.IsNullOrEmpty())
            {
                foreach (var greaterEqual in queryable._greaterThanEqual)
                {
                    JObject _field;
                    if (_range.ContainsKey(greaterEqual.Item1.FieldName()))
                    {
                        _field = (JObject)_range[greaterEqual.Item1.FieldName()];
                    }
                    else
                    {
                        _field = new JObject();
                        _range.Add(greaterEqual.Item1.FieldName(), _field);
                    }

                    if (greaterEqual.Item2 is DateTime time)
                    {
                        if (!_field.ContainsKey(dateFormat))
                        {
                            _field.Add("format", dateFormat);
                        }
                        _field.Add("gte", time.ToString(dateFormat));
                    }
                    else
                    {
                        _field.Add("gte", (double)greaterEqual.Item2);
                    }
                }
            }

            if (!queryable._lessThan.IsNullOrEmpty())
            {
                foreach (var less in queryable._lessThan)
                {
                    JObject _field;
                    if (_range.ContainsKey(less.Item1.FieldName()))
                    {
                        _field = (JObject)_range[less.Item1.FieldName()];
                    }
                    else
                    {
                        _field = new JObject();
                        _range.Add(less.Item1.FieldName(), _field);
                    }

                    if (less.Item2 is DateTime time)
                    {
                        if (!_field.ContainsKey(dateFormat))
                        {
                            _field.Add("format", dateFormat);
                        }
                        _field.Add("lt", time.ToString(dateFormat));
                    }
                    else
                    {
                        _field.Add("lt", (double)less.Item2);
                    }
                }
            }

            if (!queryable._lessThanEqual.IsNullOrEmpty())
            {
                foreach (var lessEqual in queryable._lessThanEqual)
                {
                    JObject _field;
                    if (_range.ContainsKey(lessEqual.Item1.FieldName()))
                    {
                        _field = (JObject)_range[lessEqual.Item1.FieldName()];
                    }
                    else
                    {
                        _field = new JObject();
                        _range.Add(lessEqual.Item1.FieldName(), _field);
                    }

                    if (lessEqual.Item2 is DateTime time)
                    {
                        if (!_field.ContainsKey(dateFormat))
                        {
                            _field.Add("format", dateFormat);
                        }
                        _field.Add("lte", time.ToString(dateFormat));
                    }
                    else
                    {
                        _field.Add("lte", (double)lessEqual.Item2);
                    }
                }
            }

            if (_range.Count > 0)
            {
                foreach (var jToken in _range)
                {
                    var field = new JObject
                    {
                        { jToken.Key, jToken.Value }
                    };

                    queries.Add(new JObject
                    {
                        {"range", field}
                    });
                }
            }

            return queries;
        }
    }
}
