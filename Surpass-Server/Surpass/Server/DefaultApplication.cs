﻿using System;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Surpass.Database;
using Surpass.Infrastructure.Server;
using Surpass.Plugin;
using Surpass.Plugin.AssemblyLoaders;
using Surpass.Plugin.CompilerServices;
using Surpass.Storage;

namespace Surpass.Server
{
    public class DefaultApplication : IApplication
    {
        /// <summary>
        /// 
        /// </summary>
        public virtual string FullVersion => "1.0.0 rc 1";

        /// <summary>
        /// 
        /// </summary>
        public virtual Version Version { get; }

        /// <summary>
        /// 
        /// </summary>
        public virtual IServiceCollection Services { get; protected set; }

        /// <summary>
        ///  The MicroDI Container Instance
        /// MicroDI容器的服务获取实例
        /// </summary>
        public virtual IServiceProvider Ioc => Services.BuildServiceProvider();

        /// <summary>
        /// 
        /// </summary>
        public int InProgressRequests => InProgressRequests;

        /// <summary>
        /// Initialize Flag<br/>
        /// 初始化标记<br/>
        /// </summary>
        protected int initialized = 0;


        /// <summary>
        /// Website root directory<br/>
        /// 网站根目录的路径<br/>
        /// </summary>
        protected string WebsiteRootDirectory { get; set; }

        /// <summary>
        /// 注册组件到默认的容器
        /// </summary>
        protected virtual void InitializeContainer()
        {

            Services.AddSingleton<DatabaseManager>();

            Services.AddSingleton<IAssemblyLoader, CoreAssemblyLoader>();
            Services.AddSingleton<RoslynCompilerService>();
            Services.AddSingleton<PluginManager>();

            Services.AddSingleton<LocalPathConfig>();
            Services.AddSingleton<LocalPathManager>();
        }

        /// <summary>
        /// 初始化核心组件
        /// </summary>
        protected virtual void InitializeCoreComponents()
        {
            Ioc.GetService<LocalPathConfig>().Initialize(WebsiteRootDirectory);
            Ioc.GetService<PluginManager>().Intialize();
            Ioc.GetService<DatabaseManager>().Initialize();

        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="websiteRootDirectory"></param>
        public void Initialize(IServiceCollection services,string websiteRootDirectory)
        {
            if (Interlocked.Exchange(ref initialized, 1) != 0)
            {
                throw new InvalidOperationException("Application already initialized");
            }
            try
            {
                Services = services;
                WebsiteRootDirectory = websiteRootDirectory;
                InitializeContainer();
                InitializeCoreComponents();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void OnRequest(IHttpContextAccessor context)
        {
            throw new NotImplementedException();
        }

        public IDisposable OverrideIoc()
        {
            throw new NotImplementedException();
        }
    }
}
