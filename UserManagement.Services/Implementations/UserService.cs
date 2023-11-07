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

    private readonly ILogsService _logsService;
    public UserService(IDataContext dataAccess, ILogsService logsService)
    {
        _dataAccess = dataAccess;
        _logsService = logsService;
    }
    /// <summary>
    /// Return users by active state
    /// </summary>
    /// <param name="isActive"></param>
    /// <returns></returns>
    public IEnumerable<User> FilterByActive(bool isActive)
    {
        return _dataAccess.GetAll<User>().Where(_dataAccess => _dataAccess.IsActive == isActive);
    }

    public IEnumerable<User> GetAll() => _dataAccess.GetAll<User>(user => user.Logs);

    public User FindUserById(long userId)
    {
        User? user;
        try
        {
           user = _dataAccess.GetAll<User>(user => user.Logs).Where(_dataAccess => _dataAccess.Id == userId).First();
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
            user.IsDeleted = true;
            _dataAccess.Update(user);
            _logsService.AddLog(user, LogType.DELETE);
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
            List<LogEntry> logEntries = new();
            try
            {
                oldUserData = FindUserById(user.Id);

                logEntries.Add(
                    new LogEntry
                    {
                        PropertyName = "Surname",
                        PropertyOldValue = oldUserData.Surname,
                        PropertyNewValue = user.Surname,
                    }
                );
                oldUserData.Surname = user.Surname;

                logEntries.Add(
                    new LogEntry
                    {
                        PropertyName = "Forename",
                        PropertyOldValue = oldUserData.Forename,
                        PropertyNewValue = user.Forename,
                    }
                );
                oldUserData.Forename = user.Forename;

                logEntries.Add(
                    new LogEntry
                    {
                        PropertyName = "DateOfBirth",
                        PropertyOldValue = oldUserData.DateOfBirth.ToString("dd/MM/yyyy"),
                        PropertyNewValue = user.DateOfBirth.ToString("dd/MM/yyyy"),
                    }
                );
                oldUserData.DateOfBirth = user.DateOfBirth;

                logEntries.Add(
                    new LogEntry
                    {
                        PropertyName = "Email",
                        PropertyOldValue = oldUserData.Email,
                        PropertyNewValue = user.Email,
                    }
                );
                oldUserData.Email = user.Email;


                string oldIsActiveString = oldUserData.IsActive ? "true" : "false";
                string newIsActiveString = user.IsActive ? "true" : "false";


                logEntries.Add(
                    new LogEntry
                    {
                        PropertyName = "IsActive",
                        PropertyOldValue = oldIsActiveString,
                        PropertyNewValue = newIsActiveString,
                    }
                );
                oldUserData.IsActive = user.IsActive;

                _dataAccess.Update(oldUserData);
                _logsService.AddLog(oldUserData, LogType.UPDATE, logEntries);
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
          _logsService.AddLog(user, LogType.CREATE);
        }
        else
        {
            throw new InvalidCastException("Invalid input");
        }
    }
}
