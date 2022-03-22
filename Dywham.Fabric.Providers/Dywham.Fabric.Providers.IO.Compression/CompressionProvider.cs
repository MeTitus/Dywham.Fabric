using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;

namespace Dywham.Fabric.Providers.IO.Compression
{
    public class CompressionProvider : ICompressionProvider
    {
        public string CompressText(string text)
        {
            using (var ms = new MemoryStream())
            {
                using (var gzip = new GZipStream(ms, CompressionMode.Compress))
                using (var writer = new BinaryWriter(gzip, Encoding.UTF8))
                {
                    writer.Write(text);
                }

                ms.Flush();

                return Convert.ToBase64String(ms.ToArray());
            }
        }

        public string UnCompressText(string text)
        {
            var itemData = Convert.FromBase64String(text);

            using (var src = new MemoryStream(itemData))
            using (var gzs = new GZipStream(src, CompressionMode.Decompress))
            using (var reader = new BinaryReader(gzs, Encoding.UTF8))
            {
                return reader.ReadString();
            }
        }

        public string Uncompress(string file, string destination = "")
        {
            if (string.IsNullOrEmpty(destination))
            {
                destination = Path.GetTempPath();
            }

            destination = Path.Combine(destination, Path.GetFileNameWithoutExtension(file));

            // ReSharper disable once AssignNullToNotNullAttribute
           System.IO.Compression.ZipFile.ExtractToDirectory(file, destination);

            return destination;
        }

        public void CompressFromDirectory(string sourceDirectory, string destinationFileName)
        {
            System.IO.Compression.ZipFile.CreateFromDirectory(sourceDirectory, destinationFileName, CompressionLevel.Optimal, false);
        }

        public void ExtractToDirectory(string sourceArchiveFileName, string destinationDirectoryName)
        {
            System.IO.Compression.ZipFile.ExtractToDirectory(sourceArchiveFileName, destinationDirectoryName);
        }

        public void ExtractToDirectory(string sourceArchiveFileName, string destinationDirectoryName, Encoding entryNameEncoding)
        {
            System.IO.Compression.ZipFile.ExtractToDirectory(sourceArchiveFileName, destinationDirectoryName, entryNameEncoding);
        }

        public async Task<IEnumerable<ZipFileEntry>> ExtractFilesLazyAsync(Stream fileData)
        {
            var entries = new List<ZipFileEntry>();

            await using (var zipInputStream = new ZipInputStream(fileData))
            {
                while (zipInputStream.GetNextEntry() is ZipEntry zipEntry)
                {
                    await using (var memoryStream = new MemoryStream())
                    {
                        await zipInputStream.CopyToAsync(memoryStream);

                        entries.Add(new ZipFileEntry
                        {
                            Name = zipEntry.Name,
                            Content = memoryStream.ToArray()
                        });
                    }
                }
            }

            return entries;
        }

        public async Task<List<ZipFileEntry>> ExtractFilesAsync(Stream fileData)
        {
            var entries = new List<ZipFileEntry>();

            await using (var zipInputStream = new ZipInputStream(fileData))
            {
                while (zipInputStream.GetNextEntry() is ZipEntry zipEntry)
                {
                    await using (var memoryStream = new MemoryStream())
                    {
                        await zipInputStream.CopyToAsync(memoryStream);

                        entries.Add(new ZipFileEntry
                        {
                            Name = zipEntry.Name,
                            Content = memoryStream.ToArray()
                        });
                    }
                }
            }

            return entries;
        }
    }
}