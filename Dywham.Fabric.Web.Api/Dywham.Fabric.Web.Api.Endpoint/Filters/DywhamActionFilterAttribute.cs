using System.Threading.Tasks;
using Dywham.Fabric.Web.Api.Endpoint.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dywham.Fabric.Web.Api.Endpoint.Filters
{
    public class DywhamActionFilterAttribute : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (await OnBeforeActionExecutionAsync(context))
            {
                var executed = await next();

                if (executed.Exception != null && !executed.ExceptionHandled)
                {
                    await OnExceptionAsync(executed);
                }
                else
                {
                    // NOTE: We might want to use executed here too.
                    await OnAfterActionExecutionAsync(executed);
                }
            }
        }

        public virtual Task<bool> OnBeforeActionExecutionAsync(ActionExecutingContext context)
        {
            return Task.FromResult(true);
        }

        public virtual Task OnAfterActionExecutionAsync(ActionExecutedContext context)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnExceptionAsync(ActionExecutedContext context)
        {
            if (context.Exception is not DywhamHttpException) return Task.CompletedTask;

            switch (context.Exception)
            {
                case BadRequestHttpException _:

                    context.Result = new BadRequestResult();
                    context.ExceptionHandled = true;

                    break;

                case ForbiddenHttpException _:

                    context.Result = new ForbidResult();
                    context.ExceptionHandled = true;

                    break;

                case UnauthorizedHttpException _:

                    context.Result = new UnauthorizedResult();
                    context.ExceptionHandled = true;

                    break;

                case NotFoundHttpException _:

                    context.Result = new NotFoundResult();
                    context.ExceptionHandled = true;

                    break;
            }

            return Task.CompletedTask;
        }
    }
}