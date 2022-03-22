using System;
using Dywham.Fabric.Providers.Web.Comms.RestClient;

namespace Dywham.Fabric.Web.Api.Client
{
    public interface IDywhamWebApiClient
    {
        IDywhamRestClient DywhamRestClient { get; set; }

        string BaseUrl { get; set; }
        

        void OnFilterResolution(Func<Type, object> createFilterFunc);
    }
}