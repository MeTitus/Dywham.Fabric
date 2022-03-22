using System.Threading.Tasks;
using Dywham.Fabric.Providers;

namespace Dywham.Fabric.Providers.IssueTracking.AzureDevOps
{
    public interface IAzureDevOpsProvider : IProvider
    {
        Task CreateBugAsync(string url, string projectName, string authenticationToken, string name, string body);

        Task CreateTaskAsync(string url, string projectName, string authenticationToken, string name, string body);
    }
}