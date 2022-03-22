using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dywham.Fabric.Providers.IO
{
    // ReSharper disable once InconsistentNaming
    public class IOProvider : IIOProvider
    {
        public void Write(string path, byte[] bytes)
        {
            File.WriteAllBytes(path, bytes);
        }

        public void Write(string path, Stream stream)
        {
            using (var fileStream = File.Create(path))
            {
                if (stream.CanSeek)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                }

                stream.CopyTo(fileStream);
                stream.Flush();
            }
        }

        public async Task WriteAsync(string path, Stream streamSource, CancellationToken token = default)
        {
            using (var stream = new FileStream(path, FileMode.CreateNew))
            {
                if (stream.CanSeek)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                }

                await streamSource.CopyToAsync(stream);

                await stream.FlushAsync(token);
            }
        }

        public void Write(string path, string text, Encoding encoding)
        {
            using (var stream = new StreamWriter(new FileStream(path, FileMode.CreateNew), encoding))
            {
                stream.Write(text);
                stream.Flush();
            }
        }

        public bool IsLocalPath(string path)
        {
            return !path.StartsWith("http:\\") && new Uri(path).IsFile;
        }

        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }

        public string ReadAllText(string path, Encoding encoding)
        {
            return File.ReadAllText(path, encoding);
        }

        public IEnumerable<string> EnumerateFiles(string path)
        {
            return Directory.EnumerateFiles(path);
        }

        public void WriteAllText(string filename, string text)
        {
            File.WriteAllText(filename, text);
        }

        public void WriteAllText(string filename, string text, Encoding encoding)
        {
            File.WriteAllText(filename, text, encoding);
        }

        public string Compress(string directory, string filename)
        {
            ZipFile.CreateFromDirectory(directory, filename);

            return filename;
        }

        public Stream OpenRead(string file)
        {
            return File.OpenRead(file);
        }

        public string GetTempFileName(bool create = true)
        {
            var path = Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            if (create)
            {
                using (File.Create(path))
                { }
            }

            return path;
        }

        public string GenerateTempDirectoryPath(bool create = false)
        {
            var path = Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            if (create)
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }

        public string Copy(string sourceFileName, bool overwriteIfNewer = true)
        {
            return Copy(sourceFileName, Path.GetTempPath(), overwriteIfNewer);
        }

        public string Copy(string sourceFileName, string destDirectory, bool overwriteIfNewer = true)
        {
            return Copy(sourceFileName, destDirectory, new FileInfo(sourceFileName).Name, overwriteIfNewer);
        }

        public string Copy(string sourceFileName, string destDirectory, string fileName, bool overwriteIfNewer = true)
        {
            if (!File.Exists(sourceFileName))
            {
                throw new FileNotFoundException("File not found", sourceFileName);
            }

            var file = new FileInfo(sourceFileName);
            var destFile = new FileInfo(Path.Combine(destDirectory, fileName));

            if (!file.Exists || file.LastWriteTime > destFile.LastWriteTime && overwriteIfNewer)
            {
                file.CopyTo(destFile.FullName, overwriteIfNewer);
            }

            return destFile.FullName;
        }

        public string Move(string sourceFileName, string destDirectory)
        {
            var file = new FileInfo(sourceFileName);
            var destFile = new FileInfo(Path.Combine(destDirectory, file.Name));

            file.MoveTo(destFile.FullName);

            return destFile.FullName;
        }

        public bool Exists(string folder)
        {
            try
            {
                return Directory.Exists(folder) || File.Exists(folder);
            }
            catch
            {
                return false;
            }
        }

        public string GetFileNameWithoutExtension(string file)
        {
            return Path.GetFileNameWithoutExtension(file);
        }

        public string GetFileName(string file)
        {
            return Path.GetFileName(file);
        }

        public string Combine(params string[] paths)
        {
            return Path.Combine(paths);
        }

        public void Delete(string file)
        {
            File.Delete(file);
        }

        public string GetExtension(string file)
        {
            return Path.GetExtension(file);
        }

        public string GetFullPath(string path)
        {
            return Path.GetFullPath(path);
        }

        public string ReadAsBase64(string fileLocation)
        {
            return Convert.ToBase64String(File.ReadAllBytes(fileLocation));
        }

        public async Task<string> ReadAsBase64Async(string fileLocation, CancellationToken token = default)
        {
            return Convert.ToBase64String(await ReadAsync(fileLocation, token));
        }

        public async Task<byte[]> ReadAsync(string fileLocation, CancellationToken token)
        {
            byte[] result;

            await using (var stream = File.Open(fileLocation, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                result = new byte[stream.Length];

                // ReSharper disable once UnusedVariable
                var nBytes = await stream.ReadAsync(result, 0, (int)stream.Length, token);
            }

            return result;
        }

        public string[] GetFiles(string folder)
        {
            return Directory.GetFiles(folder);
        }

        public string[] GetFiles(string folder, string searchPattern)
        {
            return Directory.GetFiles(folder, searchPattern);
        }

        public string[] GetFiles(string folder, string searchPattern, bool topDirectoryOnly)
        {
            return Directory.GetFiles(folder, searchPattern, topDirectoryOnly ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories);
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public Encoding GetFileEncoding(string filename)
        {
            var bom = new byte[4];

            using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                // ReSharper disable once MustUseReturnValue
                file.Read(bom, 0, 4);
            }

#pragma warning disable CS0618
#pragma warning disable SYSLIB0001 // Type or member is obsolete
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
#pragma warning restore SYSLIB0001 // Type or member is obsolete
#pragma warning restore CS0618

            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;

            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE

            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE

            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;

            return Encoding.ASCII;
        }

        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }
        public void RecreateDirectory(string directoryPath)
        {
            if (Directory.Exists(directoryPath))
            {
                Directory.Delete(directoryPath, true);
            }

            Directory.CreateDirectory(directoryPath!);
        }

        public void CopyDirectory(string sourceDirName, string destDirName, bool copySubDirs = true)
        {
            var dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException($"Source directory does not exist or could not be found: {sourceDirName}");
            }

            var dirs = dir.GetDirectories();

            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName!);
            }

            var files = dir.GetFiles();

            foreach (var file in files)
            {
                var tempPath = Path.Combine(destDirName, file.Name);

                file.CopyTo(tempPath, false);
            }

            if (!copySubDirs) return;

            foreach (var subDir in dirs)
            {
                var tempPath = Path.Combine(destDirName, subDir.Name);

                CopyDirectory(subDir.FullName, tempPath);
            }
        }

        public void MoveDirectory(string source, string destination)
        {
            CopyDirectory(source, destination);

            DeleteDirectory(source, true);
        }

        public string GetDirectoryName(string path)
        {
            return Path.GetDirectoryName(path);
        }

        public string[] GetDirectories(string path)
        {
            return Directory.GetDirectories(path);
        }
        public string[] GetDirectories(string path, string searchPattern)
        {
            return Directory.GetDirectories(path, searchPattern);
        }

        public string[] GetDirectories(string path, string searchPattern, bool topDirectoryOnly)
        {
            return Directory.GetDirectories(path, searchPattern, topDirectoryOnly ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories);
        }

        public void DeleteDirectory(string directory, bool recursive)
        {
            Directory.Delete(directory, recursive);
        }

        public void ConvertFileEncoding(string sourcePath, string destPath, Encoding sourceEncoding, Encoding destEncoding)
        {
            var parent = Path.GetDirectoryName(Path.GetFullPath(destPath));

            if (!Directory.Exists(parent))
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                Directory.CreateDirectory(parent);
            }

            // If the source and destination encodings are the same, just copy the file.
            if (Equals(sourceEncoding, destEncoding))
            {
                File.Copy(sourcePath, destPath, true);

                return;
            }

            // Convert the file.
            string tempName = null;

            try
            {
                tempName = Path.GetTempFileName();

                using (var sr = new StreamReader(sourcePath, sourceEncoding, false))
                {
                    using (var sw = new StreamWriter(tempName, false, destEncoding))
                    {
                        int charsRead;
                        var buffer = new char[128 * 1024];

                        while ((charsRead = sr.ReadBlock(buffer, 0, buffer.Length)) > 0)
                        {
                            sw.Write(buffer, 0, charsRead);
                        }
                    }
                }

                File.Delete(destPath);
                File.Move(tempName, destPath);
            }
            finally
            {
                if (tempName != null)
                {
                    File.Delete(tempName);
                }
            }
        }
    }
}