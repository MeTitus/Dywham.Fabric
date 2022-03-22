namespace Dywham.Fabric.Providers.IssueTracking.Jira
{
    public interface IJiraIssueTrackingProvider : IProvider
    {
        IJiraIssueTrackingSession CreateSession(string server, string user, string password);
    }
}