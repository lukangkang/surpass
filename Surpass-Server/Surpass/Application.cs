using System;
using Microsoft.Extensions.DependencyInjection;
using Surpass.Infrastructure.Server;
using Surpass.Server;

namespace Surpass
{
    /// <summary>
    /// 主应用
    /// </summary>
    public class Application
    {
        /// <summary>
        /// Application Instance<br/>
        /// 应用的实例<br/>
        /// </summary>
        public static IApplication Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new NullReferenceException("Please set Application.Instance first");
                }
                return _instance;
            }
            set => _instance = value;
        }
        private static IApplication _instance;

        /// <summary>
        /// Surpass Version String<br/>
        /// Surpass的版本字符串<br/>
        /// </summary>
        public static string FullVersion => Instance.FullVersion;

        /// <summary>
        /// Surpass Version Object<br/>
        /// Surpass的版本对象<br/>
        /// </summary>
        public static Version Version => Instance.Version;

        /// <summary>
        ///  The MicroDI Container Instance
        /// MicroDI容器的服务注册实例
        /// </summary>
        public static IServiceCollection Services => Instance.services;

        /// <summary>
        ///  The MicroDI Container Instance
        /// MicroDI容器的服务获取实例
        /// </summary>
        public static IServiceProvider Ioc => Services.BuildServiceProvider();

        /// <summary>
        /// Intialize application with DefaultApplication<br/>
        /// 初始化默认应用<br/>
        /// </summary>
        public static void Initialize(string websiteRootDirectory)
        {
            Initialize<DefaultApplication>(websiteRootDirectory);
        }

        /// <summary>
        /// Intialize application with specificed application type<br/>
        /// 初始化指定应用<br/>
        /// </summary>
        public static void Initialize<TApplication>(string websiteRootDirectory)
            where TApplication : IApplication, new()
        {
            Instance = new TApplication();
            Instance.Initialize(websiteRootDirectory);
        }
    }
}
