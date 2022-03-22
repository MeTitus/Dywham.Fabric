using System.Collections.Generic;
using System.Net;

namespace Dywham.Fabric.Providers.Web.Comms.RestClient
{
    public class DywhamRestResponse
    {
        public DywhamRestResponse(Dictionary<string, IEnumerable<string>> headers, bool success, HttpStatusCode httpStatusResponse)
        {
            Headers = headers;

            Success = success;

            HttpStatusResponse = httpStatusResponse;
        }


        public bool Success { get; }

        public HttpStatusCode HttpStatusResponse { get; }

        public Dictionary<string, IEnumerable<string>> Headers { get; }
    }

    public class DywhamHttpResponse<T> : DywhamRestResponse
    {
        public DywhamHttpResponse(Dictionary<string, IEnumerable<string>> headers, bool success, HttpStatusCode httpStatusResponse) : base(headers, success, httpStatusResponse)
        { }

        public DywhamHttpResponse(T data, Dictionary<string, IEnumerable<string>> headers, bool success, HttpStatusCode httpStatusResponse) : base(headers, success, httpStatusResponse)
        {
            Data = data;
        }


        public T Data { get; }
    }
}