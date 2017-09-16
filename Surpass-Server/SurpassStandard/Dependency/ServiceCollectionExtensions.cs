using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using SurpassStandard.Extensions;

namespace SurpassStandard.Dependency
{
    /// <summary>
    /// ServiceCollection的扩展方法
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 按Export属性自动注册类型到容器
        /// </summary>
        /// <param name="services"></param>
        /// <param name="types"></param>
        public static void AddExports(this IServiceCollection services, IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                // Get export attributes
                var exportAttributes = type.GetAttributes<ExportAttributeBase>().ToArray();
                if (!exportAttributes.Any())
                {
                    continue;
                }
                // Get reuse attribute
                var reuseType = type.GetAttribute<ServiceLifeTimeAttribute>()?.LifeTimeType ?? default(ServiceLifetime);
                // Call RegisterToContainer
                foreach (var attribute in exportAttributes)
                {
                    attribute.RegisterToContainer(services, type, reuseType);
                }
            }
        }
    }
}
