using System.Collections.Generic;
using UserManagement.Models;

namespace UserManagement.Services.Domain.Interfaces;

public interface ILogsService
{
    public IEnumerable<Log> GetAll();

    public IEnumerable<Log> FindLogsbyUserId(long userId);
    public void AddLog(User user, LogType type);

    public void AddLog(User user, LogType type, List<LogEntry> entries);
}
