//using DYFrame.Core.Packs;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using System;
//using System.Linq;

//namespace ElasticSearch.Linq
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    public abstract class EServicePackBase : DYFramePack
//    {
//        /// <summary>
//        /// 模块级别
//        /// </summary>
//        public override PackLevel Level => PackLevel.Framework;
//        /// <summary>
//        /// 启动顺序
//        /// </summary>
//        public override int Order => 20;

//        public const string _optionKey = "Es";

//        /// <summary>
//        /// ES添加注入
//        /// </summary>
//        /// <param name="services"></param>
//        /// <returns></returns>
//        public override IServiceCollection AddServices(IServiceCollection services)
//        {
//            services.ConfigureValueBinder<ESConfig>(_optionKey);

//            services.AddSingleton<IElasticClientFactory, ElasticClientFactory>();

//            var serviceProvider = services.BuildServiceProvider();
//            var context = serviceProvider.GetOptions<ESConfig>(_optionKey);

//            if (context != null)
//            {
//                var factory = serviceProvider.GetService<IElasticClientFactory>();
//                if (factory.IsCompatibleVersion())
//                {
//                    services.AddScoped<IElasticService, ElasticService>();
//                }
//                else
//                {
//                    services.AddScoped<IElasticService, LowElasticService>();
//                }
//            }

//            return services;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="provider"></param>
//        public override void UsePack(IServiceProvider provider) => base.UsePack(provider);
//    }
//}
