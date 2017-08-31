using System;
using Microsoft.Extensions.DependencyInjection;

namespace Surpass.DI.Extension
{
    /// <summary>
    /// 注册类型到依赖注入容器
    /// </summary>
    public class ExportAttributeBase:Attribute
    {
        public abstract void RegisterToContainer(IServiceCollection services,Type type, ServiceLifetime lifeType);
    }
}
