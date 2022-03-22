using System;
using Dywham.Fabric.Providers.Web.Comms.RestClient;

namespace Dywham.Fabric.Web.Api.Client.Routes
{
    public abstract class DywhamWebApiClientRoutes
    {
        protected readonly IDywhamRestClient DywhamRestClient;
        protected readonly string BaseUrl;
        protected Func<Type, object> CreateFilterFunc;


        protected DywhamWebApiClientRoutes(IDywhamRestClient dywhamRestClient, string url)
        {
            DywhamRestClient = dywhamRestClient;
            BaseUrl = url;
        }

        protected DywhamWebApiClientRoutes(IDywhamRestClient dywhamRestClient, string url, Func<Type, object> createFilterFunc)
        {
            DywhamRestClient = dywhamRestClient;
            BaseUrl = url;
            CreateFilterFunc = createFilterFunc ?? Activator.CreateInstance;
        }
    }
}