using UserManagement.API.Models.Logs;
using UserManagement.Models;

namespace UserManagement.API.Utility;

public class LogUtilities
{
    /// <summary>
    /// Create a LogListViewModel from a list of Logs which is fed into the view.
    /// </summary>
    /// <param name="logs"> List of logs models picked up from the service </param>
    /// <returns>The list view model which is added to List view.</returns>
    public static LogListViewModel CreateListViewModel(IEnumerable<Log> logs)
    {
        var items = logs.Select(p => new LogListItemViewModel
        {
            Id = p.Id,
            User = p.User,
            DateTimeOfIssue = p.DateTimeOfIssue.ToString("dd/MM/yyyy HH:mm:ss"),
            Type = p.Type,
        });

        var model = new LogListViewModel
        {
            Items = items.ToList()
        };

        return model;
    }

    private static object ConvertValue(string value, Type targetType)
    {
        // Implement appropriate conversion logic based on the target data type
        if (targetType == typeof(int) || targetType == typeof(long))
        {
            if (long.TryParse(value, out long intValue))
            {
                return Convert.ChangeType(intValue, targetType);
            }
        }
        else if (targetType == typeof(DateTime))
        {
            if (DateTime.TryParse(value, out DateTime dateTimeValue))
            {
                return Convert.ChangeType(dateTimeValue, targetType);
            }
        }
        else if (targetType == typeof(bool))
        {
            if (bool.TryParse(value, out bool boolValue))
            {
                return Convert.ChangeType(boolValue, targetType);
            }
            else if (int.TryParse(value, out int intValue))
            {
                return Convert.ChangeType(intValue != 0, targetType);
            }
        }

        return value; // Default to string if no conversion is possible
    }

    /// <summary>
    /// Builds a log user change used to display what did the current log modify
    /// </summary>
    /// <param name="log">The current log</param>
    /// <returns>LogUserChange which is used in log view</returns>
    public static LogUserChange BuildLogUserChange(Log log)
    {
        User oldUser = new User();
        User newUser = new User();
        foreach (var entry in log.LogEntries)
        {
            var oldPropertyInfo = oldUser.GetType().GetProperty(entry.PropertyName);
            var newPropertyInfo = newUser.GetType().GetProperty(entry.PropertyName);

            if (oldPropertyInfo != null && newPropertyInfo != null)
            {
                if (entry.PropertyName != "Logs")
                {
                    object oldValue = ConvertValue(entry.PropertyOldValue, oldPropertyInfo.PropertyType);
                    object newValue = ConvertValue(entry.PropertyNewValue, newPropertyInfo.PropertyType);

                    oldPropertyInfo.SetValue(oldUser, oldValue);
                    newPropertyInfo.SetValue(newUser, newValue);
                }
            }
        }

        return new LogUserChange(log, oldUser, newUser);
    }
}
