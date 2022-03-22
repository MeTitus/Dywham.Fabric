using System.Collections.Generic;

namespace Dywham.Fabric.Providers.IssueTracking.Jira
{
    public class JiraIssue
    {
        public string Key { get; set; }

        public string Project { get; set; }

        public string Type { get; set; }

        public string Priority { get; set; }

        public string Description { get; set; }

        public string Summary { get; set; }

        public string Status { get; set; }

        public List<JiraIssueStatus> Statuses { get; set; }

        public List<JiraIssueResolution> Resolutions { get; set; }
    }
}