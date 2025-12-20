using System.Diagnostics;
using Auth.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc;


namespace Auth.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }
    [HttpGet]
    public IActionResult Index()
    {
        _logger.LogInformation("Index action was called.");
        return View();
    }
    [HttpGet]
    public IActionResult Privacy()
    {
        _logger.LogInformation("Privacy action was called.");
        return View();
    }
    [HttpGet]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        _logger.LogError("Error action was called.");
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
