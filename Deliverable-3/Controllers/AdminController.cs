using DriveNow.Models;
using DriveNow.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DriveNow.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ICarRepository _cars;
    private readonly IBookingRepository _bookings;
    private readonly IDashboardRepository _dashboard;

    public AdminController(ICarRepository cars, IBookingRepository bookings, IDashboardRepository dashboard)
    {
        _cars = cars;
        _bookings = bookings;
        _dashboard = dashboard;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.Bookings = await _bookings.GetAllAsync();
        ViewBag.Stats = await _dashboard.GetAdminStatsAsync();
        return View(await _cars.GetAllAsync());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddCar(string? name, int rate)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 80 || rate < 1)
            return BadRequest(new { message = "Enter a valid vehicle name and daily rate." });

        var car = new Car { Name = name.Trim(), Rate = rate };
        car.Id = await _cars.AddAsync(car);
        return Json(new { success = true, id = car.Id, name = car.Name, rate = car.Rate, message = "Vehicle added successfully." });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCar(int id)
    {
        var deleted = await _cars.DeleteAsync(id);
        if (!deleted) return Conflict(new { message = "This vehicle has bookings and cannot be deleted." });
        return Json(new { success = true, message = "Vehicle removed from the fleet." });
    }
}
