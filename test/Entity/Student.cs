using System;
using System.Collections.Generic;
using System.Text;
using ElasticSearch.Linq;

namespace ElasticSearch.Test.Entity
{
    public class Student : IElasticEntity<long>
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }

        public string Address { get; set; }

        public DateTime BirthDay { get; set; }

        public bool IsValid { get; set; }
    }
}
