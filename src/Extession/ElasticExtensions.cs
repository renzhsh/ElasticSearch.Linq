using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel;

namespace ElasticSearch.Linq.Extession
{
    /// <summary>
    /// 集合扩展
    /// </summary>
    public static class ElasticExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> lst) => lst == null || lst.Count() == 0;


        internal static string EsType(this Type type)
        {
            return type.Name.ToLower();
        }

        internal static SearchDescriptor<T> IF<T>(this SearchDescriptor<T> source, bool condition, Func<SearchDescriptor<T>, SearchDescriptor<T>> func) where T : class
        {
            DCheck.NotNull(source, nameof(source));
            return condition ? func(source) : source;
        }

        /// <summary>
        /// Func<T,object> ==> Func<T, int>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        internal static MemberExpression ToField<T>(this Expression<Func<T, object>> func) where T : class
        {
            if (func.Body is MemberExpression)
            {
                return (MemberExpression)func.Body;
            }

            var expressionBody = (UnaryExpression)func.Body;

            return (MemberExpression)expressionBody.Operand;

        }

        internal static string IfKeyword(this MemberExpression express)
        {
            var member = express.Member;

            return member.Name + ((express.Type == typeof(string)) ? ".keyword" : "");
        }

        internal static Expression<Func<T, object>> IfKeyword<T>(this Expression<Func<T, object>> func) where T : class
        {
            MemberExpression express = func.ToField();

            if (express.Type == typeof(string))
            {
                Expression<Func<T, object>> suffixExpress = o => o.Suffix("keyword");

                var methodCall = (MethodCallExpression)suffixExpress.Body;

                var result = Expression.Call(methodCall.Method, func.Body, Expression.Constant("keyword"));

                return Expression.Lambda<Func<T, object>>(result, func.Parameters);
            }

            return func;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        internal static string FieldName<T>(this Expression<Func<T, object>> func) where T : class
        {
            MemberExpression express = (MemberExpression)func.ToField();

            return express.Member.Name;
        }

        public static string GetDesc(this Enum val)
        {
            var memberInfo = val.GetType().GetMember(val.ToString());
            var attributes = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes == null || attributes.Length != 1)
                return val.ToString();

            return (attributes.Single() as DescriptionAttribute).Description;
        }

    }

}
