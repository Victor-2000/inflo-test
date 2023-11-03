using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UserManagement.Data;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;

namespace UserManagement.Services.Domain.Implementations;

public class UserService : IUserService
{
    private readonly IDataContext _dataAccess;
    public UserService(IDataContext dataAccess) => _dataAccess = dataAccess;

    /// <summary>
    /// Return users by active state
    /// </summary>
    /// <param name="isActive"></param>
    /// <returns></returns>
    public IEnumerable<User> FilterByActive(bool isActive)
    {
        return _dataAccess.GetAll<User>().Where(_dataAccess => _dataAccess.IsActive == isActive);
    }

    public IEnumerable<User> GetAll() => _dataAccess.GetAll<User>();

    public User FindUserById(long userId)
    {
        User? user;
        try
        {
           user = _dataAccess.GetAll<User>().Where(_dataAccess => _dataAccess.Id == userId).First();
        } catch(InvalidOperationException ex)
        {
            throw ex;
        }
        return user;
    }

    private bool ValidateEmail(string email)
    {
        return Regex.IsMatch(email, @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}$");
    }

    private bool ValidateInput(User user)
    {
        if (string.IsNullOrWhiteSpace(user.Surname) || string.IsNullOrWhiteSpace(user.Forename) || string.IsNullOrWhiteSpace(user.Email))
        {
            return false;
        }

        if (!ValidateEmail(user.Email))
        {
            return false;
        }

        return true;
    }

    public void DeleteUser(int userId)
    {
        User user;
        try
        {
            user = FindUserById(userId);
            _dataAccess.Delete(user);
        }
        catch(InvalidOperationException ex)
        {
            throw ex;
        }
    }

    public void EditUser(User user)
    {
        if (ValidateInput(user))
        {
            User oldUserData;
            try
            {
                oldUserData = FindUserById(user.Id);

                oldUserData.Surname = user.Surname;
                oldUserData.Forename = user.Forename;
                oldUserData.DateOfBirth = user.DateOfBirth;
                oldUserData.Email = user.Email;
                oldUserData.IsActive = user.IsActive;

                _dataAccess.Update(oldUserData);
            }
            catch (InvalidOperationException ex)
            {
                throw ex;
            }
        }
        else
        {
            throw new InvalidCastException("Invalid input");
        }
    }

    public void AddUser(User user)
    {
        if (ValidateInput(user))
        {
          _dataAccess.Create(user);
        }
        else
        {
            throw new InvalidCastException("Invalid input");
        }
    }
}
