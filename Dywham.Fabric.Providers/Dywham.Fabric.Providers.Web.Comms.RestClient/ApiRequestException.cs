using System.Net;
using System.Net.Http;

namespace Dywham.Fabric.Providers.Web.Comms.RestClient
{
    public class ApiRequestException : HttpRequestException
    {
        public ApiRequestException(string content, string url, HttpStatusCode? statusCode) : base(content, null, statusCode)
        {
            Url = url;
        }

        
        public string Url { get; }
    }
}
