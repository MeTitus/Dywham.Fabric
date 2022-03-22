using System.Threading;
using System.Threading.Tasks;

namespace Dywham.Fabric.Providers.Network
{
    public interface IHttpProvider : IProvider
    {
        byte[] DownloadFile(string url);

        Task<byte[]> DownloadFileAsync(string url);

        Task<byte[]> DownloadFileAsync(string url, CancellationToken token);

        string DownloadFileTo(string url);

        string DownloadFileTo(string url, string location);

        Task<string> DownloadFileToAsync(string url, string location);

        Task<string> DownloadFileToAsync(string url, string location, CancellationToken token);
    }
}