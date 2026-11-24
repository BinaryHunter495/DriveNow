using DriveNow.Models;
using DriveNow.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace DriveNow.Controllers;
[Authorize(Roles = "Customer,Admin")]
public class CustomerController : Controller
{
    private readonly ICarRepository _cars;
    private readonly IBookingRepository _bookings;
    public CustomerController(ICarRepository cars, IBookingRepository bookings)
    { _cars = cars; _bookings = bookings; }
    public async Task<IActionResult> Index() =>
        View(await _bookings.GetByCustomerAsync(User.Identity!.Name!));
    [HttpGet]
    public async Task<IActionResult> Book(int id)
    {
        var car = await _cars.GetByIdAsync(id);
        return car == null ? NotFound() : View(car);
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Book(int id, int days)
    {
        var car = await _cars.GetByIdAsync(id);
        if (car == null) return NotFound();
        if (days < 1 || days > 30)
        {
            ModelState.AddModelError("days", "Please select between 1 and 30 days.");
            return View(car);
        }
        await _bookings.AddAsync(new Booking { CarId = id, Days = days, UserEmail = User.Identity!.Name! });
        TempData["Success"] = "Your booking has been confirmed.";
        return RedirectToAction(nameof(Index));
    }
}
