using System;
using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace SurpassStandard.Dependency {
	/// <inheritdoc />
	/// <summary>
	/// Scoped reuse attribute<br />
	/// A convenient attribute from ReuseAttribute<br />
	/// 标记范围重用的属性<br />
	/// 继承了ReuseAttribute的便捷属性<br />
	/// </summary>
	/// <seealso cref="T:System.ComponentModel.IContainer" />
	/// <seealso cref="T:System.ComponentModel.Container" />
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
	public class ScopedLifeTimeAttribute : ServiceLifeTimeAttribute {
		/// <inheritdoc />
		/// <summary>
		/// Initialize<br />
		/// 初始化<br />
		/// </summary>
		public ScopedLifeTimeAttribute() : base(ServiceLifetime.Scoped) { }
	}
}
