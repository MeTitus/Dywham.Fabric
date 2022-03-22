using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Dywham.Fabric.Providers;

namespace Dywham.Fabric.Providers.IO.Compression
{
    public interface ICompressionProvider : IProvider
    {
        string CompressText(string text);

        string UnCompressText(string text);

        string Uncompress(string file, string destination = "");

        void CompressFromDirectory(string sourceDirectory, string destinationFileName);

        void ExtractToDirectory(string sourceArchiveFileName, string destinationDirectoryName);

        void ExtractToDirectory(string sourceArchiveFileName, string destinationDirectoryName, Encoding entryNameEncoding);

        Task<List<ZipFileEntry>> ExtractFilesAsync(Stream fileData);

        Task<IEnumerable<ZipFileEntry>> ExtractFilesLazyAsync(Stream fileData);
    }
}