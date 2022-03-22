var provider = new JiraIssueTrackingProvider();
var session = provider.CreateSession("https://jira.hreconnect.eu/", "software.robot", "1234567890");
var ticketKey = session.CreateIssueAsync("TPFS", "Task", "B Grade - Urgent", "Issue Summary", "description", new []
    {
        "C:\\Users\\ru113022\\Desktop\\Stuff\\1.jpg",
        "C:\\Users\\ru113022\\Desktop\\Stuff\\2.jpg",
        "C:\\Users\\ru113022\\Desktop\\Stuff\\3.jpg"
    }).ConfigureAwait(true).GetAwaiter().GetResult();

var ticket = session.GetIssueAsync(ticketKey, CancellationToken.None).ConfigureAwait(true).GetAwaiter().GetResult();