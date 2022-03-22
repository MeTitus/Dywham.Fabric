using System;

namespace Dywham.Fabric.Web.Api.Endpoint.Providers.Security
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RequiresSessionAttribute : Attribute
    { }
}