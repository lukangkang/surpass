﻿using System.Reflection;
using Surpass.Domain.Entities.Interfaces;

namespace Surpass.Domain.Entities.TypeTraits {
	/// <summary>
	/// 判断类型是否包含删除标记
	/// </summary>
	/// <typeparam name="TEntity">实体类型</typeparam>
	public static class DeletedTypeTrait<TEntity> {
		/// <summary>
		/// 判断结果
		/// </summary>
		public readonly static bool HaveDeleted =
			typeof(IHaveDeleted).GetTypeInfo().IsAssignableFrom(typeof(TEntity));
	}
}
