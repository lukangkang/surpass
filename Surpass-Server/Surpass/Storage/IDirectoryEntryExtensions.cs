

//using ZKWebStandard.Utils;

namespace Surpass.Storage
{
    /// <summary>
    /// 文件目录的扩展函数
    /// </summary>
    public static class IDirectoryEntryExtensions
    {
        /// <summary>
        /// 压缩文件夹
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="zipedFile"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        //public static bool ZipDirectory(this IDirectoryEntry entry, string zipedFile, string password = null)
        //{
        //    return ZipUtils.ZipDirectory(entry.FullPath, zipedFile, password);
        //}


        //private static bool ZipDirectory(this IDirectoryEntry entry, ZipOutputStream zipStream, string parentFolderName)
        //{
        //    var result = true;
        //    IEnumerable<IDirectoryEntry> folders;
        //    IEnumerable<IFileEntry> files;
        //    ZipEntry ent = null;
        //    FileStream fs = null;
        //    Crc32 crc = new Crc32();

        //    try
        //    {
        //        ent = new ZipEntry(Path.Combine(parentFolderName, entry.DirectoryName + "/"));
        //        zipStream.PutNextEntry(ent);
        //        zipStream.Flush();

        //        files = entry.EnumerateFiles();

        //        foreach (var file in files)
        //        {
        //            fs = file.OpenRead() as FileStream;

        //            byte[] buffer = new byte[fs.Length];
        //            fs.Read(buffer, 0, buffer.Length);
        //            ent = new ZipEntry(Path.Combine(parentFolderName, entry.DirectoryName + "/" + file.Filename));
        //            ent.DateTime = DateTime.Now;
        //            ent.Size = fs.Length;

        //            fs.Close();

        //            crc.Reset();
        //            crc.Update(buffer);

        //            ent.Crc = crc.Value;
        //            zipStream.PutNextEntry(ent);
        //            zipStream.Write(buffer, 0, buffer.Length);
        //        }

        //    }
        //    catch
        //    {
        //        result = false;
        //    }
        //    finally
        //    {
        //        if (fs != null)
        //        {
        //            fs.Close();
        //            fs.Dispose();
        //        }
        //        if (ent != null)
        //        {
        //            ent = null;
        //        }
        //        GC.Collect();
        //        GC.Collect(1);
        //    }

        //    folders = entry.EnumerateDirectories();
        //    foreach (var folder in folders)
        //    {

        //    }

        //    foreach (var  folder in folders)
        //        if (!ZipDirectory(folder, zipStream, folderToZip))
        //            return false;
        //    return result;
        //}
    }
}
