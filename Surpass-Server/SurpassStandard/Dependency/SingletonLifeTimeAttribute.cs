using System;
using Microsoft.Extensions.DependencyInjection;

namespace SurpassStandard.Dependency
{
    /// <inheritdoc />
    /// <summary>
    /// Singleton reuse attribute<br />
    /// A convenient attribute from ReuseAttribute<br />
    /// 标记单例的属性<br />
    /// 继承了ReuseAttribute的便捷属性<br />
    /// </summary>
    /// <seealso cref="T:System.ComponentModel.IContainer" />
    /// <seealso cref="T:System.ComponentModel.Container" />
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
    public class SingletonLifeTimeAttribute : ServiceLifeTimeAttribute
    {


        /// <inheritdoc />
        /// <summary>
        /// Initialize<br />
        /// 初始化<br />
        /// </summary>
        public SingletonLifeTimeAttribute() : base(ServiceLifetime.Singleton) { }
    }
}