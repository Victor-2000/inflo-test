namespace UserManagement.Models;

public class LogUserChange
{
    public LogUserChange(Log log, User oldUser, User newUser)
    {
        this.log = log;
        this.oldUser = oldUser;
        this.newUser = newUser;
    }

    public User oldUser { get; set; } = default!;
    public User newUser { get; set; } = default!;


    public Log log { get; set; } = default!;
}
