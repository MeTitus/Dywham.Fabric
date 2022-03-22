namespace Dywham.Fabric.Providers.IssueTracking.Jira
{
    public class JiraIssueOptions
    {
        public string Type { get; set; } = "Task";

        public string Priority { get; set; }

        public string Description { get; set; }

        public string[] Attachments { get; set; }
    }
}
