using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;

namespace Dywham.Fabric.Providers.Web.Comms.RestClient
{
    public class DywhamRestRequestParams
    {
        public string Url { get; set; }

        public object QuerystringParams { get; set; }
        
        public Dictionary<string, object> RequestHeaders { get; set; }

        public Action<HttpRequestMessage> OnBeforeRequest { get; set; }

        public Action<HttpResponseMessage, string> OnAfterRequest { get; set; }

        public JsonSerializerSettings JsonSerializerSettingsIncoming { get; set; } = new JsonSerializerSettings
        {
            Formatting = Formatting.None
        };
    }
}