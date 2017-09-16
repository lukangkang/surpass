using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MySQL.Data.EntityFrameworkCore.Extensions;

namespace Surpass.ORM.EFCore
{
    public class EFCoreDbContextBase:DbContext
    {
        /// <summary>
        /// 数据库类型
        /// </summary>
        protected string DatabaseType { get; set; }

        /// <summary>
        /// /连接字符串
        /// </summary>
        protected string ConnectionString { get; set; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="database"></param>
        /// <param name="connectionString"></param>
        public EFCoreDbContextBase(string database, string connectionString)
        {
            DatabaseType = database;
            ConnectionString = connectionString;
        }

        /// <inheritdoc />
        /// <summary>
        /// 配置上下文选项
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (DatabaseType.Equals("Mssql"))
            {
                optionsBuilder.UseSqlServer(ConnectionString, option => option.UseRowNumberForPaging());
            }
            else if (DatabaseType.Equals("Mysql"))
            {
                optionsBuilder.UseMySQL(ConnectionString);
            }
            else
            {
                throw new ArgumentException("不支持的数据库类型");
            }

            // EF 2.0 make some warnings as error, just ignore them
            optionsBuilder.ConfigureWarnings(w => w.Ignore(CoreEventId.IncludeIgnoredWarning));
        }

        /// <summary>
        /// Configure entity model<br/>
        /// 配置实体模型<br/>
        /// </summary>
        /// <param name="modelBuilder">Model builder</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            new EFCoreMigrationHistory().Configure(modelBuilder);
        }
    }
}
