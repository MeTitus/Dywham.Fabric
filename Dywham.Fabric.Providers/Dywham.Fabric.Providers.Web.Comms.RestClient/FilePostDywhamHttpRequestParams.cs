using System.IO;

namespace Dywham.Fabric.Providers.Web.Comms.RestClient
{
    public class FilePostDywhamHttpRequestParams : DywhamRestRequestParams
    {
        public string FileName { get; set; }

        public string FilePath { get; set; }

        public byte[] FileContent { get; set; }

        public Stream Stream { get; set; }
    }
}