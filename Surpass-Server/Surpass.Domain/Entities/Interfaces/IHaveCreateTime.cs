using System;
using Surpass.Database;

namespace Surpass.Domain.Entities.Interfaces {
	/// <inheritdoc />
	/// <summary>
	/// 包含创建时间的接口
	/// </summary>
	public interface IHaveCreateTime : IEntity {
		/// <summary>
		/// 创建时间
		/// </summary>
		DateTime CreateTime { get; set; }
	}
}
