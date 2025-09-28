using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using InmobiliariaMVC.Models;
using Microsoft.AspNetCore.Authorization;

namespace InmobiliariaMVC.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;


    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    [Authorize(Roles = "Admin,Empleado")]
    public IActionResult Index()
    {
        return View();
    }

    [Authorize(Roles = "Admin,Empleado")]
    public IActionResult Privacy()
    {
        return View();
    }
    [Authorize(Roles = "Admin,Empleado")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
   
}
