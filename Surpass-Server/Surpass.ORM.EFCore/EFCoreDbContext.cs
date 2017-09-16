using System;
using System.Collections.Generic;
using System.Data;
using System.FastReflection;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Surpass.Database;
using Surpass.Infrastructure.Database;
using SurpassStandard.Utils;

namespace Surpass.ORM.EFCore
{
    /// <summary>
    /// Entity Framework Core database context<br/>
    /// Entity Framework Core的数据库上下文<br/>
    /// </summary>
    public class EFCoreDbContext : EFCoreDbContextBase,IDatabaseContext
    {
        /// <summary>
        /// Entity Framework Core transaction<br/>
        /// Entity Framework Core的事务<br/>
        /// </summary>
        protected IDbContextTransaction Transaction { get; set; }
        /// <summary>
        /// Transaction level counter<br/>
        /// 事务的嵌套计数<br/>
        /// </summary>
        protected int TransactionLevel;
        /// <summary>
        /// ORM name<br/>
        /// ORM名称<br/>
        /// </summary>
        public string ORM => ConstORM;
        internal const string ConstORM = "EFCore";

        /// <summary>
        /// Database type<br/>
        /// 数据库类型<br/>
        /// </summary>
        string IDatabaseContext.Database => database;

        protected string database;

        /// <summary>
        /// Underlying database connection<br/>
        /// 底层的数据库连接<br/>
        /// </summary>
        public object DbConnection => Database.GetDbConnection();

