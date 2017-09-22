using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Surpass.Infrastructure.Database;
using Surpass.Plugin.AssemblyLoaders;
using SurpassStandard.Options;

namespace Surpass.Database {
	/// <summary>
	/// Database manager<br/>
	/// 数据库管理器<br/>
	/// </summary>
	/// <example>
	/// <code>
	/// var databaseManager = Application.Provider.Resolve&lt;DatabaseManager&gt;();
	///	using (var context = databaseManager.CreateContext()) {
	///		var data = new ExampleTable() {
	///			Name = "test",
	///			CreateTime = DateTime.UtcNow,
	///			Deleted = false
	///		};
	///		context.Save(ref data);
	/// }
	/// </code>
	/// </example>
	/// <seealso cref="IDatabaseContext"/>
	/// <seealso cref="IDatabaseContextFactory"/>
	public class DatabaseManager {
		/// <summary>
		/// Default database context factory<br/>
		/// 默认的数据库上下文生成器<br/>
		/// </summary>
		protected IDatabaseContextFactory DefaultContextFactory { get; set; }

	    private readonly IOptionsMonitor<DatabaseOptions> _options;      

	    public DatabaseManager(IOptionsMonitor<DatabaseOptions> options) 
	    {
	        _options = options;
	    }

        /// <summary>
        /// Create database context from the default factory<br/>
        /// 使用默认的生成器创建数据库上下文<br/>
        /// </summary>
        /// <returns></returns>
        public virtual IDatabaseContext CreateContext() {
			return DefaultContextFactory.CreateContext();
		}

	    /// <summary>
	    /// Create database context factory from the given parameters<br/>
	    /// 根据传入的参数创建数据库上下文生成器<br/>
	    /// </summary>
	    /// <param name="orm">Object relational mapper</param>
	    /// <param name="database">Database name</param>
	    /// <param name="connectionString">Database connection string</param>
	    /// <param name="handlers"></param>
	    /// <param name="providers"></param>
	    /// <returns></returns>
	    public static IDatabaseContextFactory CreateContextFactor(
	        string orm, string database, string connectionString, IEnumerable<IDatabaseInitializeHandler> handlers = null,
	        IEnumerable<IEntityMappingProvider> providers = null)
	    {
	        if (string.IsNullOrEmpty(orm))
	        {
	            throw new NotSupportedException("No ORM name is provided, please set it first");
	        }
	        var assemblyName = string.Format("Surpass.ORM.{0}", orm);
	        var assemblyLoader = Application.Provider.GetService<IAssemblyLoader>();
	        Assembly assembly;
	        try
	        {
	            assembly = assemblyLoader.Load(assemblyName);
	        }
	        catch (Exception e)
	        {
	            throw new NotSupportedException(string.Format(
	                "Load ORM assembly {0} failed, please install it first. error: {1}", orm, e.Message));
	        }
	        var factorType = assembly.GetTypes().FirstOrDefault(t =>
	            typeof(IDatabaseContextFactory).IsAssignableFrom(t));
	        if (factorType == null)
	        {
	            throw new NotSupportedException(string.Format(
	                "Find factory type from ORM {0} failed", orm));
	        }
	        if (providers != null && handlers == null)
	        {
	            handlers = new IDatabaseInitializeHandler[0];
	        }
	        if (handlers != null && providers != null)
	            return (IDatabaseContextFactory)Activator.CreateInstance(factorType, database, connectionString, handlers, providers);
	        return (IDatabaseContextFactory)Activator.CreateInstance(factorType, database, connectionString);
        }

        /// <summary>
        /// Initialize database manager<br/>
        /// 初始化数据库管理器<br/>
        /// </summary>
        protected internal virtual void Initialize()
        {
            var defaultDatabase = _options.CurrentValue.FirstOrDefault(x => x.Name == "Default");
            if (defaultDatabase == null)
                throw new KeyNotFoundException("主数据库配置项缺失");

            DefaultContextFactory = CreateContextFactor("EFCore", defaultDatabase.Database, defaultDatabase.ConnectionString);
        }

        /// <summary>
        /// Use temporary database in the specified scope<br/>
        /// 在指定范围内使用临时数据库<br/>
        /// </summary>
        /// <returns></returns>
        //public virtual IDisposable UseTemporaryDatabase(string key, IEnumerable<IDatabaseInitializeHandler> handlers = null,
        //    IEnumerable<IEntityMappingProvider> providers = null)
        //{
        //    var log = Application.Provider.Resolve<LogManager>();
        //    // Create database context factory, default use nhibernate orm
        //    var configManager = Application.Provider.Resolve<WebsiteConfigManager>();
        //    var databases = configManager.WebsiteConfig.ExtensionDatabases;
        //    var orm = databases.FirstOrDefault(x => x.Key.Equals(key)).ORM ?? "NHibernate";
        //    //datatbases.GetOrDefault<string>(ExtraConfigKeys.TemporaryDatabaseORM) ?? "NHibernate";
        //    var database = databases.FirstOrDefault(x => x.Key.Equals(key)).Database ?? "MSSQL";
        //    //extra.GetOrDefault<string>(ExtraConfigKeys.TemporaryDatabaseType);
        //    var connectionString = databases.FirstOrDefault(x => x.Key.Equals(key)).ConnectionString;
        //    //extra.GetOrDefault<string>(ExtraConfigKeys.TemporaryDatabaseConnectionString);
        //    var contextFactory = CreateContextFactor(orm, database, connectionString, handlers, providers);
        //    // Override database manager with above factory
        //    var overrideIoc = Application.OverrideIoc();
        //    var databaseManagerMock = Substitute.For<DatabaseManager>();
        //    databaseManagerMock.CreateContext().Returns(callInfo => contextFactory.CreateContext());
        //    Application.Provider.Unregister<DatabaseManager>();
        //    Application.Provider.RegisterInstance(databaseManagerMock);
        //    // Finish override when disposed
        //    return overrideIoc;
        //}
    }
}
