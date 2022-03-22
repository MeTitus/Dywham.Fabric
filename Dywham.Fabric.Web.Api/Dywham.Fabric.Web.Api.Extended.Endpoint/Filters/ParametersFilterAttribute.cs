using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dywham.Fabric.Web.Api.Endpoint.Filters;
using Dywham.Fabric.Web.Api.Endpoint.Models;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dywham.Fabric.Web.Api.Extended.Endpoint.Filters
{
    public class ParametersFilterAttribute : DywhamActionFilterAttribute
    {
        public ExtendedMessagingWebApiEndpointSettings Settings { get; set; }


        public override Task<bool> OnBeforeActionExecutionAsync(ActionExecutingContext context)
        {
            var filterKv = context.ActionArguments
                .Select(e => (KeyValuePair<string, object>?)e)
                .FirstOrDefault(x => x.Value.Value is CollectionQueryModel);

            if (filterKv == null) return Task.FromResult(true);

            var filter = (CollectionQueryModel)filterKv.Value.Value;

            filter.Limit ??= Settings.DefaultApiCount;
            filter.StartIndex ??= Settings.DefaultApiOffset;

            return Task.FromResult(true);
        }
    }
}
