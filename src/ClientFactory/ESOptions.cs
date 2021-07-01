using System;
using System.Collections.Generic;
using System.Linq;

namespace ElasticSearch.Linq
{
    /// <summary>
    /// ES配置
    /// </summary>
    public class ESOptions
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

        /// <summary>
        /// 兼容版本(>=v7.0)
        /// </summary>
        /// <returns></returns>
        public bool IsCompatibleVersion()
        {
            if (string.IsNullOrEmpty(EsVersion)) return true;

            try
            {
                var major = EsVersion.Split(".").FirstOrDefault();
                return int.Parse(major) >= 7;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 验证EsOptions的有效性
        /// </summary>
        public static void Validate(ESOptions config)
        {
            if (config == null)
            {
                throw new ArgumentException("ESOptions不能为null");
            }
            if (config.Urls == null || !config.Urls.Any())
            {
                throw new ArgumentException("未指定ElasticSearch的Urls");
            }

            try
            {
                config.Urls.Select(e => new Uri(e));
            }
            catch (UriFormatException uriEx)
            {
                throw new ArgumentException("无效的Urls", uriEx);
            }

            if (string.IsNullOrEmpty(config.DefaultIndex))
            {
                throw new ArgumentException("未指定ElasticSearch的DefaultIndex");
            }

        }

    }

}
