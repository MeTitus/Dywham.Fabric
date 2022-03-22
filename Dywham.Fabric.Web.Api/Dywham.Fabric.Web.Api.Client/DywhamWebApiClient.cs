using System;
using Dywham.Fabric.Providers.Web.Comms.RestClient;

namespace Dywham.Fabric.Web.Api.Client
{
    public class DywhamWebApiClient : IDywhamWebApiClient
    {
        protected Func<Type, object> CreateFilterFunc;

        public IDywhamRestClient DywhamRestClient { get; set; }

        public string BaseUrl { get; set; }


        public void OnFilterResolution(Func<Type, object> createFilterFunc)
        {
            CreateFilterFunc = createFilterFunc;
        }
    }
}
