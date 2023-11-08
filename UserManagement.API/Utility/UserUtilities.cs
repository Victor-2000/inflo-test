using UserManagement.API.Models.Logs;
using UserManagement.API.Models.Users;
using UserManagement.Models;

namespace UserManagement.API.Utility;

public class UserUtilities
{
    /// <summary>
    /// Create a UserListViewModel from a list of Users which is fed into the view.
    /// </summary>
    /// <param name="users"> List of user models picked up from the service </param>
    /// <returns>The list view model which is added to List view.</returns>
    static public UserListViewModel CreateUserListViewModel(IEnumerable<User> users)
    {
        var items = users.Select(p => new UserListItemViewModel
        {
            Id = p.Id,
            Forename = p.Forename,
            Surname = p.Surname,
            DateOfBirth = p.DateOfBirth.ToString("dd/MM/yyyy"),
            Email = p.Email,
            IsActive = p.IsActive
        });

        var model = new UserListViewModel
        {
            Items = items.ToList()
        };

        return model;
    }

    /// <summary>
    /// Create a LogListViewModel from a list of Logs which is fed into the view.
    /// </summary>
    /// <param name="logs"> List of logs models picked up from the service </param>
    /// <returns>The list view model which is added to List view.</returns>
    static public LogListViewModel CreateLogListViewModel(IEnumerable<Log> logs)
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

}
