using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Dywham.Fabric.Data.Repositories;
using Dywham.Fabric.Web.Api.Endpoint.Exceptions;
using Dywham.Fabric.Web.Api.Endpoint.Messaging.Filters;
using Dywham.Fabric.Web.Api.Endpoint.Messaging.Routes;
using Dywham.Fabric.Web.Api.Endpoint.Models;
using Dywham.Fabric.Web.Api.Extended.Endpoint.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Dywham.Fabric.Web.Api.Extended.Endpoint.Routes
{
    [ServiceFilter(typeof(ParametersFilterAttribute)), GlobalExceptionFilter]
    public class ExtendedApiRoutes : MessagingApiRoutes
    {
        public IMapper Mapper { get; set; }


        protected async Task<T> BuildExecutionResultAsync<T, TZ>(Func<Task<TZ>> func, Action<T, TZ> mutator = null)
        {
            var entity = await func();

            if (entity == null)
            {
                throw new NotFoundHttpException();
            }

            var model = Mapper.Map<T>(entity);

            mutator?.Invoke(model, entity);

            return model;
        }

        protected async Task<CollectionQueryResultModel<T>> BuildExecutionResultAsync<T, TZ>(Func<Task<ExecutionResult<TZ>>> func, Action<T, TZ> mutator = null)
        {
            var result = await func();

            return new CollectionQueryResultModel<T>
            {
                Set = result.Data.Select(x =>
                {
                    var model = Mapper.Map<T>(x);

                    mutator?.Invoke(model, x);

                    return model;
                }).ToList(),
                SetSize = result.SetSize,
                TotalCount = result.TotalCount
            };
        }

        protected async Task<bool> BuildEmptyResultAsync(Func<Task<bool>> func)
        {
            var result = await func();

            Response.Headers.Add("x-resourcecollectionempty", result ? "false" : "true");

            return result;
        }
    }
}