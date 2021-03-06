﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Surpass.Database;
using Surpass.Domain.Repositories.Interfaces;
using Surpass.Domain.Uow.Extensions;
using Surpass.Domain.Uow.Interfaces;

namespace Surpass.Domain.Repositories
{
    public abstract class RepositoryBase<TEntity, TPrimaryKey> : IRepositoryBase<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        /// <summary>
		/// 获取工作单元
		/// </summary>
		protected IUnitOfWork UnitOfWork => Application.Provider.GetService<IUnitOfWork>();

        /// <summary>
        /// 查询实体
        /// 受这些过滤器的影响: 查询过滤器
        /// </summary>
        public IQueryable<TEntity> Query()
        {
            var uow = UnitOfWork;
            var query = uow.Context.Query<TEntity>();
            return uow.WrapQuery<TEntity, TPrimaryKey>(query);
        }

        /// <summary>
        /// 获取符合条件的单个实体
        /// 受这些过滤器的影响: 查询过滤器
        /// </summary>
        public TEntity Get(Expression<Func<TEntity, bool>> predicate)
        {
            return Query().FirstOrDefault(predicate);
        }

        /// <summary>
        /// 计算符合条件的实体数量
        /// 受这些过滤器的影响: 查询过滤器
        /// </summary>
        public long Size(Expression<Func<TEntity, bool>> predicate)
        {
            return Query().LongCount(predicate);
        }

        /// <summary>
        /// 添加或更新实体
        /// 受这些过滤器的影响: 操作过滤器
        /// </summary>
        public void Save(ref TEntity entity, Action<TEntity> update = null)
        {
            var uow = UnitOfWork;
            update = uow.WrapUpdateMethod<TEntity, TPrimaryKey>(update);
            uow.Context.Save(ref entity, update);
        }

        /// <summary>
        /// 删除实体
        /// 受这些过滤器的影响: 操作过滤器
        /// </summary>
        public void Delete(TEntity entity)
        {
            var uow = UnitOfWork;
            uow.WrapBeforeDeleteMethod<TEntity, TPrimaryKey>(e => { })(entity);
            uow.Context.Delete(entity);
        }

        /// <summary>
        /// 批量保存实体
        /// 受这些过滤器的影响: 操作过滤器
        /// </summary>
        public void BatchSave(
            ref IEnumerable<TEntity> entities, Action<TEntity> update = null)
        {
            var uow = UnitOfWork;
            update = uow.WrapUpdateMethod<TEntity, TPrimaryKey>(update);
            uow.Context.BatchSave(ref entities, update);
        }

        /// <summary>
        /// 批量更新实体
        /// 受这些过滤器的影响: 查询过滤器, 操作过滤器
        /// </summary>
        public long BatchUpdate(
            Expression<Func<TEntity, bool>> predicate, Action<TEntity> update)
        {
            var uow = UnitOfWork;
            predicate = uow.WrapPredicate<TEntity, TPrimaryKey>(predicate);
            update = uow.WrapUpdateMethod<TEntity, TPrimaryKey>(update);
            return uow.Context.BatchUpdate(predicate, update);
        }

        /// <summary>
        /// 批量删除实体
        /// 受这些过滤器的影响: 查询过滤器, 操作过滤器
        /// </summary>
        public long BatchDelete(
            Expression<Func<TEntity, bool>> predicate, Action<TEntity> beforeDelete)
        {
            var uow = UnitOfWork;
            predicate = uow.WrapPredicate<TEntity, TPrimaryKey>(predicate);
            beforeDelete = uow.WrapBeforeDeleteMethod<TEntity, TPrimaryKey>(beforeDelete);
            return uow.Context.BatchDelete(predicate, beforeDelete);
        }
    }
}
