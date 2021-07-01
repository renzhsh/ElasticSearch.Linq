using System;
using System.Collections.Generic;
using System.Text;

namespace ElasticSearch.Linq
{
    public interface IElasticEntity<Tkey> where Tkey : IEquatable<Tkey>
    {
        Tkey Id { get; set; }
    }
}
