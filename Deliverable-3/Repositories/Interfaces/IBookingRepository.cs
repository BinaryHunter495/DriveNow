using DriveNow.Models;
namespace DriveNow.Repositories.Interfaces;
public interface IBookingRepository
{
    Task<IEnumerable<Booking>> GetAllAsync();
    Task<IEnumerable<Booking>> GetByCustomerAsync(string email);
    Task<int> AddAsync(Booking booking);
}
