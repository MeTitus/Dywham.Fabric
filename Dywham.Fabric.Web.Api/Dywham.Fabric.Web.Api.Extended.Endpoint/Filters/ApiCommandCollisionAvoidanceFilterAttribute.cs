using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dywham.Fabric.Providers.Serialization.Json;
using Dywham.Fabric.Web.Api.Endpoint.Filters;
using Dywham.Fabric.Web.Api.Extended.Contracts.Commands;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dywham.Fabric.Web.Api.Extended.Endpoint.Filters
{
    public class ApiCommandCollisionAvoidanceFilterAttribute : DywhamActionFilterAttribute, IDisposable
    {
        private static readonly ConcurrentDictionary<int, DateTime> CommandDictionary = new();
        private static readonly CancellationTokenSource CancellationTokenSource = new();


        static ApiCommandCollisionAvoidanceFilterAttribute()
        {
            Task.Factory.StartNew(() =>
            {
                while (!CancellationTokenSource.IsCancellationRequested)
                {
                    var dateTime = DateTime.UtcNow;
                    var valuesToDelete = CommandDictionary.Where(x => (dateTime - x.Value).Seconds >= 3).Select(x => x.Key);

                    foreach (var value in valuesToDelete)
                    {
                        CommandDictionary.TryRemove(value, out _);
                    }

                    Thread.Sleep(1000);
                }
            }, CancellationTokenSource.Token);
        }


        public override Task<bool> OnBeforeActionExecutionAsync(ActionExecutingContext context)
        {
            var model = context.ActionArguments.Values.FirstOrDefault(v => v is ExtendedCommandModel);

            if (model == null)
            {
                return Task.FromResult(true);
            }

            if (!context.RouteData.Values.TryGetValue("UserId", out var userId))
            {
                return Task.FromResult(true);
            }

            var data = ((IJsonProvider)context.HttpContext.RequestServices.GetService(typeof(IJsonProvider)))?.Serialize(model) + userId;
            var hash = 17;

            unchecked // Overflow is fine, just wrap
            {
                hash = hash * 23 + data.GetHashCode();
            }

            if (CommandDictionary.TryAdd(hash, DateTime.UtcNow)) return Task.FromResult(true);

            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.HttpContext.Response.Body = new MemoryStream(Encoding.UTF8.GetBytes("Duplicated request identified"));

            return Task.FromResult(false);
        }

        public void Dispose() => CancellationTokenSource.Cancel();
    }
}