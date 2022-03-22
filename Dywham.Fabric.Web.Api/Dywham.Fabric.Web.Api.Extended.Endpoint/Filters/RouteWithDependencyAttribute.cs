using System;
using Microsoft.AspNetCore.Mvc;

namespace Dywham.Fabric.Web.Api.Extended.Endpoint.Filters
{
    public class RouteWithDependencyAttribute : RouteAttribute
    {
        public RouteWithDependencyAttribute(string template) : base(template)
        { }

        public RouteWithDependencyAttribute(string template, Type dependency) : base(template)
        {
            Dependency = dependency;
        }

        public RouteWithDependencyAttribute(string template, Type dependency, Type dependency2) : base(template)
        {
            Dependency = dependency;
            Dependency2 = dependency2;
        }

        public RouteWithDependencyAttribute(string template, Type dependency, Type dependency2, Type dependency3) :
            base(template)
        {
            Dependency = dependency;
            Dependency2 = dependency2;
            Dependency3 = dependency3;
        }

        public RouteWithDependencyAttribute(string template, Type dependency, Type dependency2, Type dependency3,
            Type dependency4) : base(template)
        {
            Dependency = dependency;
            Dependency2 = dependency2;
            Dependency3 = dependency3;
            Dependency4 = dependency4;
        }


        public Type Dependency { get; }

        public Type Dependency2 { get; }

        public Type Dependency3 { get; }

        public Type Dependency4 { get; }
    }
}