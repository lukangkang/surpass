using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace SurpassStandard.Dependency {
	/// <summary>
	/// Attribute for register type to IoC container with specific service type<br/>
	/// 用于根据类型实现的单个服务类型，注册类型到IoC容器的属性<br/>
	/// </summary>
	[AttributeUsage(
		AttributeTargets.Class | AttributeTargets.Struct,
		Inherited = false,
		AllowMultiple = true)]
	public class ExportAttribute : ExportAttributeBase {
		/// <summary>
		/// Service key<br/>
		/// 服务键<br/>
		/// </summary>
		public object ContractKey { get; set; }
		/// <summary>
		/// Service type<br/>
		/// 服务类型<br/>
		/// </summary>
		public Type ServiceType { get; set; }
		/// <summary>
		/// Unregister service types before register<br/>
		/// 注销已有的服务类型的实现<br/>
		/// </summary>
		public bool ClearExists { get; set; }

		/// <summary>
		/// Register implementation type to container<br/>
		/// 注册实现类型到容器<br/>
		/// </summary>
		public override void RegisterToContainer(IServiceCollection services, Type type, ServiceLifetime lifeTimeType) {
			// Apply clear exist
			if (ClearExists)
			{
			    services.RemoveAll(type);
			}
            // Register to container
		    services.Add(new ServiceDescriptor(ServiceType, type, lifeTimeType));
		}
	}
}
