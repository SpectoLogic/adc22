using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using adc22config.Models;
using Microsoft.Extensions.Options;

namespace adc22config.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger,
                       IOptionsSnapshot<Settings> settings)
    {
        _logger = logger;
        var val = settings.Value.Message;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
