using System;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;

namespace Dywham.Fabric.Providers.IssueTracking.AzureDevOps
{
    public class AzureDevOpsProvider : IAzureDevOpsProvider
    {
        public async Task CreateBugAsync(string url, string projectName, string authenticationToken, string name, string body)
        {
            await CreateAsync(url, projectName, authenticationToken, name, body, "Bug");
        }

        public async Task CreateTaskAsync(string url, string projectName, string authenticationToken, string name, string body)
        {
            await CreateAsync(url, projectName, authenticationToken, name, body, "Task");
        }

        private static async Task CreateAsync(string url, string projectName, string authenticationToken, string name, string body, string type)
        {
            var connection = new VssConnection(new Uri(url), new VssBasicCredential(string.Empty, authenticationToken));
            var witClient = connection.GetClient<WorkItemTrackingHttpClient>();
            var patchDocument = new JsonPatchDocument
            {
                new JsonPatchOperation
                {
                    Operation = Operation.Add, Path = "/fields/System.Title", Value = name
                },
                new JsonPatchOperation
                {
                    Operation = Operation.Add,
                    Path = "/fields/Microsoft.VSTS.TCM.ReproSteps",
                    Value = body
                },
                new JsonPatchOperation
                {
                    Operation = Operation.Add, Path = "/fields/Microsoft.VSTS.Common.Priority", Value = "1"
                },
                new JsonPatchOperation
                {
                    Operation = Operation.Add, Path = "/fields/Microsoft.VSTS.Common.Severity", Value = "2 - High"
                }
            };

            await witClient.CreateWorkItemAsync(patchDocument, projectName, type);
        }
    }
}