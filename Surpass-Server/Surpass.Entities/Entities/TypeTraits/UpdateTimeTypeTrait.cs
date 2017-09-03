﻿using System.Reflection;
using Surpass.Domain.Entities.Interfaces;

namespace Surpass.Domain.Entities.TypeTraits {
	/// <summary>
	/// 判断类型是否有更新时间
	/// </summary>
	/// <typeparam name="TEntity">实体类型</typeparam>
	public static class UpdateTimeTypeTrait<TEntity> {
		/// <summary>
		/// 判断结果
		/// </summary>
		public readonly static bool HaveUpdateTime =
			typeof(IHaveUpdateTime).GetTypeInfo().IsAssignableFrom(typeof(TEntity));
	}
}
