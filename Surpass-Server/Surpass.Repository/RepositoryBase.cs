using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using Surpass.Domain.Filters;
using Surpass.Domain.Interface;
using Surpass.Domain.Uow.Interfaces;
using Surpass.Infrastructure.Database;
using Surpass.Domain.Uow.Extensions;
using SurpassStandard.Utils;

namespace Surpass.Repository
{
    public abstract class RepositoryBase<TEntity, TPrimaryKey> : IRepositoryBase<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        /// <summary>
        /// 获取工作单元
        /// </summary>
        protected virtual IUnitOfWork UnitOfWork => Application.Ioc.GetService<IUnitOfWork>();

        #region 查询方法

        /// <summary>
        /// 查询实体
        /// 受这些过滤器的影响: 查询过滤器
        /// </summary>
        private IQueryable<TEntity> Query()
        {
            var uow = UnitOfWork;
            var query = uow.Context.Query<TEntity>();
            return uow.WrapQuery<TEntity, TPrimaryKey>(query);
        }

        /// <inheritdoc />
        /// <summary>
        /// 根据主键获取实体
        /// </summary>
        public virtual TEntity Get(TPrimaryKey id)
        {
            var expr = ExpressionUtils.MakeMemberEqualiventExpression<TEntity>("Id", id);
            using (UnitOfWork.Scope())
            {
                return Query().FirstOrDefault(expr);
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// 获取符合条件的单个实体
        /// 受这些过滤器的影响: 查询过滤器
        /// </summary>
        public virtual TEntity Get(Expression<Func<TEntity, bool>> predicate)
        {
            using (UnitOfWork.Scope())
            {
                return Query().FirstOrDefault(predicate);
            }
        }

        /// <summary>
        /// 根据条件获取实体列表
        /// </summary>
        public virtual IList<TEntity> GetMany(Expression<Func<TEntity, bool>> predicate = null)
        {
            using (UnitOfWork.Scope())
            {
                var query = Query();
                if (predicate != null)
                {
                    query = query.Where(predicate);
                }
                return query.ToList();
            }
        }

        /// <summary>
        /// 根据过滤函数获取实体列表
        /// </summary>
        public virtual TResult GetFunc<TResult>(
            Func<IQueryable<TEntity>, TResult> fetch)
        {
            using (UnitOfWork.Scope())
            {
                return fetch(Query());
            }
        }


        /// <inheritdoc />
        /// <summary>
        /// 计算符合条件的实体数量
        /// 受这些过滤器的影响: 查询过滤器
        /// </summary>
        public long Count(Expression<Func<TEntity, bool>> predicate)
        {
            return Query().LongCount(predicate);
        }

        #endregion

        #region 新增或修改方法

        /// <inheritdoc />
        /// <summary>
        /// 添加或更新实体
        /// 受这些过滤器的影响: 操作过滤器
        /// </summary>
        public virtual void Save(ref TEntity entity, Action<TEntity> update = null)
        {
            try
            {
                var uow = UnitOfWork;
                update = uow.WrapUpdateMethod<TEntity, TPrimaryKey>(update);
                uow.Context.Save(ref entity, update);
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        /// <inheritdoc />
        /// <summary>
        /// 批量保存实体
        /// 受这些过滤器的影响: 操作过滤器
        /// </summary>
        public virtual void BatchSave(
            ref IEnumerable<TEntity> entities, Action<TEntity> update = null)
        {
            var uow = UnitOfWork;
            update = uow.WrapUpdateMethod<TEntity, TPrimaryKey>(update);
            uow.Context.BatchSave(ref entities, update);
        }

        /// <inheritdoc />
        /// <summary>
        /// 根据筛选条件批量更新实体
        /// 受这些过滤器的影响: 查询过滤器, 操作过滤器
        /// </summary>
        public virtual long BatchUpdate(
            Expression<Func<TEntity, bool>> predicate, Action<TEntity> update)
        {
            var uow = UnitOfWork;
            predicate = uow.WrapPredicate<TEntity, TPrimaryKey>(predicate);
            update = uow.WrapUpdateMethod<TEntity, TPrimaryKey>(update);
            return uow.Context.BatchUpdate(predicate, update);
        }

        #endregion

        #region 删除方法

        /// <inheritdoc />
        /// <summary>
        /// 删除实体
        /// 受这些过滤器的影响: 操作过滤器
        /// </summary>
        public virtual void Delete(TEntity entity)
        {
            var uow = UnitOfWork;
            uow.WrapBeforeDeleteMethod<TEntity, TPrimaryKey>(e => { })(entity);
            uow.Context.Delete(entity);
        }

        /// <summary>
        /// 根据主键删除实体
        /// </summary>
        public virtual bool Delete(TPrimaryKey id)
        {
            var expr = ExpressionUtils.MakeMemberEqualiventExpression<TEntity>("Id", id);
            using (UnitOfWork.Scope())
            {
                return BatchDelete(expr) > 0;
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// 批量删除实体
        /// 受这些过滤器的影响: 查询过滤器, 操作过滤器
        /// </summary>
        public virtual long BatchDelete(
            Expression<Func<TEntity, bool>> predicate, Action<TEntity> beforeDelete = null)
        {
            var uow = UnitOfWork;
            predicate = uow.WrapPredicate<TEntity, TPrimaryKey>(predicate);
            beforeDelete = uow.WrapBeforeDeleteMethod<TEntity, TPrimaryKey>(beforeDelete);
            return uow.Context.BatchDelete(predicate, beforeDelete);
        }

        /// <summary>
        /// 批量永久删除
        /// </summary>
        public virtual long BatchDeleteForever(IEnumerable<TPrimaryKey> ids)
        {
            var uow = UnitOfWork;
            using (uow.Scope())
            using (uow.DisableQueryFilter(typeof(DeletedFilter)))
            {
                return BatchDelete(e => ids.Contains(e.Id));
            }
        }

        #endregion
    }
}
