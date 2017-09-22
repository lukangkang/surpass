using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Surpass.Plugin.AssemblyLoaders;
using Surpass.Storage;
using SurpassStandard.Options;
using SurpassStandard.Utils;

namespace Surpass.Plugin
{
    /// <summary>
    /// 插件管理器
    /// </summary>
    public sealed class PluginManager
    {
        /// <summary>
        /// Plugins<br/>
        /// 插件列表<br/>
        /// </summary>
        public IList<PluginInfo> Plugins { get; }
        /// <summary>
        /// Plugin assemblies<br/>
        /// 插件程序集列表<br/>
        /// </summary>
        public IList<Assembly> PluginAssemblies { get; }

        private readonly IOptionsMonitor<PluginOptions> _pluginOptions;

        private readonly LocalPathManager _localPathManager;

        /// <summary>
        /// Initialize<br/>
        /// 初始化<br/>
        /// </summary>
        public PluginManager(IOptionsMonitor<PluginOptions> pluginOptions)
        {
            _pluginOptions = pluginOptions;
            _localPathManager = Application.Provider.GetService<LocalPathManager>();
            Plugins = new List<PluginInfo>();
            PluginAssemblies = new List<Assembly>();
        }

        /// <summary>
        /// todo 这里是一个假的插件管理器，以后可能会实现插件化，目前只用来注册仓储和领域层的类
        /// </summary>
        internal void Intialize()
        {
            Plugins.Clear();
            PluginAssemblies.Clear();

            //从网站配置文件获取插件名
            var pluginDirectories = _localPathManager.GetPluginDirectories();
            foreach (var pluginName in _pluginOptions.CurrentValue)
            {
                var dir = pluginDirectories
                    .Select(p => PathUtils.SecureCombine(p, pluginName))
                    .FirstOrDefault(Directory.Exists);
                if (dir == null)
                {
                    throw new DirectoryNotFoundException($"Plugin directory of {pluginName} not found");
                }
                var info = PluginInfo.FromDirectory(dir);
                Plugins.Add(info);
            }

            //加载插件
            var assemblyLoader = Application.Provider.GetService<IAssemblyLoader>();
            foreach (var plugin in Plugins)
            {
                // todo 编译插件，目前不是插件不自动编译
                //plugin.Compile();
                // Load compiled assembly, some plugin may not have an assembly
                var assemblyPath = plugin.AssemblyPath();
                if (File.Exists(assemblyPath))
                {
                    var assembly = assemblyLoader.LoadFile(assemblyPath);
                    plugin.Assembly = assembly;
                    PluginAssemblies.Add(assembly);
                }
            }
        }
    }
}
