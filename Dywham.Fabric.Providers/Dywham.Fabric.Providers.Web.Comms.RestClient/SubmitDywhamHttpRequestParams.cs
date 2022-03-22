using Newtonsoft.Json;

namespace Dywham.Fabric.Providers.Web.Comms.RestClient
{
    public class SubmitDywhamHttpRequestParams : DywhamRestRequestParams
    {
        public object Data { get; set; }

        public JsonSerializerSettings JsonSerializerSettingsOutgoing { get; set; } = new JsonSerializerSettings
        {
            Formatting = Formatting.None
        };
    }
}