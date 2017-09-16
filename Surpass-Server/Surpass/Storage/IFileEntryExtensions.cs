using System.IO;
using SurpassStandard.Utils;

namespace Surpass.Storage {
	/// <summary>
	/// File entry extension methods<br/>
	/// 文件的扩展函数<br/>
	/// </summary>
	public static class IFileEntryExtensions {
		/// <summary>
		/// Read file into string<br/>
		/// 读取文件内容到字符串<br/>
		/// </summary>
		/// <param name="entry">File entry</param>
		/// <returns></returns>
		public static string ReadAllText(this IFileEntry entry) {
			using (var stream = entry.OpenRead())
			using (var reader = new StreamReader(stream)) {
				return reader.ReadToEnd();
			}
		}

		/// <summary>
		/// Write string to file<br/>
		/// 写入字符串到文件<br/>
		/// </summary>
		/// <param name="entry">File entry</param>
		/// <param name="value">String value</param>
		public static void WriteAllText(this IFileEntry entry, string value) {
			using (var stream = entry.OpenWrite())
			using (var writer = new StreamWriter(stream)) {
				writer.Write(value);
			}
		}

		/// <summary>
		/// Append string to file<br/>
		/// 追加字符串到文件末尾<br/>
		/// </summary>
		/// <param name="entry">File entry</param>
		/// <param name="value">String value</param>
		public static void AppendAllText(this IFileEntry entry, string value) {
			using (var stream = entry.OpenAppend())
			using (var writer = new StreamWriter(stream)) {
				writer.Write(value);
			}
		}

		/// <summary>
		/// Read file into byte array<br/>
		/// 读取文件到字节数组<br/>
		/// </summary>
		/// <param name="entry">File entry</param>
		/// <returns></returns>
		public static byte[] ReadAllBytes(this IFileEntry entry) {
			using (var stream = entry.OpenRead())
			using (var memoryStream = new MemoryStream()) {
				stream.CopyTo(memoryStream);
				return memoryStream.ToArray();
			}
		}

		/// <summary>
		/// Write byte array to file<br/>
		/// 写入字节数组到文件<br/>
		/// </summary>
		/// <param name="entry">File entry</param>
		/// <param name="bytes">Byte array</param>
		public static void WriteAllBytes(this IFileEntry entry, byte[] bytes) {
			using (var stream = entry.OpenWrite()) {
				stream.Write(bytes, 0, bytes.Length);
			}
		}

	    /// <summary>
	    /// 压缩文件
	    /// </summary>
	    /// <param name="entry"></param>
	    /// <param name="zipedFile"></param>
	    /// <param name="password"></param>
	    /// <returns></returns>
	    public static bool Zip(this IFileEntry entry, string zipedFile, string password = null)
	    {
	        //return ZipUtils.ZipFile(entry.UniqueIdentifier, zipedFile, password);
	        return true;
	    }

        /// <summary>
        /// 解压缩文件
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="zipedFolder">请使用PathUtils.SecureCombine()</param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool UnZip(this IFileEntry entry, string zipedFolder, string password = null)
        {
            //return ZipUtils.UnZip(entry.UniqueIdentifier, zipedFolder, password);
            return true;
        }

    }
}
