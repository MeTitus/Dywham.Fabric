using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dywham.Fabric.Providers;

namespace Dywham.Fabric.Providers.IO
{
    // ReSharper disable once InconsistentNaming
    public interface IIOProvider : IProvider
    {
        IEnumerable<string> EnumerateFiles(string path);

        void WriteAllText(string filename, string text);

        void WriteAllText(string filename, string text, Encoding encoding);

        void Write(string path, byte[] bytes);

        void Write(string path, Stream stream);

        Task WriteAsync(string path, Stream streamSource, CancellationToken token = default);

        void Write(string path, string text, Encoding encoding);

        bool IsLocalPath(string path);

        string Copy(string sourceFileName, bool overwriteIfNewer = true);

        string Copy(string sourceFileName, string destDirectory, bool overwriteIfNewer = true);

        string Copy(string sourceFileName, string destDirectory, string fileName, bool overwriteIfNewer = true);

        string Move(string sourceFileName, string destDirectory);

        Stream OpenRead(string file);

        string GetTempFileName(bool create = true);

        string GenerateTempDirectoryPath(bool create = false);

        string GetDirectoryName(string path);

        string[] GetFiles(string folder);

        string[] GetFiles(string folder, string searchPattern);

        string[] GetFiles(string folder, string searchPattern, bool topDirectoryOnly);

        bool Exists(string folder);

        bool FileExists(string path);

        bool DirectoryExists(string path);

        string GetFileNameWithoutExtension(string file);

        string GetFileName(string file);

        string Combine(params string[] paths);

        void Delete(string file);

        string GetExtension(string file);

        string GetFullPath(string path);

        string ReadAllText(string path);

        string ReadAllText(string path, Encoding encoding);

        string ReadAsBase64(string fileLocation);

        Task<string> ReadAsBase64Async(string fileLocation, CancellationToken token = default);

        Task<byte[]> ReadAsync(string fileLocation, CancellationToken token = default);

        Encoding GetFileEncoding(string filename);

        string[] GetDirectories(string path);

        string[] GetDirectories(string path, string searchPattern);

        string[] GetDirectories(string path, string searchPattern, bool topDirectoryOnly);

        void CreateDirectory(string path);

        void RecreateDirectory(string directoryPath);

        void CopyDirectory(string sourceDirName, string destDirName, bool copySubDirs = true);

        void MoveDirectory(string source, string destination);

        void DeleteDirectory(string directory, bool recursive);

        void ConvertFileEncoding(string sourcePath, string destPath, Encoding sourceEncoding, Encoding destEncoding);
    }
}