using Nest;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ElasticSearch.Linq
{
    public class ElasticQueryable<T> where T : class
    {
        internal List<(Expression<Func<T, object>>, string)> _match = null;
        public ElasticQueryable<T> Match(Expression<Func<T, object>> objectPath, string keyword)
        {
            DCheck.NotNull(objectPath, nameof(objectPath));
            DCheck.NotNullOrEmpty(keyword, nameof(keyword));

            if (_match == null)
            {
                _match = new List<(Expression<Func<T, object>>, string)>();
            }

            _match.Add((objectPath, keyword));

            return this;
        }


        internal List<(Expression<Func<T, object>>, string)> _matchPhrase = null;
        public ElasticQueryable<T> MatchPhrase(Expression<Func<T, object>> objectPath, string keyword)
        {
            DCheck.NotNull(objectPath, nameof(objectPath));
            DCheck.NotNullOrEmpty(keyword, nameof(keyword));

            if (_matchPhrase == null)
            {
                _matchPhrase = new List<(Expression<Func<T, object>>, string)>();
            }

            _matchPhrase.Add((objectPath, keyword));
            //Add(q => q.MatchPhrase(m => m.Field(objectPath).Query(keyword)));

            return this;
        }

        internal List<(Expression<Func<T, object>>, object)> _term = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectPath"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ElasticQueryable<T> Term(Expression<Func<T, object>> objectPath, object value)
        {
            DCheck.NotNull(objectPath, nameof(objectPath));
            DCheck.NotNull(value, nameof(value));

            if (_term == null)
            {
                _term = new List<(Expression<Func<T, object>>, object)>();
            }

            _term.Add((objectPath, value));
            //Add(q => q.Term(objectPath, value));

            return this;
        }


        internal List<(Expression<Func<T, object>>, object[])> _terms = null;
        public ElasticQueryable<T> Terms(Expression<Func<T, object>> objectPath, params object[] values)
        {
            DCheck.NotNull(objectPath, nameof(objectPath));
            DCheck.NotNull(values, nameof(values));

            if (_terms == null)
            {
                _terms = new List<(Expression<Func<T, object>>, object[])>();
            }

            _terms.Add((objectPath, values));

            return this;
        }


        /// <summary>
        /// 大于
        /// </summary>
        /// <param name="objectPath"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ElasticQueryable<T> GreaterThan(Expression<Func<T, object>> objectPath, double value)
        {
            DCheck.NotNull(objectPath, nameof(objectPath));
            DCheck.NotNull(value, nameof(value));

            if (_greaterThan == null)
            {
                _greaterThan = new List<(Expression<Func<T, object>>, object)>();
            }

            _greaterThan.Add((objectPath, value));

            return this;
        }

        /// <summary>
        /// 大于
        /// </summary>
        /// <param name="objectPath"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ElasticQueryable<T> GreaterThan(Expression<Func<T, object>> objectPath, DateTime value)
        {
            DCheck.NotNull(objectPath, nameof(objectPath));
            DCheck.NotNull(value, nameof(value));

            if (_greaterThan == null)
            {
                _greaterThan = new List<(Expression<Func<T, object>>, object)>();
            }

            _greaterThan.Add((objectPath, value));

            return this;
        }

        internal List<(Expression<Func<T, object>>, object)> _greaterThan = null;

        /// <summary>
        /// 大于等于
        /// </summary>
        /// <param name="objectPath"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ElasticQueryable<T> GreaterThanEqual(Expression<Func<T, object>> objectPath, double value)
        {
            DCheck.NotNull(objectPath, nameof(objectPath));
            DCheck.NotNull(value, nameof(value));

            if (_greaterThanEqual == null)
            {
                _greaterThanEqual = new List<(Expression<Func<T, object>>, object)>();
            }

            _greaterThanEqual.Add((objectPath, value));

            return this;
        }

        /// <summary>
        /// 大于等于
        /// </summary>
        /// <param name="objectPath"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ElasticQueryable<T> GreaterThanEqual(Expression<Func<T, object>> objectPath, DateTime value)
        {
            DCheck.NotNull(objectPath, nameof(objectPath));
            DCheck.NotNull(value, nameof(value));

            if (_greaterThanEqual == null)
            {
                _greaterThanEqual = new List<(Expression<Func<T, object>>, object)>();
            }

            _greaterThanEqual.Add((objectPath, value));

            return this;
        }

        internal List<(Expression<Func<T, object>>, object)> _greaterThanEqual = null;

        /// <summary>
        /// 小于
        /// </summary>
        /// <param name="objectPath"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ElasticQueryable<T> LessThan(Expression<Func<T, object>> objectPath, double value)
        {
            DCheck.NotNull(objectPath, nameof(objectPath));
            DCheck.NotNull(value, nameof(value));

            if (_lessThan == null)
            {
                _lessThan = new List<(Expression<Func<T, object>>, object)>();
            }

            _lessThan.Add((objectPath, value));

            return this;
        }

        /// <summary>
        /// 小于
        /// </summary>
        /// <param name="objectPath"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ElasticQueryable<T> LessThan(Expression<Func<T, object>> objectPath, DateTime value)
        {
            DCheck.NotNull(objectPath, nameof(objectPath));
            DCheck.NotNull(value, nameof(value));

            if (_lessThan == null)
            {
                _lessThan = new List<(Expression<Func<T, object>>, object)>();
            }

            _lessThan.Add((objectPath, value));

            return this;
        }

        internal List<(Expression<Func<T, object>>, object)> _lessThan = null;

        /// <summary>
        /// 小于等于
        /// </summary>
        /// <param name="objectPath"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ElasticQueryable<T> LessThanEqual(Expression<Func<T, object>> objectPath, double value)
        {
            DCheck.NotNull(objectPath, nameof(objectPath));
            DCheck.NotNull(value, nameof(value));

            if (_lessThanEqual == null)
            {
                _lessThanEqual = new List<(Expression<Func<T, object>>, object)>();
            }

            _lessThanEqual.Add((objectPath, value));

            return this;
        }

        /// <summary>
        /// 小于等于
        /// </summary>
        /// <param name="objectPath"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ElasticQueryable<T> LessThanEqual(Expression<Func<T, object>> objectPath, DateTime value)
        {
            DCheck.NotNull(objectPath, nameof(objectPath));
            DCheck.NotNull(value, nameof(value));

            if (_lessThanEqual == null)
            {
                _lessThanEqual = new List<(Expression<Func<T, object>>, object)>();
            }

            _lessThanEqual.Add((objectPath, value));

            return this;
        }

        internal List<(Expression<Func<T, object>>, object)> _lessThanEqual = null;
    }

}
