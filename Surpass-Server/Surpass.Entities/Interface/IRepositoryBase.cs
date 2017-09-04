using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Surpass.Infrastructure.Database;

namespace Surpass.Domain.Interface
{
    /// <summary>
    /// 仓储的接口
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <typeparam name="TPrimaryKey">主键类型</typeparam>
    public interface IRepositoryBase<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        /// <summary>
        /// 根据主键获取实体
        /// </summary>
        TEntity Get(TPrimaryKey id);

        /// <summary>
        /// 获取符合条件的单个实体
        /// 受这些过滤器的影响: 查询过滤器
        /// </summary>
        /// <param name="predicate">条件</param>
        /// <returns></returns>
        TEntity Get(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 根据条件获取实体列表
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IList<TEntity> GetMany(Expression<Func<TEntity, bool>> predicate = null);

        /// <summary>
        /// 根据过滤函数获取实体列表
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="fetch"></param>
        /// <returns></returns>
        TResult GetFunc<TResult>(Func<IQueryable<TEntity>, TResult> fetch);

        /// <summary>
        /// 计算符合条件的实体数量
        /// 受这些过滤器的影响: 查询过滤器
        /// </summary>
        /// <param name="predicate">条件</param>
        /// <returns></returns>
        long Count(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 添加或更新实体
        /// 受这些过滤器的影响: 操作过滤器
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="update">更新函数</param>
        void Save(ref TEntity entity, Action<TEntity> update);

        /// <summary>
        /// 批量保存实体
        /// 受这些过滤器的影响: 操作过滤器
        /// </summary>
        /// <param name="entities">实体列表</param>
        /// <param name="update">更新函数</param>
        void BatchSave(
            ref IEnumerable<TEntity> entities, Action<TEntity> update = null);

        /// <summary>
        /// 批量更新实体
        /// 受这些过滤器的影响: 查询过滤器, 操作过滤器
        /// </summary>
        /// <param name="predicate">更新条件</param>
        /// <param name="update">更新函数</param>
        /// <returns></returns>
        long BatchUpdate(
            Expression<Func<TEntity, bool>> predicate, Action<TEntity> update);

        /// <summary>
        /// 删除实体
        /// 受这些过滤器的影响: 操作过滤器
        /// </summary>
        /// <param name="entity">实体</param>
        void Delete(TEntity entity);

        /// <summary>
        /// 根据主键删除实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Delete(TPrimaryKey id);

        /// <summary>
        /// 批量删除实体
        /// 受这些过滤器的影响: 查询过滤器, 操作过滤器
        /// </summary>
        /// <param name="predicate">删除条件</param>
        /// <param name="beforeDelete">删除前的函数</param>
        /// <returns></returns>
        long BatchDelete(
            Expression<Func<TEntity, bool>> predicate, Action<TEntity> beforeDelete = null);

        /// <summary>
        /// 批量永久删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        long BatchDeleteForever(IEnumerable<TPrimaryKey> ids);
    }
}
