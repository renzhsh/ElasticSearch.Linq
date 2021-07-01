using System.Collections.Generic;
using System.Linq;

namespace ElasticSearch.Linq
{
    /// <summary>
    /// ES配置
    /// </summary>
    public class ESConfig
    {
        /// <summary>
        /// ES服务地址
        /// </summary>
        public List<string> Urls { get; set; }

        /// <summary>
        /// 默认索引名称
        /// </summary>
        public string DefaultIndex { get; set; }

        /// <summary>
        /// Es版本
        /// </summary>
        public string EsVersion { get; set; }

    }

}
