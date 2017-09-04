using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace SurpassStandard.Dependency {
	/// <summary>
	/// Attribute for register type to IoC container with itself and it's base type and interfaces<br/>
	/// 用于根据类型的基类和接口，注册类型到IoC容器的属性<br/>
	/// </summary>
	/// <seealso cref="IContainer"/>
	/// <seealso cref="Container"/>
	[AttributeUsage(
		AttributeTargets.Class | AttributeTargets.Struct,
		Inherited = false,
		AllowMultiple = false)]
	public class ExportManyAttribute : ExportAttributeBase {
		/// <summary>
		/// Service key<br/>
		/// 服务键<br/>
		/// </summary>
		public object ContractKey { get; set; }
		/// <summary>
		/// Except types<br/>
		/// 排除的类型列表<br/>
		/// </summary>
		public Type[] Except { get; set; }
		/// <summary>
		/// Also register with non public service types<br/>
		/// 同时注册到非公开的服务类型<br/>
		/// </summary>
		public bool NonPublic { get; set; }
		/// <summary>
		/// Unregister service types before register<br/>
		/// Please sure it won't unintentionally remove innocent implementations<br/>
		/// 注销已有的服务类型的实现<br/>
		/// 请确保它不会意外的移除无辜的实现<br/>
		/// </summary>
		public bool ClearExists { get; set; }

		/// <summary>
		/// Register implementation type to container<br/>
		/// 注册实现类型到容器<br/>
		/// </summary>
		public override void RegisterToContainer(IServiceCollection services, Type type, ServiceLifetime lifeTimeType)
		{
		    var serviceTypes = GetImplementedServiceTypes(type, NonPublic);
            // Apply except types
            if (Except != null && Except.Any()) {
				serviceTypes = serviceTypes.Where(t => !Except.Contains(t));
			}
			var serviceTypesArray = serviceTypes.ToList();

            // Apply clear exist
            if (ClearExists) {
				foreach (var serviceType in serviceTypesArray) {
					//container.Unregister(serviceType, ContractKey);
				}
			}
            // Register to container
		    services.Add(new List<ServiceDescriptor>(serviceTypesArray.Select(x => new ServiceDescriptor(x, type, lifeTimeType))));
        }

	    /// <summary>
	    /// Get base types and interfaces<br/>
	    /// 获取类型的所有基类和接口类<br/>
	    /// </summary>
	    public static IEnumerable<Type> GetImplementedTypes(Type type)
	    {
	        foreach (var interfaceType in type.GetInterfaces())
	        {
	            yield return interfaceType;
	        }
	        var baseType = type;
	        while (baseType != null)
	        {
	            yield return baseType;
	            baseType = baseType.BaseType;
	        }
	    }

	    /// <summary>
	    /// Get base types and interfaces that can be a service type<br/>
	    /// What type can't be a service type<br/>
	    /// - It's non public and parameter nonPublicServiceTypes is false<br/>
	    /// - It's from mscorlib<br/>
	    /// 获取类型的可以作为服务类型的基类和接口类<br/>
	    /// 什么类型不能作为服务类型<br/>
	    /// - 类型是非公开, 并且参数nonPublicServiceTypes是false<br/>
	    /// - 类型来源于mscorlib<br/>
	    /// </summary>
	    public static IEnumerable<Type> GetImplementedServiceTypes(Type type, bool nonPublicServiceTypes)
	    {
	        var mscorlibAssembly = typeof(int).Assembly;
	        foreach (var serviceType in GetImplementedTypes(type))
	        {
	            if ((!serviceType.IsNotPublic || nonPublicServiceTypes) &&
	                (serviceType.Assembly != mscorlibAssembly))
	            {
	                yield return serviceType;
	            }
	        }
	    }

    }
}
