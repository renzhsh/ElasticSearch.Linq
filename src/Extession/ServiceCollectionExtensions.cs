using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ElasticSearch.Linq.Extession
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddElasticSearchLinq(this IServiceCollection services, Action<ESConfig> opt)
        {

            services.Configure<ESConfig>(opt);
            return services;
        }
    }
}
