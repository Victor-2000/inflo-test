using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Services.Domain.Interfaces;

namespace UserManagement.Web.Controllers;
public class LogsController : Controller
{

    private readonly ILogsService _logsService;
    public LogsController(ILogsService logsService) => _logsService = logsService;

    public IActionResult Index()
    {
        return View();
    }


}
