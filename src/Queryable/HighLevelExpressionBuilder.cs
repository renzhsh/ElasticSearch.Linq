using ElasticSearch.Linq.Extession;
using Nest;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearch.Linq
{
    public class HighLevelExpressionBuilder<T> : IExpressionBuilder<T> where T : class
    {
        public HighLevelExpressionBuilder(IElasticClient client, ElasticSearchable<T> search)
        {
            Client = client;
            searchable = search;
        }


        private ElasticSearchable<T> searchable { get; }

        private IElasticClient Client { get; }

        public ElasticQueryResponse<T> ToList()
        {
            var resp = Client.Search<T>(BuildSearchExpression());

            return new ElasticQueryResponse<T>(resp, searchable._pageIndex, searchable._pageSize);
        }

        public async Task<ElasticQueryResponse<T>> ToListAsync()
        {
            var resp = await Client.SearchAsync<T>(BuildSearchExpression());
            return new ElasticQueryResponse<T>(resp, searchable._pageIndex, searchable._pageSize);
        }

        private Func<SearchDescriptor<T>, ISearchRequest> BuildSearchExpression()
        {
            //var resp = client.Search<AreaCodeDto>(s => s
            //    .Query(q => q
            //        .MatchPhrase(p => p
            //            .Field(c => c.AreaName)
            //            .Query("区")
            //        //.Name("hello, match_phrase")
            //        )
            //        )
            //    .Source(s => s
            //        .Includes(t => t.Fields(m => m.AreaName, m => m.AreaCode, m => m.LastUpdatedTime))
            //        )
            //    .From(1)
            //    .Size(5)
            //    .Sort(p => p.Descending(m => m.Id))
            //    );


            return s => s
                .IF(searchable._mustQueryable != null, p => p.Query(q => q.Bool(b => b.Must(BuildBoolExpression(searchable._mustQueryable)))))
                .IF(searchable._shouldQueryable != null, p => p.Query(q => q.Bool(b => b.Should(BuildBoolExpression(searchable._shouldQueryable)))))
                .IF(searchable._mustNotQueryable != null, p => p.Query(q => q.Bool(b => b.MustNot(BuildBoolExpression(searchable._mustNotQueryable)))))
                .IF(searchable._includeExpression != null, p => p.Source(q => q.Includes(m => m.Fields(searchable._includeExpression))))
                .IF(searchable._excludeExpression != null, p => p.Source(q => q.Excludes(m => m.Fields(searchable._excludeExpression))))
                .From((searchable._pageIndex - 1) * searchable._pageSize)
                .Size(searchable._pageSize)
                .IF(searchable._sortExpresion != null, p => p.Sort(p => p.Ascending(searchable._sortExpresion)))
                .IF(searchable._sortDescExpresion != null, p => p.Sort(p => p.Descending(searchable._sortDescExpresion)));
        }

        private Func<QueryContainerDescriptor<T>, QueryContainer>[] BuildBoolExpression(ElasticQueryable<T> queryable)
        {
            List<Func<QueryContainerDescriptor<T>, QueryContainer>> queries =
                new List<Func<QueryContainerDescriptor<T>, QueryContainer>>();

            if (!queryable._match.IsNullOrEmpty())
            {
                foreach (var match in queryable._match)
                {
                    queries.Add(q => q.Match(m => m.Field(match.Item1.ToField()).Query(match.Item2)));
                }
            }

            if (!queryable._matchPhrase.IsNullOrEmpty())
            {
                foreach (var phrase in queryable._matchPhrase)
                {
                    queries.Add(q => q.MatchPhrase(m => m.Field(phrase.Item1.ToField()).Query(phrase.Item2)));
                }
            }

            if (!queryable._term.IsNullOrEmpty())
            {
                foreach (var term in queryable._term)
                {
                    queries.Add(q => q.Term(t => t.Field(term.Item1.IfKeyword()).Value(term.Item2)));
                }
            }

            if (!queryable._terms.IsNullOrEmpty())
            {
                foreach (var terms in queryable._terms)
                {
                    queries.Add(d => d.Terms(tq => tq.Field(terms.Item1.IfKeyword()).Terms(terms.Item2)));
                }
            }

            if (!queryable._greaterThan.IsNullOrEmpty())
            {
                foreach (var greater in queryable._greaterThan)
                {
                    if (greater.Item2 is DateTime)
                    {
                        queries.Add(d => d.DateRange(r => r.Field(greater.Item1.ToField()).GreaterThan((DateTime)greater.Item2)));
                    }
                    else
                    {
                        queries.Add(d => d.Range(r => r.Field(greater.Item1.ToField()).GreaterThan((double)greater.Item2)));
                    }
                }
            }

            if (!queryable._greaterThanEqual.IsNullOrEmpty())
            {
                foreach (var greaterEqual in queryable._greaterThanEqual)
                {
                    if (greaterEqual.Item2 is DateTime)
                    {
                        queries.Add(d => d.DateRange(r => r.Field(greaterEqual.Item1.ToField()).GreaterThanOrEquals((DateTime)greaterEqual.Item2)));
                    }
                    else
                    {
                        queries.Add(d => d.Range(r => r.Field(greaterEqual.Item1.ToField()).GreaterThanOrEquals((double)greaterEqual.Item2)));
                    }
                }
            }

            if (!queryable._lessThan.IsNullOrEmpty())
            {
                foreach (var less in queryable._lessThan)
                {
                    if (less.Item2 is DateTime)
                    {
                        queries.Add(d => d.DateRange(r => r.Field(less.Item1.ToField()).LessThan((DateTime)less.Item2)));
                    }
                    else
                    {
                        queries.Add(d => d.Range(r => r.Field(less.Item1.ToField()).LessThan((double)less.Item2)));
                    }
                }
            }

            if (!queryable._lessThanEqual.IsNullOrEmpty())
            {
                foreach (var lessEqual in queryable._lessThanEqual)
                {
                    if (lessEqual.Item2 is DateTime)
                    {
                        queries.Add(d => d.DateRange(r => r.Field(lessEqual.Item1.ToField()).LessThanOrEquals((DateTime)lessEqual.Item2)));
                    }
                    else
                    {
                        queries.Add(d => d.Range(r => r.Field(lessEqual.Item1.ToField()).LessThanOrEquals((double)lessEqual.Item2)));
                    }
                }
            }

            return queries.ToArray();
        }
    }
}
