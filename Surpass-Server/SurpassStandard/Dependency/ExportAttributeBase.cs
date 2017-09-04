using System;
using Microsoft.Extensions.DependencyInjection;

namespace SurpassStandard.Dependency {
	/// <inheritdoc />
	/// <summary>
	/// Base attribute for register type to IoC container<br />
	/// 注册类型到IoC容器使用的属性的基类<br />
	/// </summary>
	public abstract class ExportAttributeBase : Attribute {
        /// <summary>
        /// Register implementation type to container<br/>
        /// 注册实现类型到容器<br/>
        /// </summary>
        /// <param name="services">Services</param>
        /// <param name="type">Implementation type</param>
        /// <param name="lifeTimeType">lifetime type</param>
        public abstract void RegisterToContainer(IServiceCollection services, Type type, ServiceLifetime lifeTimeType);
	}
}
