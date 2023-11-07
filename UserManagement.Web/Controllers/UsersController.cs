using System;
using System.Linq;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models.Users;
using UserManagement.Models;
using UserManagement.Web.Models.Logs;

namespace UserManagement.WebMS.Controllers;

[Route("users")]
public class UsersController : Controller
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService) => _userService = userService;

    /// <summary>
    /// Create a UserListViewModel from a list of Users which is fed into the view.
    /// </summary>
    /// <param name="users"> List of user models picked up from the service </param>
    /// <returns>The list view model which is added to List view.</returns>
    private UserListViewModel? CreateUserListViewModel (IEnumerable<User> users)
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
    public LogListViewModel? CreateLogListViewModel(IEnumerable<Log> logs)
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

    [HttpGet]
    public ViewResult List([FromQuery] bool? isActive)
    {
        UserListViewModel? model;
        if (isActive != null)
        {
            model = CreateUserListViewModel(_userService.FilterByActive((bool)isActive));
        }
        else
        {
            model = CreateUserListViewModel(_userService.GetAll());
        }

        return View(model);
    }

    [HttpGet("delete")]
    public ActionResult DeleteView([FromQuery] int id)
    {
        try
        {
            return View(_userService.FindUserById(id));
        }catch (InvalidOperationException ex)
        {
            //User not found so return to List
            if (ex != null)
            {
                return RedirectToAction("List");
            }
        }

        return RedirectToAction("List");
    }

    [HttpDelete("delete")]
    public StatusCodeResult Delete(int id)
    {
        try
        {
            _userService.DeleteUser(id);
        }catch(InvalidOperationException ex)
        {
            //User not found
            if(ex != null)
            {
                return NotFound();
            }
        }

        return Ok();
    }

    [HttpGet("edit")]
    public ActionResult EditView([FromQuery] int id, bool? inputValid)
    {
        if (inputValid != null)
        {
            ViewBag.Message = "Invalid user details!";
        }
        else
        {
            ViewBag.Message = null;
        }
        try
        {
            return View(_userService.FindUserById(id));
        }
        catch (InvalidOperationException ex)
        {
            //User not found so return to List
            if (ex != null)
            {
              return  RedirectToAction("List");
            }
        }

        return RedirectToAction("List");
    }

    [HttpPost("edit")]
    public ActionResult Edit([FromForm]User user)
    {
        try
        {
            _userService.EditUser(user);
        }
        catch (Exception ex)
        {
            //User not valid or not found
            if (ex != null)
            {
                return RedirectToAction("EditView", new { id = user.Id, inputValid = false });
            }
        }

        return RedirectToAction("List");
    }

    [HttpGet("view")]
    public ActionResult UserView([FromQuery] int id)
    {
        try
        {
            User user = _userService.FindUserById(id);
            return View(user);
        }
        catch (InvalidOperationException ex)
        {
            //User not found so return to List
            if (ex != null)
            {
                return RedirectToAction("List");
            }
        }

        return RedirectToAction("List");
    }

    [HttpGet("add")]
    public ActionResult AddView(bool? inputValid)
    {
        if(inputValid != null)
        {
            ViewBag.Message = "Invalid user details!";
        }
        else
        {
            ViewBag.Message = null;
        }
        return View();
    }

    [HttpPost("add")]
    public ActionResult AddUser([FromForm]User user)
    {
        try
        {
            _userService.AddUser(user);
        }
        catch (InvalidCastException ex)
        {
            //User not valid
            if (ex != null)
            {
                return RedirectToAction("AddView", new {inputValid = false});
            }
        }

        return RedirectToAction("List");
    }
}
