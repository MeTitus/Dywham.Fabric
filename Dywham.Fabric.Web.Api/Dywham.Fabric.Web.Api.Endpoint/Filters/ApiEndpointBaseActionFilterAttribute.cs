using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dywham.Fabric.Web.Api.Endpoint.Providers.Validation;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace Dywham.Fabric.Web.Api.Endpoint.Filters
{
    internal class ApiEndpointBaseActionFilterAttribute : DywhamActionFilterAttribute
    {
        public ApiEndpointBaseActionFilterAttribute()
        {
            Order = 0;
        }


        public override Task<bool> OnBeforeActionExecutionAsync(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                return Task.FromResult(false);
            }

            var model = context.ActionArguments.Values.FirstOrDefault(v => v is IModelValidator);

            if (model == null)
            {
                return Task.FromResult(true);
            }

            var modelValidatorType = typeof(IModelValidator<>).MakeGenericType(model.GetType());
            var modelValidator = context.HttpContext.RequestServices.GetService(modelValidatorType);

            if (modelValidator == null)
            {
                return Task.FromResult(true);
            }

            var validationResult = (ValidationResult)modelValidator.GetType().InvokeMember("Validate",
                BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public, null, modelValidator,
                new[] { model, context.ActionArguments, context.HttpContext });

            if (validationResult == null || !validationResult.ValidationIssues.Any())
            {
                return Task.FromResult(true);
            }

            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.HttpContext.Response.Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(validationResult)));

            return Task.FromResult(false);
        }
    }
}