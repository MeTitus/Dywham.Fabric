using System;
using System.Linq;
using System.Threading.Tasks;
using Dywham.Fabric.Web.Api.Endpoint.Providers.Security;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Dywham.Fabric.Web.Api.Endpoint.Filters
{
    internal class SecurityProviderActionFilterAttribute : DywhamActionFilterAttribute
    {
        public override async Task<bool> OnBeforeActionExecutionAsync(ActionExecutingContext context)
        {
            if (!(context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor))
            {
                return await base.OnBeforeActionExecutionAsync(context);
            }

            var actionAttributes = controllerActionDescriptor.MethodInfo.GetCustomAttributes(typeof(RequiresSessionAttribute), true);

            if (!actionAttributes.Any())
            {
                return await base.OnBeforeActionExecutionAsync(context);
            }

            var securityProvider = context.HttpContext.RequestServices.GetService<ISecurityProvider>();

            if (securityProvider == null)
            {
                throw new Exception("RequiresSession cannot be verified cause SecurityProvider is missing");
            }

            var permissionResult = await securityProvider.ValidateResourcePermissionAsync(context, context.HttpContext.Request.Path, context.HttpContext.Request);

            if (permissionResult == PermissionExecutionResult.Granted)
            {
                return await base.OnBeforeActionExecutionAsync(context);
            }

            context.HttpContext.Response.StatusCode = permissionResult == PermissionExecutionResult.NoPermission
                ? (int)securityProvider.NoPermission
                : (int)securityProvider.NoActiveSession;

            return false;
        }
    }
}