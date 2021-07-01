using System;
using Xunit;
using System.Collections.Generic;
using ElasticSearch.Linq;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ElasticSearch.Test
{
    using Entity;

    public abstract class ElasticQueryCase
    {
        private readonly List<Student> students = new List<Student>
        {
            new Student
            {
                Id=1,
                Name="张三",
                Age=21,
                Address="北京市",
                BirthDay=new DateTime(2000,1,5),
                IsValid=true
            },
            new Student
            {
                Id=2,
                Name="李四",
                Age=22,
                Address="杭州市",
                BirthDay=new DateTime(2000,2,5),
                IsValid=false
            },
            new Student
            {
                Id = 3,
                Name = "王五",
                Age = 23,
                Address = "太原市",
                BirthDay=new DateTime(2000,3,5),
                IsValid=true
            },
            new Student
            {
                Id = 4,
                Name = "王六一",
                Age = 24,
                Address = "太原市",
                BirthDay=new DateTime(2000,4,5),
                IsValid=false
            }
        };

        protected abstract IServiceProvider serviceProvider { get; }

        [Fact]
        public void GetClient()
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var _provider = scope.ServiceProvider;

                var clientService = _provider.GetService<IElasticService>();

                Assert.NotNull(clientService);
            }
        }

        [Fact]
        public void IndexDocument()
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var _provider = scope.ServiceProvider;

                var clientService = _provider.GetService<IElasticService>();

                var r = clientService.IndexDoc(students.First());

                Assert.True(r.IsValid);

                Assert.True(clientService.DeleteIndex<Student>());
            }
        }

        [Fact]
        public async Task IndexDocumentAsync()
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var _provider = scope.ServiceProvider;

                var clientService = _provider.GetService<IElasticService>();

                var r = await clientService.IndexDocAsync(students.First());

                Assert.True(r.IsValid);

                Assert.True(clientService.DeleteIndex<Student>());
            }
        }

        [Fact]
        public void IndexMany()
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var _provider = scope.ServiceProvider;

                var clientService = _provider.GetService<IElasticService>();

                var r = clientService.IndexMany(students);

                Assert.True(r.IsValid);

                Assert.True(clientService.DeleteIndex<Student>());
            }
        }

        [Fact]
        public async Task IndexManyAsync()
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var _provider = scope.ServiceProvider;

                var clientService = _provider.GetService<IElasticService>();

                var r = await clientService.IndexManyAsync(students);

                Assert.True(r.IsValid);

                Assert.True(clientService.DeleteIndex<Student>());
            }
        }

        [Fact]
        public void Query()
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var _provider = scope.ServiceProvider;

                var client = _provider.GetService<IElasticService>();

                Assert.True(client.IndexMany(students).IsValid);

                var resp = client.Search<Student>().ToList();

                Assert.Equal(4, resp.Total);
                Assert.NotNull(resp.Documents.First().Name);
            }
        }

        [Fact]
        public void MatchQuery()
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var _provider = scope.ServiceProvider;

                var client = _provider.GetService<IElasticService>();

                Assert.True(client.IndexMany(students).IsValid);

                var resp = client.Search<Student>()
                   .Query(q => q.Match(s => s.Name, "王"))
                   .ToList();

                Assert.Equal(2, resp.Total);

                resp = client.Search<Student>()
                  .Query(q => q.Match(s => s.Name, "五"))
                  .ToList();

                Assert.Equal(1, resp.Total);

                resp = client.Search<Student>()
                    .Query(q => q.Match(s => s.Address, "天津市"))
                    .ToList();

                Assert.Equal(4, resp.Total);

                resp = client.Search<Student>()
                    .Query(q => q.MatchPhrase(s => s.Address, "天津市"))
                    .ToList();

                Assert.Equal(0, resp.Total);

                resp = client.Search<Student>()
                    .Query(q => q.MatchPhrase(s => s.Address, "太原市"))
                    .ToList();

                Assert.Equal(2, resp.Total);


            }
        }

        [Fact]
        public void TermTextQuery()
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var _provider = scope.ServiceProvider;

                var client = _provider.GetService<IElasticService>();

                Assert.True(client.IndexMany(students).IsValid);

                var resp = client.Search<Student>()
                    .Query(q => q.Term(s => s.Address, "太原"))
                    .ToList();

                Assert.Equal(0, resp.Total);

                resp = client.Search<Student>()
                    .Query(q => q.Term(s => s.Address, "太原市"))
                    .ToList();

                Assert.Equal(2, resp.Total);

                resp = client.Search<Student>()
                    .Query(q => q.Terms(s => s.Address, "北京市", "杭州市"))
                    .ToList();

                Assert.Equal(2, resp.Total);

            }
        }

        [Fact]
        public void TermQuery()
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var _provider = scope.ServiceProvider;

                var client = _provider.GetService<IElasticService>();

                Assert.True(client.IndexMany(students).IsValid);

                var resp = client.Search<Student>()
                    .Query(q => q.Term(s => s.Age, 21))
                    .ToList();

                Assert.Equal(1, resp.Total);

                resp = client.Search<Student>()
                    .Query(q => q.Terms(s => s.Age, 21, 22, 23))
                    .ToList();

                Assert.Equal(3, resp.Total);

                resp = client.Search<Student>()
                    .Query(q => q.Terms(s => s.BirthDay, new DateTime(2000, 1, 5), new DateTime(2000, 4, 5)))
                    .ToList();

                Assert.Equal(2, resp.Total);

                resp = client.Search<Student>()
                    .Query(q => q.Term(s => s.IsValid, true))
                    .ToList();

                Assert.Equal(2, resp.Total);

                resp = client.Search<Student>()
                    .Query(q => q.Terms(s => s.IsValid, true, false))
                    .ToList();

                Assert.Equal(4, resp.Total);

            }
        }

        [Fact]
        public void RangeQuery()
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var _provider = scope.ServiceProvider;

                var client = _provider.GetService<IElasticService>();

                Assert.True(client.IndexMany(students).IsValid);


                var resp = client.Search<Student>()
                   .Query(q => q.GreaterThan(s => s.Age, 21))
                   .ToList();

                Assert.Equal(3, resp.Total);

                resp = client.Search<Student>()
                    .Query(q => q.GreaterThanEqual(s => s.Age, 21))
                    .ToList();

                Assert.Equal(4, resp.Total);

                resp = client.Search<Student>()
                    .Query(q => q.GreaterThan(s => s.BirthDay, new DateTime(2000, 3, 5)))
                    .ToList();

                Assert.Equal(1, resp.Total);

                resp = client.Search<Student>()
                    .Query(q => q.GreaterThanEqual(s => s.BirthDay, new DateTime(2000, 3, 5)))
                    .ToList();

                Assert.Equal(2, resp.Total);

            }
        }

        [Fact]
        public void BoolQuery()
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var _provider = scope.ServiceProvider;

                var client = _provider.GetService<IElasticService>();

                Assert.True(client.IndexMany(students).IsValid);

                var resp = client.Search<Student>()
                   .Query(q => q.GreaterThan(s => s.Age, 23).Term(s => s.Address, "太原市"))
                   .ToList();

                Assert.Equal(1, resp.Total);

                resp = client.Search<Student>()
                    .Should(q => q.MatchPhrase(s => s.Address, "太原市").Term(s => s.Address, "北京市"))
                    .ToList();

                Assert.Equal(3, resp.Total);

                resp = client.Search<Student>()
                    .MustNot(q => q.MatchPhrase(s => s.Address, "太原市").Term(s => s.Address, "北京市"))
                    .ToList();

                Assert.Equal(1, resp.Total);

            }
        }

    }
}
