using System.Linq;
using UserManagement.Models;
using UserManagement.Services.Domain.Implementations;
using UserManagement.Services.Domain.Interfaces;

namespace UserManagement.Data.Tests;

public class UserServiceTests
{
    [Fact]
    public void GetAll_WhenContextReturnsEntities_MustReturnSameEntities()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var service = CreateService();
        var users = SetupUsers();

        // Act: Invokes the method under test with the arranged parameters.
        var result = service.GetAll();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().BeSameAs(users);
    }

    private IQueryable<User> SetupUsers(string forename = "Johnny", string surname = "User", string email = "juser@example.com", bool isActive = true)
    {
        var users = new[]
        {
            new User
            {
                Forename = forename,
                Surname = surname,
                Email = email,
                IsActive = isActive
            }
        }.AsQueryable();

        _dataContext
            .Setup(s => s.GetAll<User>())
            .Returns(users);

        return users;
    }

    private Log[] SetupLogs(long userId = 1, LogType logType = LogType.CREATE)
    {
        var logs = new[]
        {
            new Log
            {
                UserId = userId,
                Type = logType,
            }
        };

        _logsService
            .Setup(s => s.GetAll())
            .Returns(logs);

        return logs;
    }

    private readonly Mock<IDataContext> _dataContext = new();
    private readonly Mock<ILogsService> _logsService = new();
    private UserService CreateService() => new(_dataContext.Object, _logsService.Object);
}
