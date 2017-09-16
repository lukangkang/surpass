﻿using System.Reflection;
using Surpass.Domain.Entities.Interfaces;

namespace Surpass.Domain.Entities.TypeTraits {
	/// <summary>
	/// 判断类型是否包含创建时间
	/// </summary>
	/// <typeparam name="TEntity">实体类型</typeparam>
	public static class CreateTimeTypeTrait<TEntity> {
		/// <summary>
		/// 判断结果
		/// </summary>
		public readonly static bool HaveCreateTime =
			typeof(IHaveCreateTime).GetTypeInfo().IsAssignableFrom(typeof(TEntity));
	}
}
