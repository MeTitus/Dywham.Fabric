using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Dywham.Fabric.Providers.Web.Comms.RestClient
{
    public class DywhamRestFileResponse : DywhamRestResponse, IDisposable
    {
        public DywhamRestFileResponse(Dictionary<string, IEnumerable<string>> headers, bool success,
            HttpStatusCode httpStatusResponse, Stream stream) : base(headers, success, httpStatusResponse)
        {
            Stream = stream;
        }


        public Stream Stream { get; }


        public void Dispose()
        {
            Stream?.Dispose();
        }
    }
}