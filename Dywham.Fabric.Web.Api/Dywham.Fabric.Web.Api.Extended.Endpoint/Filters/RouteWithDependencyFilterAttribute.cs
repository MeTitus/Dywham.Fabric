using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dywham.Fabric.Utils;
using Dywham.Fabric.Web.Api.Endpoint.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dywham.Fabric.Web.Api.Extended.Endpoint.Filters
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RouteWithDependencyFilterAttribute : DywhamActionFilterAttribute
    {
        private readonly Type _type;


        public RouteWithDependencyFilterAttribute(Type type)
        {
            _type = type;
        }


        public override Task<bool> OnBeforeActionExecutionAsync(ActionExecutingContext context)
        {
            var methodInfo = ((Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)context.ActionDescriptor).MethodInfo;
            var attribute = methodInfo.GetCustomAttribute<RouteWithDependencyAttribute>(true);

            if (attribute == null) return base.OnBeforeActionExecutionAsync(context);

            var dataContext = Activator.CreateInstance(_type);

            if (attribute.Dependency != null)
            {
                SetPropertyDependency(dataContext, context, attribute.Dependency);
            }

            if (attribute.Dependency2 != null)
            {
                SetPropertyDependency(dataContext, context, attribute.Dependency2);
            }

            if (attribute.Dependency3 != null)
            {
                SetPropertyDependency(dataContext, context, attribute.Dependency3);
            }

            if (attribute.Dependency4 != null)
            {
                SetPropertyDependency(dataContext, context, attribute.Dependency4);
            }

            context.Controller.SetPropertyValue("DataContext", dataContext);

            return base.OnBeforeActionExecutionAsync(context);
        }

        private static void SetPropertyDependency(object instance, ActionContext context, Type typeToInject)
        {
            var propertiesToSet = instance.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => x.PropertyType == typeToInject);
            var service = context.HttpContext.RequestServices.GetService(typeToInject);

            if (service == null)
            {
                throw new NullReferenceException("Service dependency is null");
            }

            foreach (var property in propertiesToSet)
            {
                property.SetValue(instance, service);
            }
        }
    }
}