        /// <summary>
        /// Database Initialize Handlers<br/>
        /// 数据库初始化处理器的列表<br/>
        /// </summary>
        protected IList<IDatabaseInitializeHandler> Handlers { get; set; }
        /// <summary>
        /// Entity Mapping Providers<br/>
        /// 实体映射构建器的列表<br/>
        /// </summary>
        protected IList<IEntityMappingProvider> Providers { get; set; }
        /// <summary>
        /// Database context pool<br/>
        /// 数据库上下文的缓存池<br/>
        /// </summary>
        protected internal EFCoreDbContextPool Pool { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="databaseType"></param>
        /// <param name="connectionString"></param>
        /// <param name="handlers"></param>
        /// <param name="providers"></param>
        public EFCoreDbContext(string databaseType, string connectionString, IList<IDatabaseInitializeHandler> handlers,
            IList<IEntityMappingProvider> providers) : base(databaseType, connectionString)
        {
            Transaction = null;
            TransactionLevel = 0;
            database = databaseType;
            Handlers = handlers;
            Providers = providers;
        }

        /// <inheritdoc />
        /// <summary>
        /// Configure entity model<br />
        /// 配置实体模型<br />
        /// </summary>
        /// <param name="modelBuilder">Model builder</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var entityProviders = Providers.GroupBy(p =>
                ReflectionUtils.GetGenericArguments(p.GetType(), typeof(IEntityMappingProvider<>))[0]).ToList();
            foreach (var group in entityProviders)
            {
                Activator.CreateInstance(
                    typeof(EFCoreEntityMappingBuilder<>).MakeGenericType(group.Key),
                    modelBuilder, Handlers, group.AsEnumerable());
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Dispose transaction, if pool exist then try return to pool, otherwise dispose the context<br />
        /// 释放事务, 如果池存在则尝试返回给池, 否则释放上下文<br />
        /// </summary>
        public override void Dispose()
        {
            Transaction?.Dispose();
            Transaction = null;
            if (Pool != null && Pool.Return(this))
            {
                // Returned to pool
            }
            else
            {
                base.Dispose();
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Begin a transaction<br />
        /// 开始一个事务<br />
        /// </summary>
        public void BeginTransaction(IsolationLevel? isolationLevel = null)
        {
            var level = Interlocked.Increment(ref TransactionLevel);
            if (level == 1)
            {
                if (Transaction != null)
                {
                    throw new InvalidOperationException("Transaction exists");
                }
                Transaction = (isolationLevel == null) ?
                    Database.BeginTransaction() :
                    Database.BeginTransaction(isolationLevel.Value);
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Finish the transaction<br />
        /// 结束一个事务<br />
        /// </summary>
        public void FinishTransaction()
        {
            var level = Interlocked.Decrement(ref TransactionLevel);
            if (level == 0)
            {
                if (Transaction == null)
                {
                    throw new InvalidOperationException("Transaction not exists");
                }
                Transaction.Commit();
                Transaction.Dispose();
                Transaction = null;
            }
            else if (level < 0)
            {
                Interlocked.Exchange(ref level, 0);
                throw new InvalidOperationException(
                    "You can't call FinishTransaction more times than BeginTransaction");
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Get the query object for specific entity type<br />
        /// 获取指定实体类型的查询对象<br />
        /// </summary>
        public IQueryable<T> Query<T>()
            where T : class, IEntity
        {
            return Set<T>();
        }

        #region 同步查询方法

        /// <inheritdoc />
        /// <summary>
        /// Get single entity that matched the given predicate<br />
        /// It should return null if no matched entity found<br />
        /// 获取符合传入条件的单个实体<br />
        /// 如果无符合条件的实体应该返回null<br />
        /// </summary>
        public T Get<T>(Expression<Func<T, bool>> predicate)
            where T : class, IEntity
        {
            return Query<T>().FirstOrDefault(predicate);
        }

        /// <inheritdoc />
        /// <summary>
        /// Get how many entities that matched the given predicate<br />
        /// 获取符合传入条件的实体数量<br />
        /// </summary>
        public long Count<T>(Expression<Func<T, bool>> predicate)
            where T : class, IEntity
        {
            return Query<T>().LongCount(predicate);
        }

        #endregion

        #region 异步查询方法

        /// <summary/>
        /// Get single entity that matched the given predicate<br />
        /// It should return null if no matched entity found<br />
        /// 获取符合传入条件的单个实体<br />
        /// 如果无符合条件的实体应该返回null<br />
        public async Task<T> GetAsync<T>(Expression<Func<T, bool>> predicate) where T : class, IEntity
        {
            return await Query<T>().FirstOrDefaultAsync(predicate);
        }

        /// <summary>
        /// Get how many entities that matched the given predicate<br />
        /// 获取符合传入条件的实体数量<br />
        /// </summary>
        public async Task<long> CountAsync<T>(Expression<Func<T, bool>> predicate)
            where T : class, IEntity
        {
            return await Query<T>().LongCountAsync(predicate);
        }


        #endregion

        #region 同步插入或更新方法

        /// <summary>
        /// Insert or update entity<br/>
        /// 插入或更新实体<br/>
        /// </summary>
        protected void InsertOrUpdate<T>(T entity, Action<T> update = null)
            where T : class, IEntity
        {
            var entityInfo = Entry(entity);
            update?.Invoke(entity);
            if (entityInfo.State == EntityState.Detached)
            {
                // It's not being tracked
                if (entityInfo.IsKeySet)
                {
                    // The key is not default, we're not sure it's in database or not
                    // check it first, it's rare so it shouldn't cause performance impact
                    var property = typeof(T).FastGetProperty(nameof(IEntity<object>.Id));
                    var id = property.FastGetValue(entity);
                    var expr = ExpressionUtils.MakeMemberEqualiventExpression<T>(property.Name, id);
                    if (Count(expr) > 0)
                    {
                        Update(entity);
                    }
                    else
                    {
                        Add(entity);
                    }
                }
                else
                {
                    // The key is default, set it's state to Added
                    Add(entity);
                }
            }
            else
            {
                // It's being tracked, we don't need to attach it
            }
        }




        /// <inheritdoc />
        /// <summary>
        /// Save entity to database<br />
        /// 保存实体到数据库<br />
        /// </summary>
        public void Save<T>(ref T entity, Action<T> update = null)
            where T : class, IEntity
        {
            var callbacks = Application.Ioc.GetServices<IEntityOperationHandler<T>>().ToList();
            var entityLocal = entity; // can't use ref parameter in lambda
            callbacks.ForEach(c => c.BeforeSave(this, entityLocal)); // notify before save
            InsertOrUpdate(entityLocal, update);
            SaveChanges(); // send commands to database
            callbacks.ForEach(c => c.AfterSave(this, entityLocal)); // notify after save
            entity = entityLocal;
        }

        /// <inheritdoc />
        /// <summary>
        /// Batch save entities<br />
        /// 批量保存实体<br />
        /// </summary>
        public void BatchSave<T>(ref IEnumerable<T> entities, Action<T> update = null)
            where T : class, IEntity
        {
            var entitiesLocal = entities.ToList();
            var callbacks = Application.Ioc.GetServices<IEntityOperationHandler<T>>().ToList();
            foreach (var entity in entitiesLocal)
            {
                callbacks.ForEach(c => c.BeforeSave(this, entity)); // notify before save
                InsertOrUpdate(entity, update);
            }
            SaveChanges(); // send commands to database
            foreach (var entity in entitiesLocal)
            {
                callbacks.ForEach(c => c.AfterSave(this, entity)); // notify after save
            }
            entities = entitiesLocal;
        }

        /// <inheritdoc />
        /// <summary>
        /// Batch update entities<br />
        /// 批量更新实体<br />
        /// </summary>
        public long BatchUpdate<T>(Expression<Func<T, bool>> predicate, Action<T> update)
            where T : class, IEntity
        {
            var entities = Query<T>().Where(predicate).AsEnumerable();
            BatchSave(ref entities, update);
            return entities.LongCount();
        }

        /// <inheritdoc />
        /// <summary>
        /// Batch save entities in faster way<br />
        /// 快速批量保存实体<br />
        /// </summary>
        public void FastBatchSave<T, TPrimaryKey>(IEnumerable<T> entities)
            where T : class, IEntity<TPrimaryKey>
        {
            foreach (var entity in entities)
            {
                InsertOrUpdate(entity);
            }
            SaveChanges(); // send commands to database
        }

        #endregion

        #region 异步插入或更新方法

        /// <summary>
        /// Insert or update entity<br/>
        /// 插入或更新实体<br/>
        /// </summary>
        //protected void InsertOrUpdateAcync<T>(T entity, Action<T> update = null)
        //    where T : class, IEntity
        //{
        //    var entityInfo = Entry(entity);
        //    update?.Invoke(entity);
        //    if (entityInfo.State == EntityState.Detached)
        //    {
        //        // It's not being tracked
        //        if (entityInfo.IsKeySet)
        //        {
        //            // The key is not default, we're not sure it's in database or not
        //            // check it first, it's rare so it shouldn't cause performance impact
        //            var property = typeof(T).FastGetProperty(nameof(IEntity<object>.Id));
        //            var id = property.FastGetValue(entity);
        //            var expr = ExpressionUtils.MakeMemberEqualiventExpression<T>(property.Name, id);
        //            if (Count(expr) > 0)
        //            {
        //                Update(entity);
        //            }
        //            else
        //            {
        //                Add(entity);
        //            }
        //        }
        //        else
        //        {
        //            // The key is default, set it's state to Added
        //            Add(entity);
        //        }
        //    }
        //    else
        //    {
        //        // It's being tracked, we don't need to attach it
        //    }
        //}

        #endregion

        #region 同步删除方法

        /// <inheritdoc />
        /// <summary>
        /// Delete entity from database<br />
        /// 删除数据库中的实体<br />
        /// </summary>
        public void Delete<T>(T entity)
            where T : class, IEntity
        {
            var callbacks = Application.Ioc.GetServices<IEntityOperationHandler<T>>().ToList();
            callbacks.ForEach(c => c.BeforeDelete(this, entity)); // notify before delete
            Remove(entity);
            SaveChanges(); // send commands to database
            callbacks.ForEach(c => c.AfterDelete(this, entity)); // notify after delete
        }

        /// <inheritdoc />
        /// <summary>
        /// Batch delete entities<br />
        /// 批量删除实体<br />
        /// </summary>
        public long BatchDelete<T>(Expression<Func<T, bool>> predicate, Action<T> beforeDelete)
            where T : class, IEntity
        {
            var entities = Query<T>().Where(predicate).ToList();
            var callbacks = Application.Ioc.GetServices<IEntityOperationHandler<T>>().ToList();
            foreach (var entity in entities)
            {
                beforeDelete?.Invoke(entity);
                callbacks.ForEach(c => c.BeforeDelete(this, entity)); // notify before delete
                Remove(entity);
            }
            SaveChanges(); // send commands to database
            foreach (var entity in entities)
            {
                callbacks.ForEach(c => c.AfterDelete(this, entity)); // notify after delete
            }
            return entities.Count;
        }

        /// <inheritdoc />
        /// <summary>
        /// Batch delete entities in faster way<br />
        /// 快速批量删除实体<br />
        /// </summary>
        public long FastBatchDelete<T, TPrimaryKey>(Expression<Func<T, bool>> predicate)
            where T : class, IEntity<TPrimaryKey>, new()
        {
            var entities = Query<T>().Where(predicate).Select(t => new T { Id = t.Id });
            var count = entities.LongCount();
            RemoveRange(entities);
            SaveChanges(); // send commands to database
            return count;
        }

        #endregion

        #region 异步删除方法
        #endregion



        /// <inheritdoc />
        /// <summary>
        /// Perform a raw update to database<br />
        /// 执行一个原生的更新操作<br />
        /// </summary>
        public long RawUpdate(object query, object parameters)
        {
            return Database.ExecuteSqlCommand((string)query, (object[])parameters);
        }

        /// <inheritdoc />
        /// <summary>
        /// Perform a raw query to database<br />
        /// 执行一个原生的查询操作<br />
        /// </summary>
        public IEnumerable<T> RawQuery<T>(object query, object parameters)
            where T : class
        {
            return Set<T>().FromSql((string)query, (object[])parameters);
        }
    }
}
