using System.Reflection;
using log4net;
using Microsoft.AspNetCore.Mvc;

namespace Dywham.Fabric.Web.Api.Endpoint.Routes
{
    [ApiController]
    public class ApiRoutes : ControllerBase
    {
        // ReSharper disable once UnusedMember.Global
        protected readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()!.DeclaringType);
    }
}