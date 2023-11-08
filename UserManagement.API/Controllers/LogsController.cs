using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.API.Utility;
using Microsoft.AspNetCore.Mvc;
using UserManagement.API.Models.Logs;

namespace UserManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LogsController : Controller
{

    private readonly ILogsService _logsService;

    public LogsController(ILogsService logsService)
    {
        _logsService = logsService;
    }

    [HttpGet]
    public LogListViewModel List()
    {
        return LogUtilities.CreateListViewModel(_logsService.GetAll());
    }

    [HttpGet("log")]
    public LogUserChange LogView([FromQuery]int LogId)
    {
        IEnumerable<Log> logs = _logsService.GetAll();
        Log log = logs.Where(log => log.Id == LogId).First();

        return LogUtilities.BuildLogUserChange(log);
    }
}
