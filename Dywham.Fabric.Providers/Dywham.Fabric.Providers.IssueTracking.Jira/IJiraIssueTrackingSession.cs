using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dywham.Fabric.Providers.IssueTracking.Jira
{
    public interface IJiraIssueTrackingSession
    {
        Task<string> CreateIssueAsync(string project, string description, CancellationToken token = default);

        Task<string> CreateIssueAsync(string project, string description, JiraIssueOptions options, CancellationToken token = default);

        Task<List<JiraIssue>> GetIssuesAsync(string project, int limit, int startIndex, bool asc, CancellationToken token = default);

        Task<Tuple<bool, JiraIssue>> TryGetIssueAsync(string key, CancellationToken token = default);

        Task DeleteIssueAsync(string key, CancellationToken token = default);

        Task ApplyTransitionAsync(string key, string transition, string comments, CancellationToken token = default);

        Task ApplyTransitionAsync(string key, string transition, string resolution, string comments, CancellationToken token = default);

        Task AddCommentAsync(string key, string comment, CancellationToken token = default);

        Task AddAttachmentsAsync(string key, string[] attachments, CancellationToken token = default);

        Task AppendToDescription(string key, string description, CancellationToken token = default);

        Task SetDescription(string key, string description, CancellationToken token = default);
    }
}