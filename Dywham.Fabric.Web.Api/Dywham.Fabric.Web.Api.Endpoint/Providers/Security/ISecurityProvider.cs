using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dywham.Fabric.Web.Api.Endpoint.Providers.Security
{
    public interface ISecurityProvider
    {
        HttpStatusCode NoActiveSession { get; set; }

        HttpStatusCode NoPermission { get; set; }


        Task<PermissionExecutionResult> ValidateResourcePermissionAsync(ActionExecutingContext actionContext, string resource,
            HttpRequest httpRequest, CancellationToken token = default);
    }
}