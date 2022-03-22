namespace Dywham.Fabric.Providers.IssueTracking.Jira
{
    public class JiraIssueTrackingProvider : IJiraIssueTrackingProvider
    {
        public IJiraIssueTrackingSession CreateSession(string server, string user, string password)
        {
            return new JiraIssueTrackingSession(Atlassian.Jira.Jira.CreateRestClient(server, user, password));
        }
    }
}