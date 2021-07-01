using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearch.Linq
{
    public interface IExpressionBuilder<T> where T : class
    {
        public ElasticQueryResponse<T> ToList();

        public Task<ElasticQueryResponse<T>> ToListAsync();

    }
}
