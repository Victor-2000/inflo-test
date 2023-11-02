using System;
using System.Linq;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models.Users;

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
    private UserListViewModel? CreateListViewModel (IEnumerable<Models.User> users)
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

    [HttpDelete]
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
    public ActionResult EditView([FromQuery] int id)
    {
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

    [HttpPost]
    public StatusCodeResult Edit(int id)
    {
        try
        {
            _userService.EditUser(id);
        }
        catch (InvalidOperationException ex)
        {
            //User not found
            if (ex != null)
            {
                return NotFound();
            }
        }

        return Ok();
    }

    [HttpGet("view")]
    public ActionResult ShowView([FromQuery] int id)
    {
        try
        {
            return View(_userService.FindUserById(id));
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

    [HttpPost]
    public StatusCodeResult Show(int id)
    {
        try
        {
            _userService.ViewUser(id);
        }
        catch (InvalidOperationException ex)
        {
            //User not found
            if (ex != null)
            {
                return NotFound();
            }
        }

        return Ok();
    }

    [HttpGet("view")]
    public ActionResult AddUserView([FromQuery] int id)
    {
      return View();
    }

    [HttpPost]
    public StatusCodeResult AddUser(int id)
    {
        try
        {
            _userService.AddUser(id);
        }
        catch (InvalidOperationException ex)
        {
            //User not found
            if (ex != null)
            {
                return NotFound();
            }
        }

        return Ok();
    }
}
