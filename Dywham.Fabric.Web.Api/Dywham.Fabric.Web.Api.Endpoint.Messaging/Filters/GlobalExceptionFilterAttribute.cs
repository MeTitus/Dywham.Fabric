using System.Threading.Tasks;
using Dywham.Fabric.Web.Api.Endpoint.Filters;
using log4net;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dywham.Fabric.Web.Api.Endpoint.Messaging.Filters
{
    public class GlobalExceptionFilterAttribute : DywhamActionFilterAttribute
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);


        public override Task OnExceptionAsync(ActionExecutedContext context)
        {
            Logger.Error(context.Exception);

            return base.OnExceptionAsync(context);
        }
    }
}