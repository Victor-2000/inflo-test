using System.Linq;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models.Users;

namespace UserManagement.WebMS.Controllers;

[Route("users")]
public class UsersController : Controller
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService) => _userService = userService;

    private UserListViewModel? CreateListViewModel (IEnumerable<Models.User> users)
    {
        var items = users.Select(p => new UserListItemViewModel
        {
            Id = p.Id,
            Forename = p.Forename,
            Surname = p.Surname,
            Email = p.Email,
            IsActive = p.IsActive
        });

        var model = new UserListViewModel
        {
            Items = items.ToList()
        };

        return model;
    }

    [HttpGet]
    public ViewResult List([FromQuery] bool? isActive)
    {
        UserListViewModel? model;
        if (isActive != null)
        {
            model = CreateListViewModel(_userService.FilterByActive((bool)isActive));
        }
        else
        {
            model = CreateListViewModel(_userService.GetAll());
        }

        return View(model);
    }
}
