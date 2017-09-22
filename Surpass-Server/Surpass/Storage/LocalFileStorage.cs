using Microsoft.Extensions.DependencyInjection;

namespace Surpass.Storage {
	/// <inheritdoc />
	/// <summary>
	/// Local file storage<br />
	/// 本地的文件系统<br />
	/// </summary>
	internal class LocalFileStorage : IFileStorage {
        /// <inheritdoc />
        /// <summary>
        /// Get template file, it should be readonly<br />
        /// 获取模板文件, 返回的文件应该是只读的<br />
        /// </summary>
        /// <summary>
        /// Get resource file, it should be readonly<br />
        /// 获取资源文件, 返回的文件应该是只读的<br />
        /// </summary>
        /// <summary>
        /// Get storage file<br />
        /// 获取储存文件<br />
        /// </summary>
        public IFileEntry GetStorageFile(params string[] pathParts)
        {
            var pathManager = Application.Provider.GetService<LocalPathManager>();
            var fullPath = pathManager.GetStorageFullPath(pathParts);
            return new LocalFileEntry(fullPath, false);
        }

	    /// <inheritdoc />
	    /// <summary>
	    /// Get storage directory<br />
	    /// 获取储存目录<br />
	    /// </summary>
	    public IDirectoryEntry GetStorageDirectory(params string[] pathParts)
	    {
	        var pathManager = Application.Provider.GetService<LocalPathManager>();
	        var fullPath = pathManager.GetStorageFullPath(pathParts);
	        return new LocalDirectoryEntry(fullPath);
	    }
    }
}
