using System;
using Xunit;
using System.Collections.Generic;
using ElasticSearch.Linq;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace ElasticSearch.Test
{
    public class ElasticServiceTest : ElasticQueryCase
    {

        public ElasticServiceTest()
        {
            serviceProvider = GetServiceProvider();
        }

        protected override IServiceProvider serviceProvider { get; }


        protected IServiceProvider GetServiceProvider()
        {
            ESOptions config = new ESOptions
            {
                Urls = new List<string> { "http://192.168.1.195:9200" },
                DefaultIndex = "elastic_test"
            };

            IServiceCollection services = new ServiceCollection();

            services.AddElasticSearchLinq(config);

            return services.BuildServiceProvider();
        }

        [Fact]
        public void GetElasticService()
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var _provider = scope.ServiceProvider;

                var clientService = _provider.GetService<IElasticService>();

                Assert.NotNull(clientService);

                Assert.IsType<ElasticService>(clientService);
            }
        }
    }

}
