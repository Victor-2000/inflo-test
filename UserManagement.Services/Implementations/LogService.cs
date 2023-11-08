using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;

namespace UserManagement.Services.Domain.Implementations;

public class LogService : ILogsService
{
    private readonly IDataContext _dataAccess;

    public LogService(IDataContext dataAccess) => _dataAccess = dataAccess;

    public IEnumerable<Log> GetAll() => _dataAccess.GetAll<Log>(log => log.User, log => log.LogEntries).IgnoreQueryFilters();

    public IEnumerable<Log> FindLogsbyUserId(long userId) => _dataAccess.GetAll<Log>(log => log.User, log => log.LogEntries).IgnoreQueryFilters().Where(_dataAccess => _dataAccess.UserId == userId);

    /// <summary>
    /// Populate delete or create logs
    /// </summary>
    /// <param name="user">The user details of the log</param>
    /// <param name="log">All the details about the log to populate</param>
    /// <returns>The new log with the entries</returns>
    private Log PopulateLog(User user, Log log)
    {
        Type userType = user.GetType();
        PropertyInfo[] properties = userType.GetProperties();

        log.LogEntries = new List<LogEntry>();

        foreach (PropertyInfo property in properties)
        {
            object? value = property.GetValue(user);

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            string propertyStringValue = value != null ? value.ToString() : string.Empty;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

#pragma warning disable CS8601 // Possible null reference assignment.
            LogEntry logEntry = new LogEntry()
            {
                PropertyName = property.Name,
                PropertyNewValue = propertyStringValue,
                PropertyOldValue = propertyStringValue,
                Log = log,
            };
#pragma warning restore CS8601 // Possible null reference assignment.

            _dataAccess.Create(logEntry);
        }

        return log;
    }

    /// <summary>
    /// Populate update logs
    /// </summary>
    /// <param name="user">The user details of the log</param>
    /// <param name="log">All the details about the log to populate</param>
    /// <returns>The new log with the entries</returns>
    private Log PopulateLog(Log log, List<LogEntry> entries)
    {

        log.LogEntries = new List<LogEntry>();

        foreach (var entry in entries)
        {
            entry.Log = log;
            _dataAccess.Create(entry);
        }

        return log;
    }

    public void AddLog(User user, LogType type)
    {
        Log log = new Log();
        log.DateTimeOfIssue = DateTime.Now;
        log.User = user;
        log.Type = type;

        _dataAccess.Create(log);

        log = PopulateLog(user, log);

        _dataAccess.Update(log);
    }

    public void AddLog(User user, LogType type, List<LogEntry> entries)
    {
        Log log = new Log();
        log = PopulateLog(log,entries);
        log.DateTimeOfIssue = DateTime.Now;
        log.User = user;
        log.Type = type;

        _dataAccess.DetachEntry(log.User);

        _dataAccess.Update(log);
    }
}
