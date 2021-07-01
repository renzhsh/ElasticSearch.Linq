using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ElasticSearch.Linq.Extession
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddElasticSearchLinq(this IServiceCollection services, Action<ESOptions> opt)
        {
            DCheck.NotNull(services, nameof(services));
            DCheck.NotNull(opt, nameof(opt));

            services.AddOptions().Configure(opt);

            var options = services.BuildServiceProvider().GetService<IOptions<ESOptions>>();

            var esConfig = options.Value;

            ESOptions.Validate(esConfig);

            services.AddSingleton<IElasticClientFactory, ElasticClientFactory>();
            if(esConfig.IsCompatibleVersion())
            {
                services.AddScoped<IElasticService, ElasticService>();
            }
            else
            {
                services.AddScoped<IElasticService, LowElasticService>();
            }

            return services;
        }
    }
}
