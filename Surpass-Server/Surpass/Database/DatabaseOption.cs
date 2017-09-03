using System;
using System.Collections.Generic;
using System.Text;

namespace Surpass.Database
{
    /// <summary>
    /// 其他数据库
    /// </summary>
    public class DatabaseOption
    {
        /// <summary>
        /// 数据库连接项的名称(自定义，唯一)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ConnectionString { get; set; }

    }
}
