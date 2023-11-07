using UserManagement.Models;

namespace UserManagement.Web.Models.Logs;

public class LogListViewModel
{
    public List<LogListItemViewModel> Items { get; set; } = new();
}

public class LogListItemViewModel
{
    public long Id { get; set; }
    public User User { get; set; } = default!;
    public LogType? Type { get; set; }
    public string? DateTimeOfIssue { get; set; }

    public List<LogEntry> LogEntries { get; set; } = new() { };
}
