using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Dywham.Fabric.Providers.Network
{
    public class HttpProvider : IHttpProvider
    {
        public byte[] DownloadFile(string url)
        {
            return DownloadFileAsync(url).GetAwaiter().GetResult();
        }

        public async Task<byte[]> DownloadFileAsync(string url)
        {
            return await DownloadFileAsync(url, CancellationToken.None);
        }

        public string DownloadFileTo(string url)
        {
            return DownloadFileTo(url, Path.GetTempFileName());
        }

        public string DownloadFileTo(string url, string location)
        {
            return DownloadFileToAsync(url, location).GetAwaiter().GetResult();
        }

        public async Task<string> DownloadFileToAsync(string url, string location)
        {
            return await DownloadFileToAsync(url, location, CancellationToken.None);
        }

        public async Task<string> DownloadFileToAsync(string url, string location, CancellationToken token)
        {
            var bytes = await DownloadFileAsync(url, token);

            if(bytes == null) return location;

            File.WriteAllBytes(location, bytes);

            return location;
        }

        public async Task<byte[]> DownloadFileAsync(string url, CancellationToken token)
        {
            using (var client = new HttpClient())
            {
                using (var result = await client.GetAsync(url, token))
                {
                    if (result.IsSuccessStatusCode)
                    {
                        return await result.Content.ReadAsByteArrayAsync();
                    }

                    throw new HttpRequestException(result.StatusCode.ToString());
                }
            }
        }
    }
}