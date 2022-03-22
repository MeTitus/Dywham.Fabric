using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Atlassian.Jira;

namespace Dywham.Fabric.Providers.IssueTracking.Jira
{
    public class JiraIssueTrackingSession : IJiraIssueTrackingSession
    {
        private readonly Atlassian.Jira.Jira _jiraServer;


        internal JiraIssueTrackingSession(Atlassian.Jira.Jira jiraServer)
        {
            _jiraServer = jiraServer;
        }


        public async Task<string> CreateIssueAsync(string project, string description, CancellationToken token = default)
        {
            return await CreateIssueAsync(project, description, null, token);
        }

        public async Task<string> CreateIssueAsync(string project, string summary, JiraIssueOptions options, CancellationToken token = default)
        {
            var issue = _jiraServer.CreateIssue(project);

            options ??= new JiraIssueOptions();

            issue.Summary = summary;
            issue.Type = options.Type;
            issue.Priority = options.Priority;
            issue.Description = options.Description;

            await issue.SaveChangesAsync(token);

            Thread.Sleep(1500);

            if (options.Attachments == null || !options.Attachments.Any()) return issue.Key.Value;

            foreach (var attachment in options.Attachments)
            {
                var uploadAttachmentInfo = new UploadAttachmentInfo(Path.GetFileName(attachment), await File.ReadAllBytesAsync(attachment, token));

                await issue.AddAttachmentAsync(new[] { uploadAttachmentInfo }, token);
            }

            return issue.Key.Value;
        }

        public async Task<List<JiraIssue>> GetIssuesAsync(string project, int limit, int startIndex, bool asc, CancellationToken token = default)
        {
            var issueList = new List<JiraIssue>();
            var issues = await _jiraServer.Issues.GetIssuesFromJqlAsync($"project = {project} ORDER BY created { (asc ? "ASC" : "DESC") }", limit, startIndex, CancellationToken.None);

            foreach (var issue in issues)
            {
                issueList.Add(new JiraIssue
                {
                    Key = issue.Key.Value,
                    Summary = issue.Summary,
                    Priority = issue.Priority.Name,
                    Project = issue.Project,
                    Type = issue.Type.Name,
                    Description = issue.Description,
                    Status = issue.Status.Name,
                    Resolutions = (await _jiraServer.Resolutions.GetResolutionsAsync(token))
                        .Select(x => new JiraIssueResolution
                        {
                            Name = x.Name,
                            Id = x.Id
                        }).ToList(),
                    Statuses = (await _jiraServer.Statuses.GetStatusesAsync(token))
                        .Select(x => new JiraIssueStatus
                        {
                            Name = x.Name,
                            Description = x.Description,
                            Id = x.Id
                        }).ToList()
                });
            }

            return issueList;
        }

        public async Task<Tuple<bool, JiraIssue>> TryGetIssueAsync(string key, CancellationToken token = default)
        {
            try
            {
                var jiraIssue = await _jiraServer.Issues.GetIssueAsync(key, token);

                return new Tuple<bool, JiraIssue>(true, new JiraIssue
                {
                    Summary = jiraIssue.Summary,
                    Priority = jiraIssue.Priority.Name,
                    Project = jiraIssue.Project,
                    Type = jiraIssue.Type.Name,
                    Description = jiraIssue.Description,
                    Status = jiraIssue.Status.Name,
                    Resolutions = (await _jiraServer.Resolutions.GetResolutionsAsync(token))
                        .Select(x => new JiraIssueResolution
                        {
                            Name = x.Name,
                            Id = x.Id
                        }).ToList(),
                    Statuses = (await _jiraServer.Statuses.GetStatusesAsync(token))
                        .Select(x => new JiraIssueStatus
                        {
                            Name = x.Name,
                            Description = x.Description,
                            Id = x.Id
                        }).ToList()
                });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Issue Does Not Exist", StringComparison.OrdinalIgnoreCase))
                {
                    return new Tuple<bool, JiraIssue>(false, null);
                }

                throw;
            }
        }

        public Task DeleteIssueAsync(string key, CancellationToken token = default)
        {
            return _jiraServer.Issues.DeleteIssueAsync(key, token);
        }

        public async Task ApplyTransitionAsync(string key, string transition, string comments, CancellationToken token = default)
        {
            await ApplyTransitionAsync(key, transition, string.Empty, comments, token);
        }

        public async Task ApplyTransitionAsync(string key, string transition, string resolution, string comments, CancellationToken token = default)
        {
            var issue = await _jiraServer.Issues.GetIssueAsync(key, token);

            if (!string.IsNullOrEmpty(comments))
            {
                await issue.AddCommentAsync(comments, token);
            }

            if (!string.IsNullOrEmpty(resolution))
            {
                issue.Resolution = resolution;
            }

            await issue.WorkflowTransitionAsync(transition, token: token);
        }

        public async Task AddCommentAsync(string key, string comment, CancellationToken token = default)
        {
            var issue = await _jiraServer.Issues.GetIssueAsync(key, token);

            await issue.AddCommentAsync(comment, token);
        }

        public async Task AddAttachmentsAsync(string key, string[] attachments, CancellationToken token = default)
        {
            var issue = await _jiraServer.Issues.GetIssueAsync(key, token);

            foreach (var attachment in attachments)
            {
                var uploadAttachmentInfo = new UploadAttachmentInfo(Path.GetFileName(attachment), await File.ReadAllBytesAsync(attachment, token));

                await issue.AddAttachmentAsync(new[] { uploadAttachmentInfo }, token);
            }
        }

        public async Task AppendToDescription(string key, string description, CancellationToken token = default)
        {
            var issue = await _jiraServer.Issues.GetIssueAsync(key, token);
            var text = new StringBuilder();

            text.Append(issue.Description);
            text.Append("\r\n\r\n");
            text.Append(description);

            issue.Description = text.ToString();

            await issue.SaveChangesAsync(token);
        }

        public async Task SetDescription(string key, string description, CancellationToken token = default)
        {
            var issue = await _jiraServer.Issues.GetIssueAsync(key, token);

            issue.Description = description;

            await issue.SaveChangesAsync(token);
        }
    }
}