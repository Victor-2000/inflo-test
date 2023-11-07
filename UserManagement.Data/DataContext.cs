using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using UserManagement.Models;

namespace UserManagement.Data;

public class DataContext : DbContext, IDataContext
{
    public DataContext() => Database.EnsureCreated();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseInMemoryDatabase("UserManagement.Data.DataContext");

    public void DetachEntry(object entry)
    {
        this.Entry(entry).State = EntityState.Detached;
    }

    private DateTime GenerateRandomDate()
    {
        Random rnd = new Random();
        DateTime start = new DateTime(1970, 1, 1);
        int range = (DateTime.Today - start).Days;

        return start.AddDays(rnd.Next(range));
    }

    public DateTime GenerateRandomDateTime()
    {
        Random random = new Random();

        int randomHours = random.Next(0, 24);
        int randomMinutes = random.Next(0, 60);
        int randomSeconds = random.Next(0, 60);

        DateTime randomDate = GenerateRandomDate()
            + TimeSpan.FromHours(randomHours)
            + TimeSpan.FromMinutes(randomMinutes)
            + TimeSpan.FromSeconds(randomSeconds);

        return randomDate;
    }

    /// <summary>
    /// Populates the log with LogEntries (mostly used for creation and deletion) 
    /// </summary>
    /// <param name="user">The user that the log is targeting</param>
    /// <returns></returns>
    public List<LogEntry> GenerateLogEntryList(User user, Log log)
    {
        Type userType = user.GetType();
        PropertyInfo[] properties = userType.GetProperties();

        List<LogEntry> logEntries = new List<LogEntry>();

        foreach (PropertyInfo property in properties)
        {
            object? value = property.GetValue(user);

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            string propertyStringValue = value != null ? value.ToString() : string.Empty;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

#pragma warning disable CS8601 // Possible null reference assignment.
            LogEntry logEntry = new LogEntry()
            {
                PropertyName = property.Name,
                PropertyNewValue = propertyStringValue,
                PropertyOldValue = propertyStringValue,
                LogId = log.Id,
            };
#pragma warning restore CS8601 // Possible null reference assignment.

            logEntries.Add(logEntry);
        }

        return logEntries;
    }

    protected override void OnModelCreating(ModelBuilder model)
    {
        User[] users = new[]
          {
            new User { Id = 1, Forename = "Peter", Surname = "Loew", DateOfBirth = GenerateRandomDate(), Email = "ploew@example.com", IsActive = true },
            new User { Id = 2, Forename = "Benjamin Franklin", Surname = "Gates", DateOfBirth = GenerateRandomDate(), Email = "bfgates@example.com", IsActive = true },
            new User { Id = 3, Forename = "Castor", Surname = "Troy", DateOfBirth = GenerateRandomDate(), Email = "ctroy@example.com", IsActive = false },
            new User { Id = 4, Forename = "Memphis", Surname = "Raines", DateOfBirth = GenerateRandomDate(), Email = "mraines@example.com", IsActive = true },
            new User { Id = 5, Forename = "Stanley", Surname = "Goodspeed", DateOfBirth = GenerateRandomDate(), Email = "sgodspeed@example.com", IsActive = true },
            new User { Id = 6, Forename = "H.I.", Surname = "McDunnough", DateOfBirth = GenerateRandomDate(), Email = "himcdunnough@example.com", IsActive = true },
            new User { Id = 7, Forename = "Cameron", Surname = "Poe", DateOfBirth = GenerateRandomDate(), Email = "cpoe@example.com", IsActive = false },
            new User { Id = 8, Forename = "Edward", Surname = "Malus", DateOfBirth = GenerateRandomDate(), Email = "emalus@example.com", IsActive = false },
            new User { Id = 9, Forename = "Damon", Surname = "Macready", DateOfBirth = GenerateRandomDate(), Email = "dmacready@example.com", IsActive = false },
            new User { Id = 10, Forename = "Johnny", Surname = "Blaze", DateOfBirth = GenerateRandomDate(), Email = "jblaze@example.com", IsActive = true },
            new User { Id = 11, Forename = "Robin", Surname = "Feld", DateOfBirth = GenerateRandomDate(), Email = "rfeld@example.com", IsActive = true },
        };


        Log[] logs = new[]
          {
            new Log { Id = 1, DateTimeOfIssue = GenerateRandomDateTime(), UserId = 1, Type=LogType.CREATE},
            new Log { Id = 2, DateTimeOfIssue = GenerateRandomDateTime(), UserId = 2, Type=LogType.CREATE},
            new Log { Id = 3, DateTimeOfIssue = GenerateRandomDateTime(), UserId = 3, Type=LogType.CREATE},
            new Log { Id = 4, DateTimeOfIssue = GenerateRandomDateTime(), UserId = 4, Type=LogType.CREATE},
            new Log { Id = 5, DateTimeOfIssue = GenerateRandomDateTime(), UserId = 5, Type=LogType.CREATE},
            new Log { Id = 6, DateTimeOfIssue = GenerateRandomDateTime(), UserId = 6, Type=LogType.CREATE},
            new Log { Id = 7, DateTimeOfIssue = GenerateRandomDateTime(), UserId = 7, Type=LogType.CREATE},
            new Log { Id = 8, DateTimeOfIssue = GenerateRandomDateTime(), UserId = 8, Type=LogType.CREATE},
            new Log { Id = 9, DateTimeOfIssue = GenerateRandomDateTime(), UserId = 9, Type=LogType.CREATE},
            new Log { Id = 10, DateTimeOfIssue = GenerateRandomDateTime(), UserId = 10, Type=LogType.CREATE},
            new Log { Id = 11, DateTimeOfIssue = GenerateRandomDateTime(), UserId = 11, Type=LogType.CREATE},
        };

        List<LogEntry> AllLogEntries = new();
        long logEntryId = 1;
        for (int i=0; i<logs.Length; i++)
        {
            var logEntryList = GenerateLogEntryList(users[i], logs[i]);
            foreach (var logEntry in logEntryList)
            {
                logEntryId++;
                logEntry.Id = logEntryId;
                AllLogEntries.Add(logEntry);
            }
        }
        LogEntry[] logEntriesArray = AllLogEntries.ToArray();

       model.Entity<User>().HasData(users);

       //Filter out the soft deleted elements
       model.Entity<User>().HasQueryFilter(u => !u.IsDeleted);

       model.Entity<Log>().HasData(logs);
       model.Entity<LogEntry>().HasData(logEntriesArray);


        //Set up relations between data
        model.Entity<Log>()
       .HasMany(l => l.LogEntries)
       .WithOne(le => le.Log)
       .HasForeignKey(le => le.LogId);

       model.Entity<User>()
       .HasMany(u => u.Logs)
       .WithOne(l => l.User)     
       .HasForeignKey(l => l.UserId);

    }

    public DbSet<User>? Users { get; set; }
    public DbSet<Log>? Logs { get; set; }
    public DbSet<LogEntry>? LogEntries { get; set; }

    public IQueryable<TEntity> GetAll<TEntity>(params Expression<Func<TEntity, object>>[] includes) where TEntity : class
    {
        var query = base.Set<TEntity>().AsQueryable();

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return query;
    }

    public void Create<TEntity>(TEntity entity) where TEntity : class
    {
        base.Add(entity);
        SaveChanges();
    }

    public new void Update<TEntity>(TEntity entity) where TEntity : class
    {
        base.Update(entity);
        SaveChanges();
    }

    public void Delete<TEntity>(TEntity entity) where TEntity : class
    {
        base.Remove(entity);
        SaveChanges();
    }
}
