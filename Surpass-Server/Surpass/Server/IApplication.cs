using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Surpass.Infrastructure.Server
{
    /// <summary>
    /// 应用类的接口
    /// </summary>
    public interface IApplication
    {
        /// <summary>
        /// Surpass Version String<br/>
        /// Surpass的版本字符串<br/>
        /// </summary>
        string FullVersion { get; }
        /// <summary>
        /// Surpass Version Object<br/>
        /// Surpass的版本对象<br/>
        /// </summary>
        Version Version { get; }

        /// <summary>
        /// The MicroDI Container Instance
        /// MicroDI容器的实例
        /// </summary>
        IServiceCollection services { get; }

        /// <summary>
        /// In progress requests<br/>
        /// 处理中的请求数量<br/>
        /// </summary>
        int InProgressRequests { get; }

        /// <summary>
        /// Intialize main application<br/>
        /// 初始化主应用<br/>
        /// </summary>
        /// <param name="websiteRootDirectory"></param>
        void Initialize(string websiteRootDirectory);

        /// <summary>
        /// Handle http request<br/>
        /// Method "Response.End" will be called if processing completed without errors
        /// 处理Http请求<br/>
        /// 如果处理成功完成将会调用"Response.End"函数<br/>
        /// </summary>
        /// <param name="context">Http context</param>
        void OnRequest(IHttpContextAccessor context);

        IDisposable OverrideIoc();
    }
}
