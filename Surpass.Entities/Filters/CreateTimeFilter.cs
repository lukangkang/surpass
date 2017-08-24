using System;
using Surpass.Domain.Filters.Interfaces;

namespace Surpass.Domain.Filters {
	/// <summary>
	/// 自动设置实体的创建时间
	/// </summary>
	[ExportMany]
	public class CreateTimeFilter : IEntityOperationFilter {
		/// <summary>
		/// 自动设置实体的创建时间
		/// </summary>
		void IEntityOperationFilter.FilterSave<TEntity, TPrimaryKey>(TEntity entity) {
			if (entity is IHaveCreateTime) {
				var et = (IHaveCreateTime)entity;
				if (et.CreateTime == default(DateTime)) {
					et.CreateTime = DateTime.UtcNow;
				}
			}
		}

		/// <summary>
		/// 不需要处理删除
		/// </summary>
		void IEntityOperationFilter.FilterDelete<TEntity, TPrimaryKey>(TEntity entity) { }
	}
}
