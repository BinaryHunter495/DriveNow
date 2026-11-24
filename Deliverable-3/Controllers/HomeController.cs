using System.Diagnostics;
using DriveNow.Models;
using DriveNow.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DriveNow.Controllers;

public class HomeController : Controller
{
    private readonly ICarRepository _cars;
    public HomeController(ICarRepository cars) => _cars = cars;

    public async Task<IActionResult> Index() => View(await _cars.GetAllAsync());

    public IActionResult Error() => View("Error", new ErrorViewModel
    {
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
    });

    [HttpGet]
    public async Task<IActionResult> SearchCars(string? term)
    {
        var cars = await _cars.GetAllAsync(term);
        return PartialView("_CarCards", cars);
    }
}
