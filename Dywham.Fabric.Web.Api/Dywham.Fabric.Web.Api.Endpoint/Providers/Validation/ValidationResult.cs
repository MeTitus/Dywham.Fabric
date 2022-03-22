using System.Collections.Generic;

namespace Dywham.Fabric.Web.Api.Endpoint.Providers.Validation
{
    public class ValidationResult
    {
        public Dictionary<string, object> ValidationIssues { get; set; } = new Dictionary<string, object>();

        public string Message { get; set; }
    }
}