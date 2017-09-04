using System;
using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace SurpassStandard.Dependency {
	/// <summary>
	/// Scoped reuse attribute<br/>
	/// A convenient attribute from ReuseAttribute<br/>
	/// 标记范围重用的属性<br/>
	/// 继承了ReuseAttribute的便捷属性<br/>
	/// </summary>
	/// <seealso cref="IContainer"/>
	/// <seealso cref="Container"/>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
	public class ScopedAttribute : ServiceLifeTimeAttribute {
		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		public ScopedAttribute() : base(ServiceLifetime.Scoped) { }
	}
}
