using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Nest;
using ElasticSearch.Linq.Extession;
using Elasticsearch.Net;

namespace ElasticSearch.Linq
{
    public class ElasticSearchable<T> where T : class
    {
        public ElasticSearchable(IElasticClient client)
        {
            Client = client;
        }

        public ElasticSearchable(IElasticLowLevelClient lowLevelClient, string indexName)
        {
            DCheck.NotNullOrEmpty(indexName, nameof(indexName));
            LowLevelClient = lowLevelClient;
            IndexName = indexName;
        }

        protected IElasticClient Client { get; }

        protected IElasticLowLevelClient LowLevelClient { get; }

        protected string IndexName { get; }

        internal ElasticQueryable<T> _mustQueryable = null;
        public ElasticSearchable<T> Must(Func<ElasticQueryable<T>, ElasticQueryable<T>> func)
        {
            DCheck.NotNull(func, nameof(func));

            if (_mustQueryable == null)
            {
                _mustQueryable = new ElasticQueryable<T>();
            }

            _mustQueryable = func(_mustQueryable);

            return this;
        }

        internal ElasticQueryable<T> _shouldQueryable = null;
        public ElasticSearchable<T> Should(Func<ElasticQueryable<T>, ElasticQueryable<T>> func)
        {
            DCheck.NotNull(func, nameof(func));

            if (_shouldQueryable == null)
            {
                _shouldQueryable = new ElasticQueryable<T>();
            }

            _shouldQueryable = func(_shouldQueryable);

            return this;
        }

        internal ElasticQueryable<T> _mustNotQueryable = null;
        public ElasticSearchable<T> MustNot(Func<ElasticQueryable<T>, ElasticQueryable<T>> func)
        {
            DCheck.NotNull(func, nameof(func));

            if (_mustNotQueryable == null)
            {
                _mustNotQueryable = new ElasticQueryable<T>();
            }

            _mustNotQueryable = func(_mustNotQueryable);

            return this;
        }

        public ElasticSearchable<T> Query(Func<ElasticQueryable<T>, ElasticQueryable<T>> func)
        {
            DCheck.NotNull(func, nameof(func));
            return Must(func);
        }

        internal Expression<Func<T, object>>[] _includeExpression;
        public ElasticSearchable<T> Includes(params Expression<Func<T, object>>[] objectPath)
        {
            DCheck.NotNull(objectPath, nameof(objectPath));
            _includeExpression = objectPath;
            return this;
        }

        internal Expression<Func<T, object>>[] _excludeExpression;
        public ElasticSearchable<T> Excludes(params Expression<Func<T, object>>[] objectPath)
        {
            DCheck.NotNull(objectPath, nameof(objectPath));
            _excludeExpression = objectPath;
            return this;
        }

        internal int _pageIndex = 1;
        public ElasticSearchable<T> PageIndex(int index = 1)
        {
            DCheck.GreaterThan(index, nameof(index), 1, canEqual: true);
            _pageIndex = index;
            return this;
        }

        internal int _pageSize = 10;
        public ElasticSearchable<T> PageSize(int size = 10)
        {
            DCheck.GreaterThan(size, nameof(size), 1, canEqual: true);
            _pageSize = size;
            return this;
        }

        internal Expression<Func<T, object>> _sortExpresion;
        public ElasticSearchable<T> Sort(Expression<Func<T, object>> objectPath)
        {
            DCheck.NotNull(objectPath, nameof(objectPath));
            _sortExpresion = objectPath;
            return this;
        }

        internal Expression<Func<T, object>> _sortDescExpresion;
        public ElasticSearchable<T> SortDesc(Expression<Func<T, object>> objectPath)
        {
            DCheck.NotNull(objectPath, nameof(objectPath));
            _sortDescExpresion = objectPath;
            return this;
        }

        public ElasticQueryResponse<T> ToList()
        {
            IExpressionBuilder<T> builder = null;

            if (Client != null)
            {
                builder = new HighLevelExpressionBuilder<T>(Client, this);
            }
            if (LowLevelClient != null)
            {
                builder = new LowLevelExpressionBuilder<T>(LowLevelClient, this, IndexName);
            }

            return builder.ToList();
        }

        public Task<ElasticQueryResponse<T>> ToListAsync()
        {
            IExpressionBuilder<T> builder = null;

            if (Client != null)
            {
                builder = new HighLevelExpressionBuilder<T>(Client, this);
            }
            if (LowLevelClient != null)
            {
                builder = new LowLevelExpressionBuilder<T>(LowLevelClient, this, IndexName);
            }

            return builder.ToListAsync();
        }
    }
}
