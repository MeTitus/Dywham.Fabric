using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Dywham.Fabric.Web.Api.Endpoint.Providers.Validation
{
    public interface IModelValidator
    { }

    public interface IModelValidator<in T> : IModelValidator where T: class, new()
    {
        ValidationResult Validate(T model, IDictionary<string, object> actionArgs, HttpContext httpContext);
    }
}