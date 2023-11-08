using UserManagement.Services.Domain.Interfaces;
using UserManagement.Models;
using UserManagement.API.Utility;
using Microsoft.AspNetCore.Mvc;
using UserManagement.API.Models.Users;

namespace UserManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : Controller
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService) => _userService = userService;

    [HttpGet]
    public UserListViewModel List([FromQuery] bool? isActive)
    {
        UserListViewModel model;
        if (isActive != null)
        {
            model = UserUtilities.CreateUserListViewModel(_userService.FilterByActive((bool)isActive));
        }
        else
        {
            model = UserUtilities.CreateUserListViewModel(_userService.GetAll());
        }

        return model;
    }

    [HttpGet("user")]
    public User GetUser([FromQuery] int id)
    {
        try
        {
            return _userService.FindUserById(id);
        }
        catch (InvalidOperationException ex)
        {
            //User not found so return to List
            if (ex != null)
            {
                return new User();
            }
        }

        return new User();
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

    [HttpPost("edit")]
    public StatusCodeResult Edit([FromForm]User user)
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
                return NotFound();
            }
        }

        return Ok();
    }

    [HttpPost("add")]
    public StatusCodeResult AddUser([FromForm]User user)
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
                return NotFound();
            }
        }

        return Ok();
    }
}
